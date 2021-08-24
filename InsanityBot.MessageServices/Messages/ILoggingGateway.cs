﻿using System;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace InsanityBot.MessageServices.Messages
{
    public interface ILoggingGateway
    {
        public Task SendMessage(DiscordEmbed embed);
        public Task SendMessage(String content);
        public Task SendMessage(DiscordMessageBuilder builder);

        public static ILoggingGateway Empty
        {
            get;
        }
    }
}
