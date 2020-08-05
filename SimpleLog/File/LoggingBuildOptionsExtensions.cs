using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudIO.Logging.File
{
    public static class LoggingBuildOptionsExtensions
    {
        static readonly Func<LogEntry, string> DEFAULT_FORMAT = (entry) => $"{entry.Date.ToLongDateString()},{entry.LogLevel},{entry.CorrelationId},{entry.Message}";
        const int MB = 1024 * 1024;
        public static void AddFileTarget(
            this LoggingBuildOptions options, 
            LogEntryLevels level = LogEntryLevels.All, 
            string path=".\\log",
            int maxFileSize = 100*MB,
            bool keepFileOpen = false,//si se mantiene abierta y se añade secuencia al nombre, cada hilo usaria un fichero
            Func<FileNameParameters,string> filename = null,
            Func<LogEntry, string> formatter = null)
        {

        }
    }
}
