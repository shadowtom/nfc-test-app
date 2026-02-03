using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nfc_test_app.Entities
{
    public class NFCTagDetectedEventArgs
    {
        public string? TagId { get; set; }
        public string? TagType { get; set; }
        public List<NFCRecord> Records { get; set; } = new();
        public DateTime DetectedAt { get; set; }
    }
}
