using SimpleLog.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleLog.FileTarget
{
    public class FileTarget : SimpleLogTarget
    {
        static internal readonly Func<LogEntry, string> DEFAULT_FORMAT = (entry) => $"{entry.Date.ToString("u")},{entry.LogLevel},{entry.Message}";
        static internal readonly Func<FileTargetNamingOptions, string> DEFAULT_FILENAME = (entry) => $"{entry.LogLevel}{(entry.Sequence == 0 ? string.Empty : $"_{entry.Sequence}")}.log";
        public Func<LogEntry, bool> Condition { get; set; }
        public Func<LogEntry, string> Formatter { get; set; }
        public string FolderPath;
        public Func<FileTargetNamingOptions, string> FileName;
        public int MaxFileSize;
        public Encoding Encoding;
        ILogFilesRepository LogFiles;
        public FileTarget(ILogFilesRepository logFiles)
        {
            LogFiles = logFiles;
        }

        public void Log(LogEntry entry)
        {
            LogFiles.WriteLine(
                GetCurrentFileName(entry), 
                FormatMessage(entry),
                Encoding);
        }

        /// <summary>
        /// Gets filename the log should be written to and manages splitting by sequence number
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private string GetCurrentFileName(LogEntry entry)
        {
            int seq = 0;
            
            //ToDo: Cache filename and check for condition against current first
            string fileName = Path.Combine(FolderPath, FileName(new FileTargetNamingOptions()
            {
                LogLevel = entry.LogLevel,
                Sequence = seq
            }));


            while (LogFiles.GetFileSize(fileName) > MaxFileSize)
            {
                string newfileName = Path.Combine(FolderPath, FileName(new FileTargetNamingOptions()
                {
                    LogLevel = entry.LogLevel,
                    Sequence = ++seq
                }));

                //no sequence is being applied
                if (fileName == newfileName)
                    break;
                else
                    fileName = newfileName;

            }
            return fileName;
        }

        /// <summary>
        /// Formats the message adding fallback to DEFAULT_FORMAT in case of an exception
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private string FormatMessage(LogEntry entry)
        {
            string message;
            try
            {
                message = Formatter(entry);
            }
            catch (Exception ex)
            {
                message = $"Log format exception falling back to default format. {ex.Message}\n{DEFAULT_FORMAT(entry)}";
            }

            return message;
        }
    }
}
