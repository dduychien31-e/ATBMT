using System.Security.Cryptography;

namespace ATBM.Services
{
    public class EncryptionService
    {
        // Khóa AES 32 byte (AES-256)
        private readonly byte[] Key =
        {
            1,2,3,4,5,6,7,8,
            9,10,11,12,13,14,15,16,
            17,18,19,20,21,22,23,24,
            25,26,27,28,29,30,31,32
        };

        // Vector khởi tạo 16 byte
        private readonly byte[] IV =
        {
            32,31,30,29,
            28,27,26,25,
            24,23,22,21,
            20,19,18,17
        };

        // Mã hóa file
        public void EncryptFile(string inputFile, string outputFile)
        {
            using Aes aes = Aes.Create();

            aes.Key = Key;
            aes.IV = IV;

            using FileStream inputStream = new FileStream(inputFile, FileMode.Open);
            using FileStream outputStream = new FileStream(outputFile, FileMode.Create);

            using CryptoStream cryptoStream =
                new CryptoStream(outputStream,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write);

            inputStream.CopyTo(cryptoStream);
        }

        // Giải mã file
        public void DecryptFile(string inputFile, string outputFile)
        {
            using Aes aes = Aes.Create();

            aes.Key = Key;
            aes.IV = IV;

            using FileStream inputStream = new FileStream(inputFile, FileMode.Open);
            using FileStream outputStream = new FileStream(outputFile, FileMode.Create);

            using CryptoStream cryptoStream =
                new CryptoStream(inputStream,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read);

            cryptoStream.CopyTo(outputStream);
        }
    }
}