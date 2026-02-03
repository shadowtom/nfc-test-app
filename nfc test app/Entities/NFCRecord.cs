namespace nfc_test_app.Entities
{
    public class NFCRecord
    {
        public string? Type { get; set; }
        public string? Payload { get; set; }
        public byte[]? RawData { get; set; }
    }
}