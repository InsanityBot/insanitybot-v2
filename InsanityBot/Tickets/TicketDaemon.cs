namespace InsanityBot.Tickets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;

using global::InsanityBot.Tickets.Closure;
using global::InsanityBot.Tickets.CustomCommands;
using global::InsanityBot.Tickets.Transcripts;
using global::InsanityBot.Utility.Config;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TicketDaemon
{
	internal ConcurrentDictionary<Guid, DiscordTicket> Tickets { get; set; }
	internal Dictionary<Guid, DiscordTicketData> AdditionalData { get; set; }
	internal static TicketConfiguration Configuration { get; set; }
	internal CustomCommandHandler CommandHandler { get; set; }
	internal TicketCreator TicketCreator { get; set; }
	public TicketTranscriber Transcriber { get; set; }
	public UInt32 TicketCount { get; internal set; }
	public static UInt32 StaticTicketCount => InsanityBot.TicketDaemon.TicketCount;
	public static String RandomTicketName

	{
		get
		{
			String validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			Char[] stringChars = new Char[8];
			Random random = new();

			for(Int32 i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = validChars[random.Next(validChars.Length)];
			}

			return new String(stringChars);
		}
	}
	public List<TicketPreset> Presets { get; private set; }
	public TicketPreset DefaultPreset => this.Presets.First(xm => xm.Id == "default");
	public ClosingQueue ClosingQueue { get; set; }

	public TicketDaemon()
	{
		Configuration = new ConfigurationManager().Deserialize<TicketConfiguration>("./config/ticket.json");

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
			this.Tickets = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, DiscordTicket>>(File.ReadAllText("./cache/tickets/tickets.json"));
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

		if(!File.Exists("./cache/tickets/closequeue.json"))
		{
			File.Create("./cache/tickets/closequeue.json").Close();
		}
		else
		{
			this.ClosingQueue = JsonConvert.DeserializeObject<ClosingQueue>(
				File.ReadAllText("./cache/tickets/closequeue.json"));
		}

		if(this.ClosingQueue == null) // file was null
		{
			this.ClosingQueue = new();
		}

		this.ClosingQueue.handler = new();

		_ = Task.Run(() =>
		{
			this.ClosingQueue.handler.Start();
		});

		this.Transcriber = new();
		this.Transcriber.RegisterTranscriber<HumanReadableTranscriber>();

		this.Presets = new();

		this.CommandHandler = new();

		this.TicketCreator = new();

		foreach(String v in Directory.GetFiles("./cache/tickets/presets", "*.json"))
		{
			StreamReader reader = new(v);
			this.Presets.Add(JsonConvert.DeserializeObject<TicketPreset>(reader.ReadToEnd()));
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

	public Guid CreateTicket(TicketPreset preset, CommandContext context, String topic, out DiscordChannel channel)
	{
		this.TicketCount++;
		return this.TicketCreator.CreateTicket(preset, context, topic, out channel);
	}

	public async Task DeleteTicket(DiscordTicket ticket)
	{
		StreamWriter writer = new(new FileStream("./cache/tickets/transcripts/index.json", FileMode.Append));

		// manually write json to save the performance cost of deserializing and serializing
		await writer.WriteAsync($",{{\"readable\":\"./cache/tickets/transcripts/{ticket.DiscordChannelId}-readable.md\"," +
			$"\"minimal\":\"./cache/tickets/transcripts/{ticket.DiscordChannelId}-minimal.json\"}}");

		writer.Close();

		if(Configuration.Value<Boolean>("insanitybot.tickets.transcripts.enable"))
		{
			await this.Transcriber.Transcribe(ticket);

			FileStream transcript = new($"./cache/tickets/transcripts/{ticket.DiscordChannelId}-readable.md", FileMode.Open);
			transcript.Position = 0;

			if(!(ticket.AddedUsers == null))
			{
				foreach(UInt64 v in ticket.AddedUsers)
				{
					DiscordMember member = await InsanityBot.HomeGuild.GetMemberAsync(v);

					if(member.IsBot)
					{
						continue;
					}

					DiscordDmChannel dm = await member.CreateDmChannelAsync();

					transcript.Position = 0;
					DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder().WithFile("transcript.md", transcript);

					try
					{
						_ = dm?.SendMessageAsync(messageBuilder);
					}
					catch(UnauthorizedException)
					{
						// they have their dms closed. nothing to worry about.
					}
					catch(Exception)
					{
						throw;
					}
				}
			}

			DiscordMember owner = await InsanityBot.HomeGuild.GetMemberAsync(ticket.Creator);
			DiscordDmChannel ownerDm = await owner.CreateDmChannelAsync();

			transcript.Position = 0;
			DiscordMessageBuilder ownerMessageBuilder = new DiscordMessageBuilder().WithFile("transcript.md", transcript);

			try
			{
				_ = ownerDm?.SendMessageAsync(ownerMessageBuilder);
			}
			catch(UnauthorizedException)
			{
				// they have their dms closed. nothing to worry about.   
			}
			catch(Exception)
			{
				throw;
			}

			transcript.Close();
		}

		this.Tickets.Remove(ticket.TicketGuid, out ticket);

		DiscordChannel ticketChannel = InsanityBot.HomeGuild.GetChannel(ticket.DiscordChannelId);

		DiscordEmbedBuilder logEmbed = InsanityBot.Embeds["insanitybot.ticketlog.close"]
			.AddField("Channel", ticketChannel.Name);

		await InsanityBot.MessageLogger.LogMessage(logEmbed.Build(), InsanityBot.CommandsExtension.CreateFakeContext(InsanityBot.Client.CurrentUser,
			ticketChannel, "i.close", "i.", null));

		await ticketChannel.DeleteAsync("Ticket closed");
	}

	public void RemoveTicket(UInt64 id)
	{
		foreach(KeyValuePair<Guid, DiscordTicket> v in from x in this.Tickets
													   where x.Value.DiscordChannelId == id
													   select x)
		{
			DiscordTicket t = v.Value;
			this.Tickets.Remove(v.Key, out t);
		}
	}

	public void SaveAll()
	{

		if(!File.Exists("./cache/tickets/tickets.json"))
		{
			File.Create("./cache/tickets/tickets.json").Close();
		}

		StreamWriter writer = new("./cache/tickets/tickets.json");
		writer.Write(JsonConvert.SerializeObject(this.Tickets));
		writer.Close();


		if(!File.Exists("./cache/tickets/data.json"))
		{
			File.Create("./cache/tickets/data.json").Close();
		}

		writer = new("./cache/tickets/data.json");
		writer.Write(JsonConvert.SerializeObject(this.AdditionalData));
		writer.Close();


		if(!File.Exists("./cache/tickets/closequeue.json"))
		{
			File.Create("./cache/tickets/closequeue.json").Close();
		}
		using(writer = new("./cache/tickets/closequeue.json"))
		{
			writer.WriteLine(JsonConvert.SerializeObject(this.ClosingQueue));
			writer.Close();
		}
	}
	public Task RouteCustomCommand(DiscordClient cl, MessageCreateEventArgs e)
	{
		String command = null;

		foreach(JToken v in Configuration["insanitybot.customcommands.prefices"] as JArray)
		{
			if(e.Message.Content.StartsWith(v.Value<String>()))
			{
				command = e.Message.Content[v.Value<String>().Length..];
				break;
			}
			return Task.CompletedTask;
		}

		CommandContext context = cl.GetCommandsNext().CreateFakeContext(e.Author, e.Channel, e.Message.Content, "i.", null);
		this.CommandHandler.HandleCommand(context, command);

		return Task.CompletedTask;
	}
	internal Guid UpgradeProtoTicket(ProtoTicket ticket)
	{
		DiscordTicket upgrade = new()
		{
			Creator = ticket.Creator,
			DiscordChannelId = ticket.DiscordChannelId,
			Settings = ticket.Settings,
			TicketGuid = Guid.NewGuid(),
			AddedUsers = new List<UInt64>(),
			Staff = new List<UInt64>()
		};

		this.Tickets.TryAdd(upgrade.TicketGuid, upgrade);
		return upgrade.TicketGuid;
	}

	~TicketDaemon()
	{
		this.SaveAll();

		TicketDaemonState state = new();
		state.SaveDaemonState(this);
	}
}