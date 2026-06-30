using ATBM.Models;
using ATBM.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace ATBM.Controllers
{
    public class HomeController : Controller
    {
        private readonly HashService hashService = new();
        private readonly EncryptionService encryptionService = new();
        private readonly MetadataService metadataService = new();
        private readonly CloudService cloudService = new();
        private readonly LogService logService = new();

        public IActionResult Index()
        {
            return View(metadataService.GetAll());
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return RedirectToAction("Index");

            Directory.CreateDirectory("Uploads");

            // 1. Lưu file gốc
            string originalPath = Path.Combine("Uploads", file.FileName);

            using (var stream = new FileStream(originalPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // 2. Mã hóa AES
            string encryptedPath = originalPath + ".enc";

            encryptionService.EncryptFile(originalPath, encryptedPath);

            // 3. Tính SHA256
            string hash = hashService.ComputeHash(encryptedPath);

            // 4. Upload lên 3 cloud
            cloudService.UploadToAllClouds(encryptedPath);

            // 5. Lưu metadata
            metadataService.Save(new FileMetadata
            {
                FileName = file.FileName,
                EncryptedFileName = Path.GetFileName(encryptedPath),
                Hash = hash,
                FileSize = file.Length,
                UploadTime = DateTime.Now,
                Clouds = cloudService.GetClouds()
            });

            // 6. Ghi log
            foreach (var cloud in cloudService.GetClouds())
            {
                logService.WriteLog(new LogEntry
                {
                    Time = DateTime.Now,
                    Action = "Upload",
                    FileName = file.FileName,
                    Cloud = cloud,
                    Status = "Success"
                });
            }

            return RedirectToAction("Index");
        }
        public IActionResult Download(string fileName)
        {
            // Tìm thông tin file trong metadata
            var metadata = metadataService.GetByFileName(fileName);

            if (metadata == null)
            {
                return NotFound("Không tìm thấy metadata của file.");
            }

            // Tìm file mã hóa trong các cloud
            string? encryptedPath = null;

            foreach (var cloud in metadata.Clouds)
            {
                string path = cloudService.GetFilePath(cloud, metadata.EncryptedFileName);

                if (System.IO.File.Exists(path))
                {
                    encryptedPath = path;
                    break;
                }
            }

            if (encryptedPath == null)
            {
                return NotFound("Không tìm thấy file trên các cloud.");
            }

            // Tạo thư mục Download nếu chưa có
            Directory.CreateDirectory("Downloads");

            // Đường dẫn file sau khi giải mã
            string outputPath = Path.Combine("Downloads", metadata.FileName);

            // Giải mã
            encryptionService.DecryptFile(encryptedPath, outputPath);

            // Đọc file và trả về cho trình duyệt
            byte[] bytes = System.IO.File.ReadAllBytes(outputPath);

            return File(bytes, "application/octet-stream", metadata.FileName);
        }
        public IActionResult Verify(string fileName)
        {
            var metadata = metadataService.GetByFileName(fileName);

            if (metadata == null)
            {
                return Content("Không tìm thấy metadata.");
            }

            string result = "";

            foreach (var cloud in metadata.Clouds)
            {
                string path = cloudService.GetFilePath(cloud, metadata.EncryptedFileName);

                if (!System.IO.File.Exists(path))
                {
                    result += $"{cloud}: ❌ Mất file<br>";
                    continue;
                }

                string currentHash = hashService.ComputeHash(path);

                if (currentHash == metadata.Hash)
                {
                    result += $"{cloud}: ✅ Toàn vẹn<br>";
                }
                else
                {
                    result += $"{cloud}: ❌ File bị sửa đổi<br>";
                }
            }

            return Content(result, "text/html; charset=utf-8");
        }
        public IActionResult Recovery(string fileName)
        {
            var metadata = metadataService.GetByFileName(fileName);

            if (metadata == null)
            {
                return Content("Không tìm thấy metadata.");
            }

            string? sourceFile = null;

            // Tìm cloud còn file
            foreach (var cloud in metadata.Clouds)
            {
                string path = cloudService.GetFilePath(cloud, metadata.EncryptedFileName);

                if (System.IO.File.Exists(path))
                {
                    sourceFile = path;
                    break;
                }
            }

            if (sourceFile == null)
            {
                return Content("Không còn bản sao nào để khôi phục.");
            }

            // Khôi phục cloud bị mất file
            foreach (var cloud in metadata.Clouds)
            {
                string destination = cloudService.GetFilePath(cloud, metadata.EncryptedFileName);

                if (!System.IO.File.Exists(destination))
                {
                    System.IO.File.Copy(sourceFile, destination, true);

                    logService.WriteLog(new LogEntry
                    {
                        Time = DateTime.Now,
                        Action = "Recovery",
                        FileName = metadata.FileName,
                        Cloud = cloud,
                        Status = "Recovered"
                    });
                }
            }

            return RedirectToAction("Index");
        }
    }
}