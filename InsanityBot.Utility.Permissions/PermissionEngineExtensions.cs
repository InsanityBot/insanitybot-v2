using System;
using System.Collections.Generic;
using System.Linq;

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

            if(permissions.IsAdministrator)
            {
                return true;
            }

            if(permissions[permission] == PermissionValue.Allowed)
            {
                return true;
            }
            else if(permissions[permission] == PermissionValue.Denied)
            {
                return false;
            }

            List<UInt64> roles = permissions.AssignedRoles.ToList();

            roles.AddRange(from v in member.Roles
                           where !roles.Contains(v.Id)
                           select v.Id);

            while(roles.Count != 0)
            {
                RolePermissions rolePermissions = activeEngine.GetRolePermissions(roles[0]);

                if(rolePermissions.IsAdministrator || rolePermissions[permission] == PermissionValue.Allowed)
                {
                    return true;
                }
                else if(rolePermissions[permission] == PermissionValue.Denied)
                {
                    return false;
                }

                if(rolePermissions.Parent != 0)
                {
                    roles.Add(rolePermissions.Parent);
                }

                roles.Remove(roles[0]);
            }

            DefaultPermissions defaults = DefaultPermissions.Deserialize();

            if(defaults[permission] == PermissionValue.Allowed || defaults.IsAdministrator)
            {
                return true;
            }

            return false;
        }

        public static PermissionEngine InitializeEngine(this DiscordClient client, PermissionConfiguration configuration)
        {
            activeEngine = new(configuration);
            return activeEngine;
        }
    }
}
