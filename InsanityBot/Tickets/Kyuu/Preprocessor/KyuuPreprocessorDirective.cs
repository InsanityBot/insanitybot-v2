using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Tickets.Kyuu.Tasks.Preprocessor;

namespace InsanityBot.Tickets.Kyuu.Preprocessor
{
    public class KyuuPreprocessorDirective
    {
        public String Identifier { get; set; }
        public Boolean Parameters { get; set; }
        public PreprocessorTask Task { get; set; }

        public static readonly KyuuPreprocessorDirective EventDirective = new()
        {
            Identifier = "event",
            Parameters = true,
            Task = EventDirectiveProcessor.ProcessDirective
        };

        public static readonly KyuuPreprocessorDirective CategoryDirective = new()
        {
            Identifier = "category",
            Parameters = true
        };

        public static readonly KyuuPreprocessorDirective CommandDirective = new()
        {
            Identifier = "command",
            Parameters = true
        };

        public static readonly KyuuPreprocessorDirective MessageDirective = new()
        {
            Identifier = "message",
            Parameters = true
        };

        public static readonly KyuuPreprocessorDirective AdminDirective = new()
        {
            Identifier = "admin",
            Parameters = false
        };
    }
}
