using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Permissions.Model
{
    public class RolePermissions : PermissionBase
    {
        public RolePermissions(UInt64 Id)
        {
            this.SnowflakeIdentifier = Id;
            this.Permissions = new();
            this.IsAdministrator = false;
        }

        public static RolePermissions operator +(RolePermissions left, PermissionBase right)
        {
            RolePermissions permissions = left;

            if (right.IsAdministrator)
                permissions.IsAdministrator = true;

            foreach (var v in right.Permissions)
                if (v.Value)
                    permissions.Permissions[v.Key] = true;

            return permissions;
        }

        public static RolePermissions operator +(RolePermissions left, DefaultPermissions right)
        {
            RolePermissions permissions = left;

            foreach (var v in right.Permissions)
            {
                if (!permissions.Permissions.ContainsKey(v.Key))
                    permissions.Permissions.Add(v.Key, v.Value);
            }

            return permissions;
        }

        public static RolePermissions operator -(RolePermissions left, PermissionBase right)
        {
            RolePermissions permissions = left;

            if (!right.IsAdministrator)
                permissions.IsAdministrator = false;

            foreach(var v in right.Permissions)
            {
                if (!v.Value && permissions.Permissions[v.Key])
                    permissions.Permissions[v.Key] = false;
            }

            return permissions;
        }

        public static RolePermissions operator -(RolePermissions left, DefaultPermissions right)
        {
            RolePermissions permissions = new(left.SnowflakeIdentifier);

            foreach(var v in right.Permissions)
            {
                if (left.Permissions.ContainsKey(v.Key))
                    permissions.Permissions.Add(v.Key, left.Permissions[v.Key]);
                else
                    permissions.Permissions.Add(v.Key, v.Value);
            }

            return permissions;
        }
    }
}
