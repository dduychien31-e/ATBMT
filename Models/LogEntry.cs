namespace ATBM.Models
{
    public class LogEntry
    {
        public DateTime Time { get; set; }

        public string Action { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Cloud { get; set; } = string.Empty;
    }
}