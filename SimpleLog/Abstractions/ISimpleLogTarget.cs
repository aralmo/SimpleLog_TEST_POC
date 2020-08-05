﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLog.Abstractions
{
    public interface ISimpleLogTarget
    {
        void Log(LogEntry entry);
        Func<LogEntry, bool> Condition { get; }
    }
}