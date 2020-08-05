using SimpleLog.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleLog
{
    /// <summary>
    /// This logger target buffers log entries and call the nested logger passed as parameter when required.
    /// </summary>
    public class BufferedLogger : ISimpleLogTarget
    {
        EventWaitHandle QueueIsNotFull = new EventWaitHandle(true, EventResetMode.ManualReset);

        readonly ConcurrentQueue<LogEntry> Entries
            = new ConcurrentQueue<LogEntry>();

        readonly int MaxBufferSize;
        readonly BufferExceededBehaviors BufferExceededBehavior;

        readonly ISimpleLogTarget Logger;
        private Task LogWrittingTask;
        public Func<LogEntry, bool> Condition => Logger.Condition;

        public BufferedLogger(ISimpleLogTarget logger, int maxBufferSize = BufferSize.Infinite, BufferExceededBehaviors bufferExceededBehavior = BufferExceededBehaviors.Lock)
        {
            MaxBufferSize = maxBufferSize;
            BufferExceededBehavior = bufferExceededBehavior;
            Logger = logger;
        }

        void StartLogWrittingTask(CancellationToken cancellationToken)
        {
            LogWrittingTask = Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!Entries.TryDequeue(out LogEntry entry))
                    {
                        //exit the loop if there are no more entries to log
                        LogWrittingTask = null;
                        break;
                    }
                    else
                    {
                        Logger.Log(entry);
                        QueueIsNotFull.Set();
                    }
                }
            }, cancellationToken);
        }

        public void Log(LogEntry entry)
        {
            if (MaxBufferSize > BufferSize.Infinite &&
                Entries.Count >= MaxBufferSize)
            {
                switch (BufferExceededBehavior)
                {
                    case BufferExceededBehaviors.Discard:
                        return; //discard new log item
                    case BufferExceededBehaviors.Lock:
                        while (Entries.Count >= MaxBufferSize)
                        {
                            //wait until the queue has space for the new item
                            QueueIsNotFull.WaitOne();
                        }
                        break;
                }
            }

            Entries.Enqueue(entry);

            if (LogWrittingTask?.IsCompleted??true)
                StartLogWrittingTask(new CancellationToken());

            //if queue exceeds maxbuffersize reset the waiter
            if (Entries.Count >= MaxBufferSize)
                QueueIsNotFull.Reset();
        }
    }
}
