namespace InsanityBot.Tickets.Transcripts;
using System.Threading.Tasks;

public interface ITranscriber
{
	public Task Transcribe(DiscordTicket ticket);
}