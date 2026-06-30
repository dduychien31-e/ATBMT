using System.Security.Cryptography;

namespace ATBM.Services
{
    public class HashService
    {
        // Tạo mã băm SHA256 từ file
        public string ComputeHash(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);

            byte[] hash = sha256.ComputeHash(stream);

            return Convert.ToHexString(hash);
        }

        // So sánh hash của file với hash đã lưu
        public bool VerifyHash(string filePath, string expectedHash)
        {
            string currentHash = ComputeHash(filePath);

            return currentHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}