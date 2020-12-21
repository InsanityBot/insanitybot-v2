using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation
{
    public partial class Mute
    {
        [Command("unmute")]
        public async Task UnmuteCommand(CommandContext ctx,
            DiscordMember member,
            String arguments)
        {
            if (arguments.StartsWith('-'))
            {
                await ParseUnmuteCommand(ctx, member, arguments);
                return;
            }
            InsanityBot.Client.Logger.LogWarning(new EventId(1133, "ArgumentParser"),
                "Unmute command was called with invalid arguments, running default arguments");
            await ExecuteUnmuteCommand(ctx, member, false, false);
        }

        private async Task ParseUnmuteCommand(CommandContext ctx,
            DiscordMember member,
            String arguments)
        {
            String cmdArguments = arguments;
            try
            {
                if (!arguments.Contains("-r") && !arguments.Contains("--reason"))
                    cmdArguments += " --reason void"; //we dont need the reason but its required by the protocol

                await Parser.Default.ParseArguments<UnmuteOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await ExecuteUnmuteCommand(ctx, member, o.Silent, o.DmMember);
                    });
            }
            catch (Exception e)
            {
                DiscordEmbedBuilder failed = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.failure"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.RespondAsync(embed: failed.Build());
            }
        }

        private async Task ExecuteUnmuteCommand(CommandContext ctx,
            DiscordMember member,
            Boolean silent,
            Boolean dmMember)
        {

        }
    }

    public class UnmuteOptions : ModerationOptionBase
    {

    }
}
