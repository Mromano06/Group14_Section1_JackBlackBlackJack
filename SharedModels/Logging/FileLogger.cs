using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Logging
{
    public static class FileLogger
    {
        private static readonly string _logPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "log.txt");


        private static readonly object _lock = new object();

        public static void Log(string message)
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            lock (_lock)
            {
                File.AppendAllText(_logPath, entry + Environment.NewLine);
            }
        }

    }
}
