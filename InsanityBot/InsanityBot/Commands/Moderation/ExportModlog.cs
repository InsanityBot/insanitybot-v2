using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

namespace InsanityBot.Commands.Moderation
{
    public class ExportModlog : BaseCommandModule
    {
        [Command("exportmodlog")]
        public async Task ExportModlogCommand(CommandContext ctx, 
            DiscordMember member,
            Boolean dmFile = false)
        {
            if(!member.HasPermission("insanitybot.moderation.export_modlog"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            DiscordChannel exportChannel;

            if (!dmFile)
                exportChannel = ctx.Channel;
            else
                exportChannel = await ctx.Member.CreateDmChannelAsync();

            await exportChannel.SendFileAsync($"./data/{member.Id}/modlog.json");
        }
    }
}
