using SimpleLog;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Clou.IO.Logging.xUnitTests
{
    public class ScopesProviderIsThreadSafe
    {
        [Trait("", "LogScopesProvider")]
        [Fact(DisplayName = "Is aware of threads")]
        public void MultiThreadedScopesPoC()
        {
            ConcurrentQueue<string> logs = new ConcurrentQueue<string>();
            var logger = new Logger((m) => logs.Enqueue(m));
            object lock_object = new object();

            var t1 = Task.Run(() =>
            {
                using (logger.BeginScope("thread 1 scope"))
                {
                    Thread.Sleep(20);
                    logger.LogInformation("thread 1 info");
                }
            });

            var t2 = Task.Run(() =>
            {
                Thread.Sleep(10);
                using (logger.BeginScope("thread 2 scope"))
                {
                    logger.LogInformation("thread 2 info");
                }
            });

            Task.WaitAll(t1, t2);
            var result = string.Join('\n', logs.ToArray());

            string[] validResults = new string[] { "thread 2 scope,thread 2 info\nthread 1 scope,thread 1 info", "thread 1 scope,thread 1 info\nthread 2 scope,thread 2 info" };
            Assert.Contains(result, validResults);
        }

        class Logger : ILogger
        {
            readonly Action<string> log;
            public Logger(Action<string> log) => this.log = log;
            public IDisposable BeginScope<TState>(TState state)
            {
                return LogScopesProvider.CreateScope(state);
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                object scope = LogScopesProvider.GetState();
                log($"{scope},{state}");
            }
        }
    }
}
