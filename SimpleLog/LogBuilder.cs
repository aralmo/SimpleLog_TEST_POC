using SimpleLog.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLog
{
    public class LogBuilder
    {
        internal LogLevel[] EnabledLevels { get; set; } 
            = null;
        internal List<SimpleLogTarget> LogTargets { get; set; } 
            = new List<SimpleLogTarget>();

        public void AddLogTarget(SimpleLogTarget logTarget)
        {
            LogTargets.Add(logTarget);
        }

        public ILogger Build()
        {
            return new SimpleLogger(this);
        }
    }
}
