using System;

namespace SimpleLog.ConsoleTarget
{
    public static class ConsoleTargetLoggingBuildOptionsExtensions
    {
        /// <summary>
        /// Adds a console target to the logging system
        /// </summary>
        /// <param name="options"></param>
        /// <param name="condition">Log conditions for this target</param>
        /// <param name="formatter">Message formatter for this target</param>
        /// <param name="output">Output buffer this target will write to</param>
        /// <param name="enableBuffering">Sets if buffering should be used with this target</param>
        /// <param name="maxBufferSize">Maximum number of elements in the buffer</param>
        /// <param name="bufferExceededBehavior">Sets the behavior of this target when the buffer is full and new entries are logged</param>
        public static LogBuilder AddConsoleTarget(
            this LogBuilder options,
            Func<LogEntry,bool> condition = null,
            Func<LogEntry, string> formatter = null,
            ConsoleOutputs output = ConsoleOutputs.StandardOutput,
            bool enableBuffering = true,
            int maxBufferSize = BufferSize.Infinite,
            BufferExceededBehaviors bufferExceededBehavior = BufferExceededBehaviors.Lock)
        {
            var console_logger = new ConsoleTarget(
                (m) => System.Console.WriteLine(m),
                (m) => System.Console.Error.WriteLine(m))
            {
                Formatter = formatter ?? ConsoleTarget.DEFAULT_FORMAT,
                OutputStream = output,
                Condition = condition
            };
            
            if (enableBuffering)
            {
                options.AddLogTarget(
                    new BufferedLogger(
                        console_logger, 
                        maxBufferSize, 
                        bufferExceededBehavior));
            }
            else
            {
                options.AddLogTarget(console_logger);
            }

            return options;
        }


    }
}
