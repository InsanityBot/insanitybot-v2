using DSharpPlus.Entities;

using System;
using System.IO;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Transcripts
{
    public class HumanReadableTranscriber : ITranscriber
    {
        public async Task Transcribe(DiscordTicket ticket)
        {
            StreamWriter writer = new(File.Create($"./cache/tickets/transcripts/{ticket.DiscordChannelId}-readable.md"));

            DiscordChannel channel = InsanityBot.HomeGuild.GetChannel(ticket.DiscordChannelId);
            DiscordMember creator = await InsanityBot.HomeGuild.GetMemberAsync(ticket.Creator);

            await writer.WriteAsync($"# Ticket Transcript of ticket {channel.Name}\n\n" +
                $"Transcript created at {DateTimeOffset.Now}.\n\n" +
                $"## Channel information:\n\n" +
                $"- Created At: {channel.CreationTimestamp}\n" +
                $"- Created By: {creator.Username}#{creator.Discriminator}\n\n");

            await writer.WriteAsync($"## Transcript:\n\n");

            foreach(DiscordMessage v in await channel.GetMessagesAfterAsync(0, 10000))
            {
                await writer.WriteAsync($"{v.Author.Username}#{v.Author.Discriminator}: {v.Content}\n\n");
            }

            writer.Close();
        }
    }
}
