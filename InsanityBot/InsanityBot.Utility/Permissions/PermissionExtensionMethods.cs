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
            UserPermissions permissions = (UserPermissions)UserPermissions.Deserialize(member.Id);
            return permissions[PermissionId];
        }
    }
}
