using DSharpPlus.Entities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;

namespace InsanityBot.MessageServices.Embeds
{
	internal class DefaultEmbeds
	{
        public Dictionary<String, DiscordEmbedBuilder> Embeds { get; set; }

        public DefaultEmbeds()
        {
            StreamReader reader = new("./cache/embeds/default.json");
            Dictionary<String, EmbedData> protoEmbeds = JsonConvert.DeserializeObject<Dictionary<String, EmbedData>>(reader.ReadToEnd());

            Embeds = new();

            foreach(var v in protoEmbeds)
            {
                Embeds.Add(v.Key, new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(v.Value.Color),
                    Footer = new()
                    {
                        Text = v.Value.Footer ?? null
                    },
                    Title = v.Value.Title ?? null
                });
            }
        }
	}
}
