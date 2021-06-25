using DSharpPlus;

using Microsoft.Extensions.Logging;

using System;
using System.Linq;

namespace InsanityBot.Core.Logger
{
    internal class InsanityBotLogger : ILogger, ILogger<BaseDiscordClient>
    {
        private LoggerConfiguration Config { get; set; }
        private static readonly Object __lock = new();

        public InsanityBotLogger(LoggerConfiguration config) => this.Config = config;

        public InsanityBotLogger(BaseDiscordClient client) => this.Config = InsanityBot.LoggerConfig;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public Boolean IsEnabled(LogLevel logLevel) => logLevel >= (LogLevel)Convert.ToInt32(this.Config.Configuration["LogLevel"]);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, String> formatter)
        {
            if(!this.IsEnabled(logLevel))
            {
                return;
            }

            if(this.Config.EventExclusions.Contains(eventId.Name))
            {
                return;
            }

            if(this.Config.EventIdExclusions.Contains(eventId.Id))
            {
                return;
            }

            lock(__lock)
            {
                String ename = eventId.Name;
                ename = ename?.Length > 12 ? ename?.Substring(0, 12) : ename;
                System.Console.Write($"[{DateTimeOffset.Now.ToString((String)this.Config.Configuration["TimestampFormat"])}] ");

                switch(logLevel)
                {
                    case LogLevel.Trace:
                        System.Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                    case LogLevel.Debug:
                        System.Console.ForegroundColor = ConsoleColor.Green;
                        break;

                    case LogLevel.Information:
                        System.Console.ForegroundColor = ConsoleColor.Magenta;
                        break;

                    case LogLevel.Warning:
                        System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;

                    case LogLevel.Error:
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case LogLevel.Critical:
                        System.Console.BackgroundColor = ConsoleColor.Red;
                        break;
                }
                System.Console.Write(logLevel switch
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
                System.Console.ResetColor();

                System.Console.Write($" [{eventId.Id}/{ename}] ");

                String message = formatter(state, exception);
                System.Console.WriteLine(message);
                if(exception != null)
                {
                    System.Console.WriteLine(exception);
                }
            }
        }
    }
}
