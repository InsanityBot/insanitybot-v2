using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Controller;
using InsanityBot.Utility.Permissions.Model;

namespace InsanityBot.Utility.Permissions.Interface
{
    public static class CheckPermissions
    {
        #region User
        public static Boolean CheckUserPermission(this DiscordMember member, String permission)
        {
            UserPermissions permissions = UserPermissionSerializer.Deserialize(member.Id);

            if (permissions.IsAdministrator)
                return true;
            return permissions[permission];
        }

        public static Boolean HasAdministrator(this DiscordMember member)
        {
            UserPermissions permissions = UserPermissionSerializer.Deserialize(member.Id);

            if (permissions.IsAdministrator)
                return true;
            return false;
        }
        #endregion

        #region Role
        public static Boolean CheckRolePermission(this DiscordRole role, String permission)
        {
            RolePermissions permissions = RolePermissionSerializer.Deserialize(role.Id);

            if (permissions.IsAdministrator)
                return true;
            return permissions[permission];
        }
        #endregion

        #region Script
        public static Boolean CheckScriptPermission(this DiscordMember member, String permission)
        {
            ScriptPermissions permissions = ScriptPermissionSerializer.GetScriptPermissions(member.Id);

            if (permissions.IsAdministrator)
                return true;
            return permissions.Permissions[permission];
        }
        #endregion

        #region Default
        public static Boolean CheckDefaultPermission(String permission)
        {
            DefaultPermissions permissions = DefaultPermissionSerializer.GetDefaultPermissions();

            if (permissions.IsAdministrator)
                return true;
            return permissions.Permissions[permission];
        }
        #endregion
    }
}
