using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;

using Microsoft.Extensions.Logging;

namespace InsanityBot.Core.Logger
{
    class LoggerProvider : ILoggerProvider
    {
        public LoggerConfiguration Configuration { get; }
        private Boolean __disposed = false;

        public LoggerProvider(BaseDiscordClient client) : this(InsanityBot.LoggerConfig)
        {

        }

        public LoggerProvider(DiscordWebhookClient client) : this(InsanityBot.LoggerConfig)
        {

        }

        public LoggerProvider(LoggerConfiguration config)
        {
            Configuration = config;
        }

        public ILogger CreateLogger(String categoryName)
        {
            if (this.__disposed)
                throw new InvalidOperationException("This InsanityBot Logger Provider is already disposed.");

            if (categoryName != typeof(BaseDiscordClient).FullName && categoryName != typeof(DiscordWebhookClient).FullName)
                throw new ArgumentException($"This provider can only provide instances of loggers for {typeof(BaseDiscordClient).FullName} or {typeof(DiscordWebhookClient).FullName}.", nameof(categoryName));

            return new InsanityBotLogger(Configuration);
        }

        public void Dispose()
        {
            this.__disposed = true;
        }
    }
}
