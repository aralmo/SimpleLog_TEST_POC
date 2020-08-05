using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace SimpleLog.FileTarget
{
    /// <summary>
    /// This repository handles multiple threads writting to same file.
    /// </summary>
    public class LogFilesRepository : ILogFilesRepository
    {
        static Dictionary<string, LogFile> LogFiles { get; set; } = new Dictionary<string, LogFile>();
        public long GetFileSize(string filename)
        {
            if (LogFiles.TryGetValue(filename, out LogFile logFile))
            {
                return logFile.Size;
            }
            else
            {
                var info = new FileInfo(filename);
                return info.Exists ? info.Length : 0;
            }
        }

        static readonly object writeLock = new object();
        public void WriteLine(string filename, string message, Encoding encoding)
        {
            lock (writeLock)
            {
                var logFile = GetLogFileItem(filename);
                logFile.WriteLine(message, encoding);
            }
        }

        private static LogFile GetLogFileItem(string filename)
        {
            if (!LogFiles.TryGetValue(filename, out LogFile logFile))
            {

                var info = new FileInfo(filename);
                if (!info.Exists)
                {
                    //if directory does not exist create it
                    string folder = Path.GetDirectoryName(filename);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                }

                logFile = new LogFile()
                {
                    Filename = filename,
                    Size = info.Exists ? info.Length : 0
                };

                LogFiles.TryAdd(filename, logFile);
            }

            return logFile;
        }
        
        /// <summary>
        /// Represents a log file static through all threads that may be writting to it.
        /// </summary>
        class LogFile
        {
            public string Filename { get; set; }
            public long Size { get; set; }
            public Task WriteTask { get; set; }
            public Encoding Encoding { get; set; } = null;

            readonly object LockObject = new object();

            Task FlushTask;
            CancellationTokenSource FlushTaskCancellationTokenSource;
            StringBuilder Buffer = new StringBuilder();
            public void WriteLine(string line, Encoding encoding)
            {
                //one thread at a time
                lock (LockObject)
                {
                    if (Encoding == null)
                        Encoding = encoding;
                    else
                    {
                        //could be a mess otherwise
                        if (Encoding != encoding)
                            throw new InvalidOperationException($"Multiple encodings detected while writting to file {Filename}");
                    }

                    //Wait until there are 64kb of data in the buffer before start writting and lock after that 
                    //if the write task is working
                    Buffer.AppendLine(line);
                    if (WriteTask?.IsCompleted ?? true)
                    {
                        if (Buffer.Length >= 64 * 1024)
                        {
                            WriteBuffer();
                        }
                        else
                        {
                            //If it's less than 64kb in the buffer, create a task to automatically flush it after 2 seconds
                            CreateFlushTask();
                        }
                    }
                    else
                    {
                        if (Buffer.Length > 64 * 1024)
                        {
                            //lock this thread until buffer has been emptied by writter
                            WriteTask.Wait();
                        }
                    }
                }
            }

            private void CreateFlushTask()
            {
                if (FlushTask?.IsCompleted ?? true)
                {
                    FlushTaskCancellationTokenSource = new CancellationTokenSource();
                    FlushTask = Task.Run(() =>
                    {
                        Thread.Sleep(500); //wait 2 seconds before writting the contents of the buffer
                        if (!FlushTaskCancellationTokenSource.Token.IsCancellationRequested)
                            WriteBuffer();

                    }, FlushTaskCancellationTokenSource.Token);
                }
            }

            /// <summary>
            /// Writes the current buffer to the target file and adds size to the cache so no fileinfo is constantly required
            /// </summary>
            private void WriteBuffer()
            {
                //Cancels the flush task if it's waiting
                FlushTaskCancellationTokenSource.Cancel();

                string data = Buffer.ToString();
                Size += Encoding.GetByteCount(data);
                Buffer = new StringBuilder();
                WriteTask = File.AppendAllTextAsync(Filename, data, Encoding);
            }

            public void Flush()
            {
                if (WriteTask != null && WriteTask.IsCompleted == false)
                    WriteTask.Wait();

                File.AppendAllText(Filename, Buffer.ToString(), Encoding);
            }
        }

        public void Flush()
        {
            foreach (var file in LogFiles.Values)
                file.Flush();
        }

        ~LogFilesRepository()
        {
            Flush();
        }
    }

}
