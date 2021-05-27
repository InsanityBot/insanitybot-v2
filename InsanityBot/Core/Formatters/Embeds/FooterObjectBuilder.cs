using DSharpPlus.Entities;

using System;

namespace InsanityBot.Core.Formatters.Embeds
{
    public class FooterObjectBuilder
    {
        private String Text { get; set; } = null;
        private String Url { get; set; } = null;

        public FooterObjectBuilder WithText(String text)
        {
            this.Text = text;
            return this;
        }

        public FooterObjectBuilder WithUrl(Uri url)
        {
            this.Url = url.ToString();
            return this;
        }

        public DiscordEmbedBuilder Build(DiscordEmbedBuilder embedBuilder)
        {
            embedBuilder.WithFooter(Text, Url);
            return embedBuilder;
        }
    }
}
