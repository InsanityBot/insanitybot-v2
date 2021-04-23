using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Kyuu.Preprocessor
{
    public static class EventDirectiveProcessor
    {
        public static Task<Boolean> ProcessDirective(KyuuPreprocessorContext context)
        {
            switch(context.Instruction.Split(' ')[1])
            {
                case "on_command_sent":
                    foreach (var v in InsanityBot.Config.Prefixes)
                        if (context.Message.Content.StartsWith(v))
                            return Task.FromResult(true);
                    return Task.FromResult(false);
                case "on_message_sent":
                    if (context.Message.Content != null)
                        return Task.FromResult(true);
                    return Task.FromResult(false);
                case "on_reaction_added":
                    if (context.Message.Reactions.Count != 0)
                        return Task.FromResult(true);
                    return Task.FromResult(false);
            }
            return Task.FromResult(false);
        }
    }
}
