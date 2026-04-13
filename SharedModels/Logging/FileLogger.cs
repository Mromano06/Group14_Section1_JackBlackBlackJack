using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Logging
{
    /// <summary>
    /// Provides simple txt file based logging functionality.
    /// </summary>
    /// <remarks>
    /// This static logger writes timestamped log entries to a text file.
    /// 
    /// Thread safety is ensured using a lock to prevent concurrent write issues.
    /// </remarks>
    public static class FileLogger
    {
        /// <summary>
        /// The full path to the log file.
        /// </summary>
        /// <remarks>
        /// The log file is named <c>log.txt</c>.
        /// </remarks>
        private static readonly string _logPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "log.txt");

        /// <summary>
        /// Synchronization object used to ensure thread-safe file access.
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Writes a message to the log file with a timestamp.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// Each log entry is created using the current date and time
        /// in the format <c>yyyy-MM-dd HH:mm:ss</c>.
        /// 
        /// Example output:
        /// <code>
        /// [2026-04-12 18:30:45] Player placed a bet of $50
        /// </code>
        /// </remarks>
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
