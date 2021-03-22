using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Permissions.Controller;
using InsanityBot.Utility.Permissions.Model;

namespace InsanityBot.Utility.Permissions.Interface
{
    public static class RoleDefaultSync
    {
        public const String RolePermissionFilePath = "./data/role-permissions";

        public static void SyncRoles()
        {
            String[] roles = Directory.GetFiles(RolePermissionFilePath);
            UInt64[] roleIds = new UInt64[roles.Length];

            foreach(var v in roles)
            {
                String id = v.Split('\\')
                    .Last()
                    .Split('.')[0];
                roleIds = roleIds.Append(Convert.ToUInt64(id)).ToArray();
            }

            PermissionBackupHandler.BackupRoles(true);

            foreach (var v in roleIds)
                Task.Run(() => { SyncRole(v); });
        }

        private static void SyncRole(UInt64 id)
        {
            DefaultPermissions defaultPermissions = DefaultPermissionSerializer.GetDefaultPermissions();
            RolePermissions rolePermissions = RolePermissionSerializer.Deserialize(id);

            rolePermissions += defaultPermissions;
            RolePermissionSerializer.Serialize(rolePermissions);
        }
    }
}
