using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;

namespace InsanityBot.Utility.Permissions
{
    public static class PermissionEngineExtensions
    {
        private static PermissionEngine activeEngine;

        public static Boolean HasPermission(this DiscordMember member, String permission)
        {
            return true;
        }

        public static PermissionEngine InitializeEngine(this DiscordClient client, PermissionConfiguration configuration)
        {
            activeEngine = new(configuration);
            return activeEngine;
        }
    }
}
