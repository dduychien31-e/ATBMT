namespace ATBM.Models
{
    public class FileMetadata
    {
        public string FileName { get; set; } = string.Empty;

        public string EncryptedFileName { get; set; } = string.Empty;

        public string Hash { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadTime { get; set; }

        public List<string> Clouds { get; set; } = new();
    }
}