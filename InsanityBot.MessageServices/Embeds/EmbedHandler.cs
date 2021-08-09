using DSharpPlus;
using DSharpPlus.Entities;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InsanityBot.MessageServices.Embeds
{
    public class EmbedHandler
    {
        private DefaultEmbeds _defaultEmbeds;
        private Dictionary<String, DiscordEmbedBuilder> _embeds = new();

        public void Initialize(ILogger<BaseDiscordClient> logger)
        {
            _defaultEmbeds = new();
            _embeds = _defaultEmbeds.Embeds;
            LoadPatches(logger);
        }

        public void LoadPatches(ILogger<BaseDiscordClient> logger)
        {
            if(!File.Exists("./config/embeds.json"))
            {
                logger.LogInformation(new EventId(1100, "EmbedLoader"), "No embed override file discovered, using default embeds");
                return;
            }
            else
            {
                logger.LogInformation(new EventId(1000, "EmbedLoader"), "Embed override file discovered, loading overrides...");
            }

            StreamReader reader = new("./config/embeds.json");
            JObject overrides = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            reader.Close();

            if(overrides.SelectToken("insanitybot") == null)
            {
                return;
            }

            List<String> keys = (from e in _defaultEmbeds.Embeds
                                 select e.Key).ToList();

            foreach(var v in keys)
            {
                JToken j = overrides.SelectToken(v);

                if(j == null)
                {
                    continue;
                }

                if(j["color"] != null && j["color"].Type == JTokenType.String)
                {
                    _embeds[v].Color = new DiscordColor(j["color"].Value<String>());
                }

                if(j["footer"] != null && j["footer"].Type == JTokenType.String)
                {
                    _embeds[v].Footer = new() { Text = j["footer"].Value<String>() };
                }

                if(j["title"] != null && j["title"].Type == JTokenType.String)
                {
                    _embeds[v].Title = j["title"].Value<String>();
                }
            }
        }

        public DiscordEmbedBuilder this[String s]
        {
            get
            {
                if(s.StartsWith("default "))
                {
                    return _defaultEmbeds.Embeds[s[7..]];
                }
                return _embeds[s];
            }
        }
    }
}
