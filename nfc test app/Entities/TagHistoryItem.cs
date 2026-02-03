using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nfc_test_app.Entities
{
    public class TagHistoryItem
    {
        public string TagId { get; set; } = string.Empty;
        public string TagType { get; set; } = string.Empty;
        public DateTime DetectedAt { get; set; }
        public int RecordCount { get; set; }
        public List<NFCRecord> Records { get; set; } = new();

        public string DisplayText => $"{TagId} - {DetectedAt:HH:mm:ss}";
        public string DetailText => $"{TagType} • {RecordCount} registro(s)";
    }
}
