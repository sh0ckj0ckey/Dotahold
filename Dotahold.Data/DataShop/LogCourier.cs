﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Dotahold.Data.DataShop
{
    /// <summary>
    /// 这是一只信使，帮你运送各处输出的日志到日志文件中
    /// </summary>
    public static class LogCourier
    {
        public enum LogType
        {
            Info,
            Warning,
            Error,
        }

        private static readonly ConcurrentQueue<string> _logQueue = new();

        private static readonly AutoResetEvent _logEvent = new(false);

        private static readonly CancellationTokenSource _cancellationTokenSource = new();

        private static readonly Task _logTask = Task.Run(() => ProcessLogQueue(_cancellationTokenSource.Token));

        private static StorageFile? _logFile = null;

        private const int _maxLogFileSize = 10 * 1024 * 1024;

        private const int _maxLogLines = 10000;

        public static void Log(string message, LogType logType = LogType.Info)
        {
            try
            {
                var stackTrace = new StackTrace();
                var stackFrame = stackTrace.GetFrame(1);
                string callingMethodName = stackFrame is not null ? DiagnosticMethodInfo.Create(stackFrame)?.Name ?? "" : "";

                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logType}] {callingMethodName}: {message}";
                Debug.WriteLine(logMessage);
                _logQueue.Enqueue(logMessage);
                _logEvent.Set();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private static async Task ProcessLogQueue(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _logEvent.WaitOne();

                while (_logQueue.TryDequeue(out string? logMessage))
                {
                    try
                    {
                        await WriteLogToFileAsync(logMessage);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }
            }
        }

        private static async Task WriteLogToFileAsync(string logMessage)
        {
            if (string.IsNullOrWhiteSpace(logMessage))
            {
                return;
            }

            _logFile ??= await ApplicationData.Current.LocalFolder.CreateFileAsync("dotahold.log", CreationCollisionOption.OpenIfExists);

            // 检查文件大小
            var fileProperties = await _logFile.GetBasicPropertiesAsync();
            if (fileProperties.Size > _maxLogFileSize)
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
            // 只保留后半部分日志
            var lines = await FileIO.ReadLinesAsync(logFile);
            var newLines = lines.Skip(lines.Count / 2);
            await FileIO.WriteLinesAsync(logFile, newLines);
        }

        public static void StopLogging()
        {
            try
            {
                _cancellationTokenSource.Cancel();
                _logEvent.Set();
                _logTask.Wait();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
    }
}
