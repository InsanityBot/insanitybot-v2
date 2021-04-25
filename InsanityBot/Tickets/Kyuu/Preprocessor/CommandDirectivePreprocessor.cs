using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Kyuu.Preprocessor
{
    public static class CommandDirectivePreprocessor
    {
        public static Task<Boolean> ProcessDirective(KyuuPreprocessorContext context)
        {
            String command = context.Message.Content.Split(' ')[0];
            foreach(var v in InsanityBot.Config.Prefixes)
            {
                if (command.StartsWith(v))
                    command = command.Remove(0, v.Length);
            }

            if (context.Instruction.Split(' ')[1] == command)
                return Task.FromResult(true);
            return Task.FromResult(false);
        }
    }
}
