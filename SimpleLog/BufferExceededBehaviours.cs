using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLog
{
    /// <summary>
    /// Represents the behaviour of the Log method whenever the buffer has exceeded it's MaxBufferSize
    /// </summary>
    public enum BufferExceededBehaviors
    {
        /// <summary>
        /// Doesn't allow buffer to exceed MaxBufferSize by locking the Log method thread.
        /// </summary>
        Lock,
        /// <summary>
        /// Discard new log messages that would exceed MaxBufferSize
        /// </summary>
        Discard
    }
}
