using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Kyuu.Preprocessor
{
    public static class CategoryDirectivePreprocessor
    {
        public static Task<Boolean> ProcessDirective(KyuuPreprocessorContext context)
        {
            UInt64 categoryCondition = Convert.ToUInt64(context.Instruction.Split(' ')[1]);
            if (context.Message.Channel.ParentId == categoryCondition)
                return Task.FromResult(true);
            return Task.FromResult(false);
        }
    }
}
