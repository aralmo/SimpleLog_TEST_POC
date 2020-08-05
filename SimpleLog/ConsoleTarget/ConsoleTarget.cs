using SimpleLog.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLog.ConsoleTarget
{
    public class ConsoleTarget : SimpleLogTarget
    {
        static internal readonly Func<LogEntry, string> DEFAULT_FORMAT = (entry) => $"{entry.Date.ToString("yyyy-MM-dd hh:ss")},{entry.LogLevel},{entry.Message}";
        public Func<LogEntry, bool> Condition { get; set; }
        public Func<LogEntry, string> Formatter { get; set; }
        public ConsoleOutputs OutputStream { get; set; }
            = ConsoleOutputs.StandardOutput;

        Action<string> StandardOutputWriteLine;
        Action<string> StandardErrorWriteLine;
        public ConsoleTarget(Action<string> standardOutputWriteLine, Action<string> standardErrorWriteLine)
        {
            StandardOutputWriteLine = standardOutputWriteLine;
            StandardErrorWriteLine = standardErrorWriteLine;
        }

        public void Log(LogEntry entry)
        {
            //If message formatter was customized by using the log format parameter use that
            //else use the simplelog formatter set in the targets configuration.
            string message = FormatMessage(entry);

            switch (OutputStream)
            {
                case ConsoleOutputs.StandardOutput:
                    StandardOutputWriteLine(message);
                    break;
                case ConsoleOutputs.StandardError:
                    StandardErrorWriteLine(message);
                    break;
            }
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
