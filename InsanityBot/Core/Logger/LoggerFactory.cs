using System;
using System.Collections.Generic;

using DSharpPlus;

using Microsoft.Extensions.Logging;

namespace InsanityBot.Core.Logger
{
    public class LoggerFactory : ILoggerFactory
    {
        private List<ILoggerProvider> Providers { get; } = new();
        private Boolean __disposed = false;

        public ILogger CreateLogger(String categoryName)
        {
            if (this.__disposed)
            {
                throw new InvalidOperationException("This logger factory is already disposed.");
            }

            if (categoryName != typeof(BaseDiscordClient).FullName && categoryName != typeof(DiscordWebhookClient).FullName)
            {
                throw new ArgumentException($"This factory can only provide instances of loggers for {typeof(BaseDiscordClient).FullName} or {typeof(DiscordWebhookClient).FullName}.", nameof(categoryName));
            }

            return new InsanityBotLogger(InsanityBot.LoggerConfig);
        }

        public void AddProvider(ILoggerProvider provider) => this.Providers.Add(provider);

        public void Dispose()
        {
            if (this.__disposed)
            {
                return;
            }

            this.__disposed = true;

            foreach (ILoggerProvider provider in this.Providers)
            {
                provider.Dispose();
            }

            this.Providers.Clear();
        }
    }
}
