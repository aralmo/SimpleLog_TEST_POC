using System;

namespace SimpleLog.Abstractions
{
    public interface ISimpleLogTarget
    {
        void Log(LogEntry entry);
        Func<LogEntry, bool> Condition { get; }
    }
}
