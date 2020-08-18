using System;
using System.Collections.Generic;
using System.Text;

using DSharpPlus;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static DiscordConfiguration ClientConfiguration = new DiscordConfiguration
        {
            AutoReconnect = true,
            Token = MainConfig.Token,
            TokenType = TokenType.Bot
        }
        public static DiscordClient Client = new DiscordClient();
    }
}
