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
        internal List<ISimpleLogTarget> LogTargets { get; set; } 
            = new List<ISimpleLogTarget>();

        public void AddLogTarget(ISimpleLogTarget logTarget)
        {
            LogTargets.Add(logTarget);
        }

        public ILogger Build()
        {
            return new SimpleLogger(this);
        }
    }
}
