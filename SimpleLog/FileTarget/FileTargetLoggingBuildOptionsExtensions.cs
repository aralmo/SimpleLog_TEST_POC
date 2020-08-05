using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLog.FileTarget
{
    public static class FileTargetLoggingBuildOptionsExtensions
    {
        const int MB = 1024 * 1024;
        private static readonly LogFilesRepository Repository = new LogFilesRepository();

#warning complete comments
        /// <summary>
        /// Adds a file target to the logging system
        /// </summary>
        /// <param name="options"></param>
        /// <param name="folderPath">Path where the log files should be saved on</param>
        /// <param name="fileName">Function to determine filename</param>
        /// <param name="maxFileSize">Maximum file size before file name sequence will be increased</param>
        /// <param name="condition">Log conditions for this target</param>
        /// <param name="formatter">Message formatter for this target</param>
        /// <param name="encoding">Encoding of the file, defaults to: Encoding.Default</param>
        /// <param name="enableBuffering">Sets if buffering should be used with this target</param>
        /// <param name="maxBufferSize">Maximum number of elements in the buffer</param>
        /// <param name="bufferExceededBehavior">Sets the behavior of this target when the buffer is full and new entries are logged</param>
        public static LogBuilder AddFileTarget(
            this LogBuilder options,
            string folderPath = ".\\logs",
            Func<FileTargetNamingOptions,string> fileName = null,
            int maxFileSize = 100*MB,
            Encoding encoding = null,
            Func<LogEntry, bool> condition = null,
            Func<LogEntry, string> formatter = null,
            bool enableBuffering = true,
            int maxBufferSize = BufferSize.Infinite,
            BufferExceededBehaviors bufferExceededBehavior = BufferExceededBehaviors.Lock)
        {
            var fileLogger = new FileTarget(Repository)
            {
                Formatter = formatter ?? FileTarget.DEFAULT_FORMAT,
                Condition = condition,
                FolderPath = folderPath,
                FileName = fileName ?? FileTarget.DEFAULT_FILENAME,
                MaxFileSize = maxFileSize,
                Encoding = encoding??Encoding.Default
            };

            if (enableBuffering)
            {
                options.AddLogTarget(
                    new BufferedLogger(
                        fileLogger,
                        maxBufferSize,
                        bufferExceededBehavior));
            }
            else
            {
                options.AddLogTarget(fileLogger);
            }
            return options;
        }


    }
}
