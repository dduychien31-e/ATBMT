using System.Text.Json;
using ATBM.Models;

namespace ATBM.Services
{
    public class MetadataService

    {
        private readonly string metadataFile =
            Path.Combine("Metadata", "metadata.json");

        public MetadataService()
        {
            Directory.CreateDirectory("Metadata");

            if (!File.Exists(metadataFile))
            {
                File.WriteAllText(metadataFile, "[]");
            }
        }

        // Đọc danh sách metadata
        public List<FileMetadata> GetAll()
        {
            string json = File.ReadAllText(metadataFile);

            return JsonSerializer.Deserialize<List<FileMetadata>>(json)
                   ?? new List<FileMetadata>();
        }

        // Thêm metadata mới
        public void Save(FileMetadata metadata)
        {
            var list = GetAll();

            list.Add(metadata);

            string json = JsonSerializer.Serialize(
                list,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(metadataFile, json);
        }
        // Tìm metadata theo tên file
        public FileMetadata? GetByFileName(string fileName)
        {
            return GetAll().FirstOrDefault(x => x.FileName == fileName);
        }
    }
}