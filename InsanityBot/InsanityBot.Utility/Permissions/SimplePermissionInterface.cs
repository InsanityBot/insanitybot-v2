using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Interface;

namespace InsanityBot.Utility.Permissions
{
    public static class SimplePermissionInterface // we keep this for convenience only. complex requests should use InsanityBot.Utility.Permissions.Interface
    {
        public static Boolean HasPermission(this DiscordMember member, String permission, Boolean requiresAdmin)
        {
            if (requiresAdmin)
                return member.HasAdministrator();

            if (permission.StartsWith("script"))
                return member.CheckScriptPermission(permission);
            return member.CheckUserPermission(permission);
        }
    }
}
