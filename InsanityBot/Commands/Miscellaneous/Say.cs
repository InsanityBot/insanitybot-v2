using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using InsanityBot.Core.Attributes;

namespace InsanityBot.Commands.Miscellaneous
{
    public class Say : BaseCommandModule
    {
        [Command("say")]
        [RequirePermission("insanitybot.miscellaneous.say")]
        public async Task SayCommand(CommandContext ctx,
            [RemainingText]
            String text)
        {
            if((ctx.Message.MentionedRoles.Count != 0 || ctx.Message.Content.Contains("@everyone") || ctx.Message.Content.Contains("@here"))
                && ((ctx.Member?.PermissionsIn(ctx.Channel) & DSharpPlus.Permissions.MentionEveryone) != DSharpPlus.Permissions.MentionEveryone)
                && InsanityBot.Config.Value<Boolean>("insanitybot.miscellaneous.block_say_role_pings"))
            {
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            if(ctx.Message?.MentionedUsers.Count != 0
                && ((ctx.Member?.PermissionsIn(ctx.Channel) & DSharpPlus.Permissions.MentionEveryone) != DSharpPlus.Permissions.MentionEveryone)
                && InsanityBot.Config.Value<Boolean>("insanitybot.miscellaneous.block_say_user_pings"))
            {
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            _ = ctx.Message?.DeleteAsync();
            _ = ctx.Channel?.SendMessageAsync(text);
        }
    }
}
