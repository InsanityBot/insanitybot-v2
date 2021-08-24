using System;

using DSharpPlus.Entities;

using Newtonsoft.Json;

namespace InsanityBot.MessageServices.Embeds
{
    public struct EmbedData
    {
        [JsonProperty("color", Required = Required.Always)]
        public String Color { get; set; }

        [JsonProperty("footer", Required = Required.DisallowNull)]
        public String Footer { get; set; }

        [JsonProperty("title", Required = Required.DisallowNull)]
        public String Title { get; set; }

        public DiscordEmbedBuilder ToEmbedBuilder()
        {
            return new()
            {
                Color = new DiscordColor(this.Color),
                Footer = new()
                {
                    Text = this.Footer
                },
                Title = this.Title
            };
        }
    }
}
