using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Permissions.Controller;
using InsanityBot.Utility.Permissions.Model;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Interface
{
    public static class UserRoleSync
    {
        public static void SyncUsers()
        {
            // no users assigned to any roles, nothing to sync
            if (!File.Exists("./cache/permissions/role-user-mappings.json"))
                return;

            StreamReader reader = new("./cache/permissions/role-user-mappings.json");

            Dictionary<UInt64, UInt64[]> roleUserMappings = JsonConvert.DeserializeObject
                <Dictionary<UInt64, UInt64[]>>(reader.ReadToEnd());

            PermissionBackupHandler.BackupUsers(true);

            foreach (var v in roleUserMappings)
                Task.Run(() => { SyncSingleRole(v.Key, v.Value); });
        }

        private static void SyncSingleRole(UInt64 roleId, UInt64[] userIds)
        {
            // this role is just default, return
            if (!File.Exists($"./data/role-permissions/{roleId}.json"))
                return;

            RolePermissions permissions = RolePermissionSerializer.Deserialize(roleId);

            foreach (var v in userIds)
                Task.Run(() => { SyncUser(permissions, v); });
        }

        private static void SyncUser(RolePermissions permissions, UInt64 userId)
        {
            UserPermissions perms = UserPermissionSerializer.Deserialize(userId);
            perms += permissions;
            UserPermissionSerializer.Serialize(perms);
        }
    }
}
