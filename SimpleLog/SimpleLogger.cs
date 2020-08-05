using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLog
{
    /// <summary>
    /// The CloudIOLogger, this class takes care of conditions and call the appropiate log targets.
    /// </summary>
    public class SimpleLogger : ILogger
    {
        LogBuilder Options { get; set; }
        public string CorrelationId { get; private set; }
        public SimpleLogger(LogBuilder options)
        {
            Options = options;
        }
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var entry = new LogEntry()
            {
                Date = DateTime.Now,
                LogLevel = logLevel,
                EventId = eventId,
                State = state,
                Exception = exception,
                Scope = LogScopesProvider.GetState(),
                Message = formatter(state, exception)
            };

            //iterate each logger in order and log if conditions are met
            foreach(var logger in Options.LogTargets)
            {
                if (logger.Condition == null || logger.Condition(entry))
                {
                    logger.Log(entry);
                    entry.HasBeenLogged = true;
                }
            }
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return
                Options.EnabledLevels == null ||
                Options.EnabledLevels
                    .Contains(logLevel);
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return
                LogScopesProvider
                .CreateScope(state);
        }
    }
}
