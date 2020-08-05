using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLog
{
    /// <summary>
    /// Represents an entry in the log system.
    /// </summary>
    public class LogEntry
    {
        static readonly EventId DEFAULT_EVENTID = new EventId(0, string.Empty);
        static readonly object[] DEFAULT_PARAMS = new object[0];
        /// <summary>
        /// EvenId that was passed onto the ILogger.Log method
        /// </summary>
        public EventId EventId { get; set; } = DEFAULT_EVENTID;
        /// <summary>
        /// Date and time at wich this log entry has been created.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Level of this log entry
        /// </summary>
        public LogLevel LogLevel { get; set; }
        /// <summary>
        /// **For future implementation into an Asp.NET Middleware
        /// CorrelationId given by a service orchestration.
        /// </summary>
        public string CorrelationId { get; set; } = string.Empty;
        /// <summary>
        /// Indicates if this entry has already been logged by a log target
        /// </summary>
        public bool HasBeenLogged { get; set; }
        /// <summary>
        /// State object that has been passed onto the ILogger.Log method
        /// </summary>
        public object State { get; set; }
        /// <summary>
        /// Message to log after formatting with the ILogger.Log formatter parameter.
        /// </summary>
        public string Message { get; set; }
        internal Func<object, Exception, string> CustomFormatter { get; set; }
        /// <summary>
        /// Object that was passed when creating a new scope with the ILogger.BeginScope method
        /// </summary>
        public object Scope { get; set; }
        /// <summary>
        /// Exception that was passed onto mosthe ILogger.Log method.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
