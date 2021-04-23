using System;
using System.Linq;

using DSharpPlus;

using Microsoft.Extensions.Logging;

namespace InsanityBot.Core.Logger
{
    internal class InsanityBotLogger : ILogger, ILogger<BaseDiscordClient>
    {
        private LoggerConfiguration Config { get; set; }
        private static readonly Object __lock = new();

        public InsanityBotLogger(LoggerConfiguration config) => this.Config = config;

        public InsanityBotLogger(BaseDiscordClient client) => Config = InsanityBot.LoggerConfig;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public Boolean IsEnabled(LogLevel logLevel)
            => logLevel >= (LogLevel)Convert.ToInt32(this.Config.Configuration["LogLevel"]);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, String> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            if (this.Config.EventExclusions.Contains(eventId.Name))
            {
                return;
            }

            if (this.Config.EventIdExclusions.Contains(eventId.Id))
            {
                return;
            }

            lock (__lock)
            {
                String ename = eventId.Name;
                ename = ename?.Length > 12 ? ename?.Substring(0, 12) : ename;
                Console.Write($"[{DateTimeOffset.Now.ToString((String)this.Config.Configuration["TimestampFormat"])}] ");

                switch (logLevel)
                {
                    case LogLevel.Trace:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;

                    case LogLevel.Information:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;

                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;

                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case LogLevel.Critical:
                        Console.BackgroundColor = ConsoleColor.Red;
                        break;
                }
                Console.Write(logLevel switch
                {
                    LogLevel.Trace => "[Trace]",
                    LogLevel.Debug => "[Debug]",
                    LogLevel.Information => "[Info]",
                    LogLevel.Warning => "[Warn]",
                    LogLevel.Error => "[Error]",
                    LogLevel.Critical => "[Fatal]",
                    LogLevel.None => "[None]",
                    _ => "[?????] "
                });
                Console.ResetColor();

                Console.Write($" [{eventId.Id}/{ename}] ");

                String message = formatter(state, exception);
                Console.WriteLine(message);
                if (exception != null)
                {
                    Console.WriteLine(exception);
                }
            }
        }
    }
}
