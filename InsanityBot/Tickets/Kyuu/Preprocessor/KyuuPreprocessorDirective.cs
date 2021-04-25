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
            Task = EventDirectivePreprocessor.ProcessDirective
        };

        public static readonly KyuuPreprocessorDirective CategoryDirective = new()
        {
            Identifier = "category",
            Parameters = true,
            Task = CategoryDirectivePreprocessor.ProcessDirective
        };

        public static readonly KyuuPreprocessorDirective CommandDirective = new()
        {
            Identifier = "command",
            Parameters = true,
            Task = CommandDirectivePreprocessor.ProcessDirective
        };

        public static readonly KyuuPreprocessorDirective MessageDirective = new()
        {
            Identifier = "message",
            Parameters = true,
            Task = MessageDirectivePreprocessor.ProcessDirective
        };

        public static readonly KyuuPreprocessorDirective PermissionDirective = new()
        {
            Identifier = "permission",
            Parameters = true,
            Task = PermissionDirectivePreprocessor.ProcessDirective
        };

        public static readonly KyuuPreprocessorDirective AdminDirective = new()
        {
            Identifier = "admin",
            Parameters = false,
            Task = AdminDirectivePreprocessor.ProcessDirective
        };

        public static readonly Dictionary<String, KyuuPreprocessorDirective> Directives = new()
        {
            { "event", EventDirective },
            { "category", CategoryDirective },
            { "command", CommandDirective },
            { "message", MessageDirective },
            { "permission", PermissionDirective },
            { "admin", AdminDirective }
        };
    }
}
