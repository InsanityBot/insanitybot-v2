using DSharpPlus.Entities;

using System;

namespace InsanityBot.Core.Formatters.Embeds
{
    public class FieldObjectBuilder
    {
        private String Title { get; set; } = null;
        private String Value { get; set; } = null;
        private Boolean Inline { get; set; } = false;

        public FieldObjectBuilder WithTitle(String title)
        {
            this.Title = title;
            return this;
        }

        public FieldObjectBuilder WithValue(String value)
        {
            this.Value = value;
            return this;
        }

        public FieldObjectBuilder WithInline(Boolean inline)
        {
            this.Inline = inline;
            return this;
        }

        public DiscordEmbedBuilder Build(DiscordEmbedBuilder embedBuilder)
        {
            embedBuilder.AddField(Title, Value, Inline);
            return embedBuilder;
        }
    }
}
