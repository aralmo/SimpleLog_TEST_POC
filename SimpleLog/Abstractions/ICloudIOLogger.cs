using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLog.Abstractions
{
    public interface SimpleLogTarget
    {
        void Log(LogEntry entry);
        Func<LogEntry, bool> Condition { get; }
    }
}
