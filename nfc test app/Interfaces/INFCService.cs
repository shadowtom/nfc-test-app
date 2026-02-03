using nfc_test_app.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nfc_test_app.Interfaces
{
    public interface INFCService
    {
        event EventHandler<NFCTagDetectedEventArgs>? TagDetected;
        event EventHandler<string>? StatusChanged;

        Task<bool> IsAvailableAsync();
        Task<bool> IsEnabledAsync();
        Task StartListeningAsync();
        Task StopListeningAsync();
        bool IsListening { get; }
    }
}
