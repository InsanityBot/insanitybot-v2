using System;
using System.Collections.Generic;

using CommandLine;

namespace InsanityBot.Commands.Moderation
{
    public abstract class ModerationOptionBase
    {
        [Option('s', "silent", Default = false, Required = false)]
        public Boolean Silent { get; set; }

        [Option('d', "dmmember", Default = false, Required = false)]
        public Boolean DmMember { get; set; }

        [Option('r', "reason", Required = false, Separator = '!')]
        public IEnumerable<String> Reason { get; set; }
    }
}
