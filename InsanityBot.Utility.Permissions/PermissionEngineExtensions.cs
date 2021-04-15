using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Data;

namespace InsanityBot.Utility.Permissions
{
    public static class PermissionEngineExtensions
    {
        private static PermissionEngine activeEngine;

        public static Boolean HasPermission(this DiscordMember member, String permission)
        {
            UserPermissions permissions = activeEngine.GetUserPermissions(member.Id);

            if (permissions.IsAdministrator)
                return true;

            if (permissions[permission] == PermissionValue.Allowed)
                return true;
            else if (permissions[permission] == PermissionValue.Denied)
                return false;

            List<UInt64> inheritedRoles = permissions.AssignedRoles.ToList();

            inheritedRoles.AddRange(from v in member.Roles
                                    select v.Id);

            List<UInt64> finalRoles = new(inheritedRoles);
            
            foreach(var v in inheritedRoles)
                finalRoles.AddRange(GetRoleIdsRecursive(v));
            
            foreach(var v in finalRoles)
            {
                RolePermissions inherited = activeEngine.GetRolePermissions(v);

                if (inherited.IsAdministrator)
                    return true;

                if (inherited[permission] == PermissionValue.Allowed)
                    return true;
                else if (inherited[permission] == PermissionValue.Denied)
                    return false;
            }

            DefaultPermissions defaults = DefaultPermissions.Deserialize();

            if (defaults[permission] == PermissionValue.Allowed || defaults.IsAdministrator)
                return true;

            return false;
        }

        private static List<UInt64> GetRoleIdsRecursive(UInt64 roleId)
        {
            List<UInt64> value = new();
            RolePermissions permissions;

            do
            {
                permissions = activeEngine.GetRolePermissions(roleId);
                if (permissions.Parent != 0)
                    value.Add(permissions.Parent);
            } while (permissions.Parent != 0);

            return value;
        }

        public static PermissionEngine InitializeEngine(this DiscordClient client, PermissionConfiguration configuration)
        {
            activeEngine = new(configuration);
            return activeEngine;
        }
    }
}
