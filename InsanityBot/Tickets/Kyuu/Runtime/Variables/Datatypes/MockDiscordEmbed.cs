using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace InsanityBot.Tickets.Kyuu.Runtime.Variables.Datatypes
{
    public struct MockDiscordEmbed
    {
        public Int32 Color { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public MockDiscordEmbedField[] Fields { get; set; }
        public String Footer { get; set; }

        public static explicit operator DiscordEmbed(MockDiscordEmbed e)
        {
            DiscordEmbedBuilder builder = new()
            {
                Color = new(e.Color),
                Title = e.Title,
                Description = e.Description,
                Footer = new()
                {
                    Text = e.Footer
                }
            };

            foreach(var v in e.Fields)
            {
                builder.AddField(v.Name, v.Value, v.Inline);
            }

            return builder.Build();
        }
    }
}
