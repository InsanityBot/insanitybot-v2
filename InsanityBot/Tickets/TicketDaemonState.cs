namespace InsanityBot.Tickets;
using System;
using System.IO;

using Newtonsoft.Json;

internal struct TicketDaemonState
{
	public UInt32 TicketCount { get; set; }

	public void RestoreDaemonState(ref TicketDaemon daemon)
	{
		if(!Directory.Exists("./cache/tickets"))
		{
			Directory.CreateDirectory("./cache/tickets");
		}

		if(!File.Exists("./cache/tickets/state.json"))
		{
			File.Create("./cache/tickets/state.json").Close();
			daemon.TicketCount = 0;
			return;
		}

		StreamReader reader = new("./cache/tickets/state.json");
		this = JsonConvert.DeserializeObject<TicketDaemonState>(reader.ReadToEnd());
		reader.Close();

		daemon.TicketCount = this.TicketCount;
	}

	public void SaveDaemonState(TicketDaemon daemon)
	{
		this.TicketCount = daemon.TicketCount;

		StreamWriter writer = new("./cache/tickets/state.json");
		writer.Write(JsonConvert.SerializeObject(this));
		writer.Close();
	}
}