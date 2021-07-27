using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Transcripts
{
    public interface ITranscriber
    {
        public Task Transcribe(DiscordTicket ticket);
    }
}
