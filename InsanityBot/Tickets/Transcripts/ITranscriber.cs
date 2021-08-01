using System.Threading.Tasks;

namespace InsanityBot.Tickets.Transcripts
{
    public interface ITranscriber
    {
        public Task Transcribe(DiscordTicket ticket);
    }
}
