using System;
using System.Threading.Tasks;

using InsanityBot.Tickets.Kyuu.Preprocessor;

namespace InsanityBot.Tickets.Kyuu.Tasks
{
    public delegate Task<Boolean> ConditionEvaluationTask(String condition);
    public delegate Task FunctionEvaluationTask(String function, String arguments);

    namespace Preprocessor
    {
        public delegate Task<Boolean> EventPreprocessorTask(String @event, KyuuPreprocessorContext context);
        public delegate Task<Boolean> CategoryPreprocessorTask(UInt64 category, KyuuPreprocessorContext context);
        public delegate Task<Boolean> MessagePreprocessorTask(String message, KyuuPreprocessorContext context);
        public delegate Task<Boolean> CommandPreprocessorTask(String command, KyuuPreprocessorContext context);
        public delegate Task<Boolean> PreprocessorTask(KyuuPreprocessorContext context);
    }
}