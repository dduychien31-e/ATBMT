using ATBM.Models;

namespace ATBM.Services
{
    public class LogService
    {
        private readonly string logFile =
            Path.Combine("Logs", "system.log");

        public void WriteLog(LogEntry entry)
        {
            Directory.CreateDirectory("Logs");

            string log =
                $"{entry.Time:yyyy-MM-dd HH:mm:ss} | " +
                $"{entry.Action} | " +
                $"{entry.FileName} | " +
                $"{entry.Cloud} | " +
                $"{entry.Status}";

            File.AppendAllText(
            logFile,
            log + Environment.NewLine,
            System.Text.Encoding.UTF8);
        }

        public List<string> ReadLogs()
        {
            Directory.CreateDirectory("Logs");

            if (!File.Exists(logFile))
                return new List<string>();

            return File.ReadAllLines(logFile).ToList();
        }
    }
}