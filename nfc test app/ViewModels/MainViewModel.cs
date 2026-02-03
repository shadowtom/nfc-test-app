using nfc_test_app.Entities;
using nfc_test_app.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace nfc_test_app.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly INFCService _nfcService;
        private string _statusMessage = "Toca 'Iniciar Escaneo' para comenzar";
        private bool _isScanning;
        private string _lastTagId = "-";
        private string _lastTagType = "-";
        private DateTime? _lastDetectionTime;
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<TagHistoryItem> TagHistory { get; } = new();
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                _isScanning = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ScanButtonText));
                OnPropertyChanged(nameof(ScanButtonColor));
            }
        }

        public string LastTagId
        {
            get => _lastTagId;
            set
            {
                _lastTagId = value;
                OnPropertyChanged();
            }
        }

        public string LastTagType
        {
            get => _lastTagType;
            set
            {
                _lastTagType = value;
                OnPropertyChanged();
            }
        }

        public string LastDetectionTime
        {
            get => _lastDetectionTime?.ToString("HH:mm:ss") ?? "-";
        }

        public string ScanButtonText => IsScanning ? "Detener Escaneo" : "Iniciar Escaneo";

        public Color ScanButtonColor => IsScanning ? Colors.Red : Color.FromArgb("#512BD4");

        public ICommand ToggleScanCommand { get; }
        public ICommand ClearHistoryCommand { get; }

        public MainViewModel(INFCService nfcService)
        {
            _nfcService = nfcService;

            // Subscribe to NFC events
            _nfcService.TagDetected += OnTagDetected;
            _nfcService.StatusChanged += OnStatusChanged;

            // Initialize commands
            ToggleScanCommand = new Command(async () => await ToggleScanAsync());
            ClearHistoryCommand = new Command(() => TagHistory.Clear());
        }

        private async Task ToggleScanAsync()
        {
            if (IsScanning)
            {
                await _nfcService.StopListeningAsync();
                IsScanning = false;
            }
            else
            {
                await _nfcService.StartListeningAsync();
                IsScanning = true;
            }
        }

        private void OnTagDetected(object? sender, NFCTagDetectedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LastTagId = e.TagId ?? "Desconocido";
                LastTagType = e.TagType ?? "Desconocido";
                _lastDetectionTime = e.DetectedAt;
                OnPropertyChanged(nameof(LastDetectionTime));

                StatusMessage = $"¡Tarjeta detectada! ID: {LastTagId}";

                // Add to history
                var historyItem = new TagHistoryItem
                {
                    TagId = LastTagId,
                    TagType = LastTagType,
                    DetectedAt = e.DetectedAt,
                    RecordCount = e.Records.Count,
                    Records = e.Records
                };

                TagHistory.Insert(0, historyItem);

                // Keep only last 20 items
                while (TagHistory.Count > 20)
                {
                    TagHistory.RemoveAt(TagHistory.Count - 1);
                }
            });
        }

        private void OnStatusChanged(object? sender, string status)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StatusMessage = status;
            });
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
