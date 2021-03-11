using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Permissions.Model;

namespace InsanityBot.Utility.Permissions.Controller
{
    public static class UserPermissionUpdater
    {
        public static UserPermissions GetUpdatedRolePermissions(UInt64 userId)
        {
            UserPermissions permissions = UserPermissionSerializer.Deserialize(userId);
            permissions += DefaultPermissionSerializer.GetDefaultPermissions();
            permissions -= DefaultPermissionSerializer.GetDefaultPermissions();
            return permissions;
        }

        public static void UpdateRolePermissions(UInt64 userId)
        {
            UserPermissions permissions = UserPermissionSerializer.Deserialize(userId);
            permissions += DefaultPermissionSerializer.GetDefaultPermissions();
            permissions -= DefaultPermissionSerializer.GetDefaultPermissions();
            UserPermissionSerializer.Serialize(permissions);
        }
    }
}
