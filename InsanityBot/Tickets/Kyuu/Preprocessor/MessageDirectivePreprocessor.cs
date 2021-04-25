using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Kyuu.Preprocessor
{
    public static class MessageDirectivePreprocessor
    {
        public static Task<Boolean> ProcessDirective(KyuuPreprocessorContext context)
        {
            if (context.Instruction.Split(' ')[1] == context.Message.Content)
                return Task.FromResult(true);
            return Task.FromResult(false);
        }
    }
}
