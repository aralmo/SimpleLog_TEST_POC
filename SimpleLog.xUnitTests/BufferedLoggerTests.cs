using SimpleLog;
using SimpleLog.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SimpleLog.xUnitTests
{
    [Trait("", "BufferedQueue")]
    public class BufferedLoggerTests
    {
        [Fact(DisplayName = "Waithandlers are correctly working with log bursts")]
        public void BufferedLoggerDontLockup()
        {
            var logger = new SlowLogger();
            var bufferedLogger = new BufferedLogger(logger, 10, BufferExceededBehaviors.Lock);

            Task.Run(() =>
            {
                for (int n = 0; n < 100; n++)
                    bufferedLogger.Log(new LogEntry() { Message = n.ToString() });
            });

            logger.TestFinished.WaitOne(1000);

            Assert.Equal(100, logger.LoggedEntries.Count);
        }

        [Fact(DisplayName = "LogWritting task restarts after completed")]
        public void BufferedLoggerRestartsTask()
        {
            var logger = new SlowLogger();
            var bufferedLogger = new BufferedLogger(logger, 10, BufferExceededBehaviors.Lock);

            bufferedLogger.Log(new LogEntry() { Message = "msg1" });
            while (logger.LoggedEntries.Count < 1) ;
            bufferedLogger.Log(new LogEntry() { Message = "msg2" });
        }

        class SlowLogger : ISimpleLogTarget
        {
            public EventWaitHandle TestFinished = new EventWaitHandle(false, EventResetMode.ManualReset);
            public List<string> LoggedEntries { get; set; } = new List<string>();

            public Func<LogEntry, bool> Condition => throw new NotImplementedException();

            int count;
            public void Log(LogEntry entry)
            {
                LoggedEntries.Add(entry.Message);
                count++;
                if (count == 100)
                    TestFinished.Set();

                Thread.Sleep(1);
            }
        }
    }
}
