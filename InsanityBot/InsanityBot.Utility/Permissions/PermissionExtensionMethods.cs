using System;
using System.Collections.Generic;
using System.Text;

using DSharpPlus;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Reference;

namespace InsanityBot.Utility.Permissions
{
    public static class PermissionExtensionMethods
    {
        public static void InitializePermissionFramework(this DiscordClient client)
        {
            client.GuildMemberRemoved += PermissionManager.RemoveGuildMember;
            client.GuildMemberUpdated += PermissionManager.UpdateGuildMember;
        }

        public static Boolean HasPermission(this DiscordMember member, String PermissionId)
        {
#if false
            UserPermissions permissions = UserPermissions.Deserialize(member.Id);
            if (permissions.IsAdministrator)
                return true;
            return permissions["PermissionId"];
#else
            return true;
#endif
        }
    }
}
