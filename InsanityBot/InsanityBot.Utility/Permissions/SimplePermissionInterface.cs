using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Controller;
using InsanityBot.Utility.Permissions.Model;

namespace InsanityBot.Utility.Permissions
{
    public static class SimplePermissionInterface // we keep this for convenience only. complex requests should use InsanityBot.Utility.Permissions.Interface
    {
        public static Boolean HasPermission(this DiscordMember member, String permission)
        {
            UserPermissions permissions = UserPermissionSerializer.Deserialize(member.Id);
            
            if(permissions.IsAdministrator)
                return true;

            return permissions[permission];
        }
    }
}
