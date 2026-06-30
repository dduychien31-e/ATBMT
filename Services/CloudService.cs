namespace ATBM.Services
{
    public class CloudService
    {
        private readonly string[] clouds =
        {
            "CloudA",
            "CloudB",
            "CloudC"
        };

        // Upload file lên tất cả cloud
        public void UploadToAllClouds(string sourceFile)
        {
            foreach (var cloud in clouds)
            {
                Directory.CreateDirectory(cloud);

                string destination = Path.Combine(
                    cloud,
                    Path.GetFileName(sourceFile));

                File.Copy(sourceFile, destination, true);
            }
        }

        // Kiểm tra file tồn tại trên cloud
        public bool FileExists(string cloud, string fileName)
        {
            return File.Exists(
                Path.Combine(cloud, fileName));
        }

        // Lấy đường dẫn file trên cloud
        public string GetFilePath(string cloud, string fileName)
        {
            return Path.Combine(cloud, fileName);
        }

        // Danh sách cloud
        public List<string> GetClouds()
        {
            return clouds.ToList();
        }
    }
}