using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleLog.ConsoleTarget;

namespace SimpleLog
{
    /// <summary>
    /// DependencyInjection extensions for the SimpleLog framework
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Sets the enabled log levels for the logging framework.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="minLevel"></param>
        /// <param name="maxLevel"></param>
        public static void EnableLevels(this LogBuilder options, LogLevel minLevel = LogLevel.Information, LogLevel maxLevel = LogLevel.Critical)
        {
            options.EnabledLevels = ClampLogLevels(minLevel, maxLevel).ToArray();
        }
        private static IEnumerable<LogLevel> ClampLogLevels(LogLevel minLevel, LogLevel maxLevel)
        {
            foreach (int v in Enum.GetValues(typeof(LogLevel)))
                if (v >= (int)minLevel && v <= (int)maxLevel)
                    yield return (LogLevel)v;
        }

        /// <summary>
        /// Sets the enabled log levels for the logging framework.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="enabledLevels"></param>
        public static void EnableLevels(this LogBuilder options, params LogLevel[] enabledLevels)
        {
            options.EnabledLevels = enabledLevels;
        }

        /// <summary>
        /// Enables SimpleLog framework
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddSimpleLog(this IServiceCollection services)
        {
            //Logging to console by default
            var options = new LogBuilder();
            options.AddConsoleTarget();

            services.AddSingleton(options);
            services.AddScoped<SimpleLogger>();
        }

        /// <summary>
        /// Enables SimpleLog framework
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddSimpleLog(this IServiceCollection services, Action<LogBuilder> configuration)
        {
            var options = new LogBuilder();
            configuration(options);

            services.AddSingleton(options);

            //If not targets are added during configuration
            //add a default console target
            if (!options.LogTargets.Any())
                options.AddConsoleTarget();

            services.AddTransient<ILogger, SimpleLogger>();
        }

    }
}
