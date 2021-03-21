using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Permissions.Model;

namespace InsanityBot.Utility.Permissions.Controller
{
    public static class RolePermissionUpdater
    {
        public static RolePermissions GetUpdatedRolePermissions(UInt64 roleId)
        {
            RolePermissions permissions = RolePermissionSerializer.Deserialize(roleId);
            permissions += DefaultPermissionSerializer.GetDefaultPermissions();
            permissions -= DefaultPermissionSerializer.GetDefaultPermissions();
            return permissions;
        }

        public static void UpdateRolePermissions(UInt64 roleId)
        {
            RolePermissions permissions = RolePermissionSerializer.Deserialize(roleId);
            permissions += DefaultPermissionSerializer.GetDefaultPermissions();
            permissions -= DefaultPermissionSerializer.GetDefaultPermissions();
            RolePermissionSerializer.Serialize(permissions);
        }
    }
}
