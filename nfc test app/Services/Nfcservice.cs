using Microsoft.Extensions.Logging;
using nfc_test_app.Entities;
using nfc_test_app.Interfaces;
using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nfc_test_app.Services
{
    internal class Nfcservice : INFCService
    {
        private readonly ILogger<Nfcservice>? _logger;
        private bool _isListening;

        public event EventHandler<NFCTagDetectedEventArgs>? TagDetected;
        public event EventHandler<string>? StatusChanged;

        public bool IsListening => _isListening;
        public Nfcservice(ILogger<Nfcservice>? logger = null)
        {
            _logger = logger;

            // Subscribe to NFC events
            CrossNFC.Current.OnMessageReceived += OnMessageReceived;
            CrossNFC.Current.OnTagDiscovered += OnTagDiscovered;
        }
        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var available = CrossNFC.Current.IsAvailable;
                _logger?.LogInformation($"NFC Available: {available}");
                return await Task.FromResult(available);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking NFC availability");
                return false;
            }
        }

        public async Task<bool> IsEnabledAsync()
        {
            try
            {
                var enabled = CrossNFC.Current.IsEnabled;
                _logger?.LogInformation($"NFC Enabled: {enabled}");
                return await Task.FromResult(enabled);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking if NFC is enabled");
                return false;
            }
        }

        public async Task StartListeningAsync()
        {
            try
            {
                if (!await IsAvailableAsync())
                {
                    StatusChanged?.Invoke(this, "NFC no está disponible en este dispositivo");
                    return;
                }

                if (!await IsEnabledAsync())
                {
                    StatusChanged?.Invoke(this, "NFC está desactivado. Por favor actívalo en Configuración");
                    return;
                }

                CrossNFC.Current.StartListening();
                _isListening = true;
                StatusChanged?.Invoke(this, "Escuchando tarjetas NFC...");
                _logger?.LogInformation("NFC listening started");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error starting NFC listening");
                StatusChanged?.Invoke(this, $"Error: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        public async Task StopListeningAsync()
        {
            try
            {
                CrossNFC.Current.StopListening();
                _isListening = false;
                StatusChanged?.Invoke(this, "Detenido");
                _logger?.LogInformation("NFC listening stopped");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error stopping NFC listening");
            }

            await Task.CompletedTask;
        }
        private void OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            try
            {
                if (tagInfo == null) return;

                var tagData = new NFCTagDetectedEventArgs
                {
                    TagId = BitConverter.ToString(tagInfo.Identifier ?? Array.Empty<byte>()).Replace("-", ""),
                    TagType = tagInfo.GetType().Name,
                    DetectedAt = DateTime.Now
                };

                _logger?.LogInformation($"Tag discovered: {tagData.TagId}");

                TagDetected?.Invoke(this, tagData);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing discovered tag");
            }
        }

        private void OnMessageReceived(ITagInfo tagInfo)
        {
            try
            {
                if (tagInfo == null) return;

                var tagData = new NFCTagDetectedEventArgs
                {
                    TagId = BitConverter.ToString(tagInfo.Identifier ?? Array.Empty<byte>()).Replace("-", ""),
                    TagType = tagInfo.GetType().Name,
                    DetectedAt = DateTime.Now,
                    Records = new List<NFCRecord>()
                };

                // Parse NDEF records if available
                if (tagInfo.Records != null && tagInfo.Records.Any())
                {
                    foreach (var record in tagInfo.Records)
                    {
                        var nfcRecord = new NFCRecord
                        {
                            Type = record.TypeFormat.ToString(),
                            RawData = record.Payload
                        };

                        // Try to parse payload as text
                        try
                        {
                            if (record.Payload != null && record.Payload.Length > 0)
                            {
                                // Check if it's a text record
                                if (record.TypeFormat == Plugin.NFC.NFCNdefTypeFormat.WellKnown)
                                {
                                    // Skip the first byte (status byte) and decode
                                    var languageCodeLength = record.Payload[0] & 0x3F;
                                    var textBytes = record.Payload.Skip(1 + languageCodeLength).ToArray();
                                    nfcRecord.Payload = Encoding.UTF8.GetString(textBytes);
                                }
                                else
                                {
                                    nfcRecord.Payload = Encoding.UTF8.GetString(record.Payload);
                                }
                            }
                        }
                        catch
                        {
                            nfcRecord.Payload = BitConverter.ToString(record.Payload ?? Array.Empty<byte>());
                        }

                        tagData.Records.Add(nfcRecord);
                    }
                }

                _logger?.LogInformation($"Message received from tag: {tagData.TagId}");

                TagDetected?.Invoke(this, tagData);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing received message");
            }
        }
    }
}
