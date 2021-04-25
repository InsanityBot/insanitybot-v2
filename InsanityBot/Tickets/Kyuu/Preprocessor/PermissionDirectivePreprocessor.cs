using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

namespace InsanityBot.Tickets.Kyuu.Preprocessor
{
    public static class PermissionDirectivePreprocessor
    {
        public static Task<Boolean> ProcessDirective(KyuuPreprocessorContext context)
        {
            if ((context.Message.Author as DiscordMember).HasPermission(context.Instruction.Split(' ')[1]))
                return Task.FromResult(true);
            return Task.FromResult(false);
        }
    }
}
