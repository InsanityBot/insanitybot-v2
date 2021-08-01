using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Transcripts
{
    public class TicketTranscriber
    {
        private readonly List<ITranscriber> Transcribers = new();

        public void RegisterTranscriber<T>()
            where T : ITranscriber, new() => this.Transcribers.Add(new T());

        public async Task Transcribe(DiscordTicket ticket)
        {
            foreach(ITranscriber v in this.Transcribers)
            {
                await v.Transcribe(ticket);
            }
        }
    }
}
