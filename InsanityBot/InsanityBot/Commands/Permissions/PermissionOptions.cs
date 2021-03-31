using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

namespace InsanityBot.Commands.Permissions
{
    public class PermissionOptions
    {
        [Option('s', "silent", Default = false, Required = false)]
        public Boolean Silent { get; set; }

        [Option('p', "permission", Required = true)]
        public String Permission { get; set; }
    }
}
