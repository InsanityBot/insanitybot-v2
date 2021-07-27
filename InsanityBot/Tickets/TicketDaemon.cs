﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using InsanityBot.Tickets.CustomCommands;
using InsanityBot.Tickets.Transcripts;
using InsanityBot.Utility.Config;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets
{
    public class TicketDaemon
    {
        internal Dictionary<Guid, DiscordTicket> Tickets { get; set; }
        internal Dictionary<Guid, DiscordTicketData> AdditionalData { get; set; }
        internal static TicketConfiguration Configuration { get; set; }
        internal CustomCommandHandler CommandHandler { get; set; }
        internal TicketCreator TicketCreator { get; set; }
        public TicketTranscriber Transcriber { get; set; }
        public static UInt32 TicketCount { get; private set; }
        public static String RandomTicketName

        {
            get
            {
                String validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                Char[] stringChars = new Char[8];
                Random random = new();

                for(int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = validChars[random.Next(validChars.Length)];
                }

                 return new String(stringChars);
            }
        }
        public List<TicketPreset> Presets { get; private set; }
        public TicketPreset DefaultPreset
        {
            get
            {
                return Presets.First(xm => xm.Id == "default");
            }
        }

        public TicketDaemon()
        {
            if(!File.Exists("./cache/tickets/presets/default.json"))
            {
                InsanityBot.Client.Logger.LogCritical(new EventId(2000, "TicketDaemon"), "Could not find default ticket preset, aborting...");
                Process.GetCurrentProcess().Kill();
            }

            if(!File.Exists("./cache/tickets/tickets.json"))
            {
                this.Tickets = new();
            }
            else
            {
                this.Tickets = JsonConvert.DeserializeObject<Dictionary<Guid, DiscordTicket>>(File.ReadAllText("./cache/tickets/tickets.json"));
            }

            if(!File.Exists("./cache/tickets/data.json"))
            {
                this.AdditionalData = new();
            }
            else
            {
                this.AdditionalData = JsonConvert.DeserializeObject<Dictionary<Guid, DiscordTicketData>>(
                    File.ReadAllText("./cache/tickets/data.json"));
            }

            if(!Directory.Exists("./cache/tickets/transcripts"))
            {
                Directory.CreateDirectory("./cache/tickets/transcripts");
            }

            if(!File.Exists("./cache/tickets/transcripts/index.json"))
            {
                FileStream stream = File.Create("./cache/tickets/transcripts/index.json");
                stream.Write(Encoding.UTF8.GetBytes("[]"));
                stream.Close();
            }

            Configuration = new TicketConfigurationManager().Deserialize("./config/ticket.json");

            Transcriber = new();
            Transcriber.RegisterTranscriber<HumanReadableTranscriber>();

            foreach(var v in Directory.GetFiles("./cache/tickets/presets", "*.json"))
            {
                StreamReader reader = new(v);
                Presets.Add(JsonConvert.DeserializeObject<TicketPreset>(reader.ReadToEnd()));
                reader.Close();
            }
        }
        public DiscordTicket GetDiscordTicket(UInt64 Id)
        {
            return (from v in this.Tickets.Values
                    where v.DiscordChannelId == Id
                    select v).ToList().First();
        }
        public DiscordTicketData GetTicketData(UInt64 Id)
        {
            return (from v in this.AdditionalData
                    where v.Key == this.GetDiscordTicket(Id).TicketGuid
                    select v).ToList().First().Value;
        }

        public async Task<Guid> CreateTicket(TicketPreset preset, CommandContext context, String topic)
        {
            TicketCount++;
            return await TicketCreator.CreateTicket(preset, context, topic);
        }

        public async Task DeleteTicket(DiscordTicket ticket)
        {
            StreamWriter writer = new("./cache/tickets/transcripts/index.json");

            writer.BaseStream.Position--;

            // manually write json to save the performance cost of deserializing and serializing
            await writer.WriteAsync($",{{\"readable\":\"./cache/tickets/transcripts/{ticket.DiscordChannelId}-readable.md\"," +
                $"\"minimal\":\"./cache/tickets/transcripts/{ticket.DiscordChannelId}-minimal.json\"}}");

            await Transcriber.Transcribe(ticket);

            foreach(var v in ticket.AddedUsers)
            {
                DiscordMember member = await InsanityBot.HomeGuild.GetMemberAsync(v);

                DiscordDmChannel dm = await member.CreateDmChannelAsync();

                DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder().WithFile("transcript.md", new FileStream(
                    $"./cache/tickets/transcripts/{ticket.DiscordChannelId}-readable.md", FileMode.Open));

                await dm.SendMessageAsync(messageBuilder);
            }

            await InsanityBot.HomeGuild.GetChannel(ticket.DiscordChannelId).DeleteAsync("Ticket closed");
        }

        public void SaveAll()
        {
            if(this.Tickets.Count > 0)
            {
                if(!File.Exists("./cache/tickets/tickets.json"))
                {
                    File.Create("./cache/tickets/tickets.json").Close();
                }

                StreamWriter writer = new("./cache/tickets/tickets.json");
                writer.Write(JsonConvert.SerializeObject(this.Tickets));
                writer.Close();
            }

            if(this.AdditionalData.Count > 0)
            {
                if(!File.Exists("./cache/tickets/data.json"))
                {
                    File.Create("./cache/tickets/data.json").Close();
                }

                StreamWriter writer = new("./cache/tickets/data.json");
                writer.Write(JsonConvert.SerializeObject(this.AdditionalData));
                writer.Close();
            }
        }
        public Task RouteCustomCommand(DiscordClient cl, MessageCreateEventArgs e)
        {
            String command = null;

            foreach(var v in Configuration["insanitybot.customcommands.prefices"] as JArray)
            {
                if(e.Message.Content.StartsWith(v.Value<String>()))
                {
                    command = e.Message.Content.Substring(v.Value<String>().Length);
                    break;
                }
                return Task.CompletedTask;
            }

            CommandContext context = cl.GetCommandsNext().CreateFakeContext(e.Author, e.Channel, e.Message.Content, "i.", null);
            CommandHandler.HandleCommand(context, command);

            return Task.CompletedTask;
        }
        internal Task<Guid> UpgradeProtoTicket(ProtoTicket ticket)
        {
            DiscordTicket upgrade = new()
            {
                Creator = ticket.Creator,
                DiscordChannelId = ticket.DiscordChannelId,
                Settings = ticket.Settings,
                TicketGuid = Guid.NewGuid()
            };

            Tickets.Add(upgrade.TicketGuid, upgrade);
            return Task.FromResult(upgrade.TicketGuid);
        }

        ~TicketDaemon()
        {
            this.SaveAll();
        }
    }
}