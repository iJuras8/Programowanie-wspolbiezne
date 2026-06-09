using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    internal class Logger : IDisposable
    {
        private readonly string _filePath;
        private readonly ConcurrentQueue<string> _logQueue = new();
        private readonly Task _loggingTask;
        private bool _isDisposed = false;

        public Logger()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BallsDiagnosticData.log");

            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            _loggingTask = Task.Run(WriteToFileLoop);
        }

        public void Log(string ballName, IVector position, IVector velocity)
        {
            if (_isDisposed) return;

            string logEntry = $"[{ballName}] Time: {DateTime.Now:HH:mm:ss.fff} | Pos: X={position.x:F2}, Y={position.y:F2} | Vel: X={velocity.x:F2}, Y={velocity.y:F2}";
            _logQueue.Enqueue(logEntry);
        }

        private async Task WriteToFileLoop()
        {
            using StreamWriter writer = new StreamWriter(_filePath, append: true);

            while (!_isDisposed || !_logQueue.IsEmpty)
            {
                if (_logQueue.TryDequeue(out string? log))
                {
                    await writer.WriteLineAsync(log);
                }
                else
                {
                    await Task.Delay(10);
                }
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            _loggingTask.Wait();
        }
    }
}