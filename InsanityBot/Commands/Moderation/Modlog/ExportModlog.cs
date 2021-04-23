using System;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation.Modlog
{
    public class ExportModlog : BaseCommandModule
    {
        [Command("exportmodlog")]
        public async Task ExportModlogCommand(CommandContext ctx,
            DiscordMember member,
            Boolean dmFile = false)
        {
            try
            {
                if (!ctx.Member.HasPermission("insanitybot.moderation.export_modlog"))
                {
                    await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                    return;
                }

                DiscordChannel exportChannel;

                if (!dmFile)
                {
                    exportChannel = ctx.Channel;
                }
                else
                {
                    exportChannel = await ctx.Member.CreateDmChannelAsync();
                }

                if (!File.Exists($"./data/{member.Id}/modlog.json"))
                {
                    await ctx.Channel.SendMessageAsync(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.export_modlog.no_modlog"],
                        ctx, member));
                    return;
                }

                Stream stream = new FileStream($"./data/{member.Id}/modlog.json", FileMode.Open);

                DiscordMessageBuilder messageBuilder = new();
                messageBuilder.WithFile("modlog_export.json", stream);

                await exportChannel.SendMessageAsync(messageBuilder);
            }
            catch (Exception e)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1181, "ExportModlog"), $"{e}: {e.Message}\n{e.StackTrace}");
            }
        }

        [Command("exportmodlog")]
        public async Task ExportModlogCommand(CommandContext ctx,
            Boolean dmFile = false) => await ExportModlogCommand(ctx, ctx.Member, dmFile);
    }
}
