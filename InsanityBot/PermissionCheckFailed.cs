using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;

using InsanityBot.Core.Attributes;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        private static async Task PermissionFailed(CommandsNextExtension _, CommandErrorEventArgs e)
        {
            List<CheckBaseAttribute> failedChecks = (e.Exception as ChecksFailedException)?.FailedChecks.ToList()
                ?? new List<CheckBaseAttribute>();

            foreach(CheckBaseAttribute v in failedChecks)
            {
                if(v is RequirePermissionAttribute)
                {
                    await e.Context.Channel?.SendMessageAsync(Embeds["insanitybot.lacking_permission"]
                        .WithDescription(LanguageConfig["insanitybot.error.lacking_permission"])
                        .Build());
                    continue;
                }
                if(v is RequireAdminPermissionAttribute)
                {
                    await e.Context.Channel?.SendMessageAsync(Embeds["insanitybot.lacking_admin_permission"]
                        .WithDescription(LanguageConfig["insanitybot.error.lacking_admin_permission"])
                        .Build());
                    continue;
                }
            }
        }
    }
}
