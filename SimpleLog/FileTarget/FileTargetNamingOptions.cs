using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLog.FileTarget
{
    public struct FileTargetNamingOptions
    {
        /// <summary>
        /// File number
        /// </summary>
        public int Sequence;
        /// <summary>
        /// Log level of this file
        /// </summary>
        public LogLevel LogLevel;
    }
}
