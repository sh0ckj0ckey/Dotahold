using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Dotahold.Core.DataShop
{
    public static class LogCourier
    {
        public enum LogType
        {
            Info,
            Warning,
            Error,
        }

        private static readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private static readonly AutoResetEvent _logEvent = new AutoResetEvent(false);
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static readonly Task _logTask = Task.Run(() => ProcessLogQueue(_cancellationTokenSource.Token));

        private const int MaxLogFileSize = 5 * 1024 * 1024; // 5 MB
        private const int MaxLogLines = 10000; // 最大日志行数

        private static StorageFile _logFile = null;

        public static void LogAsync(string message, LogType logType = LogType.Info)
        {
            var stackTrace = new StackTrace();
            var callingMethod = stackTrace.GetFrame(1).GetMethod().Name;

            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logType}] {callingMethod}: {message}";
            Debug.WriteLine(logMessage);
            _logQueue.Enqueue(logMessage);
            _logEvent.Set();
        }

        private static async Task ProcessLogQueue(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _logEvent.WaitOne();

                while (_logQueue.TryDequeue(out string logMessage))
                {
                    await WriteLogToFileAsync(logMessage);
                }
            }
        }

        private static async Task WriteLogToFileAsync(string logMessage)
        {
            if (_logFile is null)
            {
                _logFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("dotahold.log", CreationCollisionOption.OpenIfExists);
            }

            // 检查文件大小
            var fileProperties = await _logFile.GetBasicPropertiesAsync();
            if (fileProperties.Size > MaxLogFileSize)
            {
                await ClearOldLogsAsync(_logFile);
            }

            // 检查文件行数
            //var lines = await FileIO.ReadLinesAsync(logFile);
            //if (lines.Count > MaxLogLines)
            //{
            //    await ClearOldLogsAsync(logFile);
            //}

            await FileIO.AppendTextAsync(_logFile, logMessage + Environment.NewLine);
        }

        private static async Task ClearOldLogsAsync(StorageFile logFile)
        {
            try
            {
                var lines = await FileIO.ReadLinesAsync(logFile);
                var newLines = lines.Skip(lines.Count / 2).ToList(); // 保留后半部分日志
                await FileIO.WriteLinesAsync(logFile, newLines);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void StopLogging()
        {
            _cancellationTokenSource.Cancel();
            _logEvent.Set();
            _logTask.Wait();
        }
    }
}
