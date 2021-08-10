using DSharpPlus;

using InsanityBot.Utility.Config;

using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Linq;

namespace InsanityBot.Core.Logger
{
    internal class InsanityBotLogger : ILogger, ILogger<BaseDiscordClient>
    {
        private LoggerConfiguration Config { get; set; }
        private static readonly Object __lock = new();
        private StreamWriter logWriter;

        private StreamWriter LogWriter
        {
            get
            {
                if(logWriter == null)
                {
                    if((Boolean)InsanityBot.LoggerConfig.Configuration["LogToFile"])
                    {
                        if(!Directory.Exists("./logs"))
                        {
                            Directory.CreateDirectory("./logs");
                        }

                        try
                        {
                            File.Move("./logs/latest.txt", $"./logs/log-{DateTime.Now:yyyy-MM-dd-hh-mm-dd}.txt");
                        }
                        catch
                        {
                            // the file didnt exist, no worries
                        }

                        logWriter = new(File.Create("./logs/latest.txt"));
                        return logWriter;
                    }
                    else
                    {
                        throw new ArgumentException("File logging is disabled yet was called.");
                    }
                }
                return logWriter;
            }
        }

        public InsanityBotLogger(LoggerConfiguration config) => this.Config = config;

        public InsanityBotLogger(BaseDiscordClient client) => this.Config = InsanityBot.LoggerConfig;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public Boolean IsEnabled(LogLevel logLevel) => logLevel >= (LogLevel)Convert.ToInt32(this.Config.Configuration["LogLevel"]);

        public void LogConsole<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, String> formatter)
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
                Console.Write($"[{DateTimeOffset.Now.ToString((String)this.Config.Configuration["TimestampFormat"])}] ");

                switch(logLevel)
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
                if(exception != null)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        public void LogFile<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, String> formatter)
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
                LogWriter.Write($"[{DateTimeOffset.Now.ToString((String)this.Config.Configuration["TimestampFormat"])}] ");

                LogWriter.Write(logLevel switch
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

                LogWriter.Write($" [{eventId.Id}/{ename}] ");

                String message = formatter(state, exception);
                LogWriter.WriteLine(message);

                if(exception != null)
                {
                    LogWriter.WriteLine(exception);
                }

                LogWriter.Flush();
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, String> formatter)
        {
            LogConsole(logLevel, eventId, state, exception, formatter);

            if((Boolean)InsanityBot.LoggerConfig["LogToFile"])
            {
                LogFile(logLevel, eventId, state, exception, formatter);
            }
        }
    }
}
