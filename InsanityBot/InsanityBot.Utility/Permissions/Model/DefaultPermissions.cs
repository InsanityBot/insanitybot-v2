using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Permissions.Model
{
    public class DefaultPermissions : PermissionBase
    {
        public override UInt64 SnowflakeIdentifier
        {
            get => 0;
            set => base.SnowflakeIdentifier = 0;
        }

        public static DefaultPermissions Empty
        {
            get => new();
        }

        public DefaultPermissions()
        {
            SnowflakeIdentifier = 0;
            IsAdministrator = false;
            Permissions = new();
        }

        public static DefaultPermissions operator +(DefaultPermissions left, DefaultPermissions right)
        {
            DefaultPermissions permissions = Empty;

            if (left.IsAdministrator || right.IsAdministrator)
                permissions.IsAdministrator = true;

            foreach (var v in left.Permissions)
                permissions.Permissions[v.Key] = v.Value;

            foreach (var v in right.Permissions)
                permissions.Permissions[v.Key] = v.Value;

            return permissions;
        }

        public static DefaultPermissions operator +(DefaultPermissions left, PermissionDeclaration right)
        {
            DefaultPermissions permissions = left;

            permissions.Permissions[right.Permission] = right.Default;

            return permissions;
        }

        public static DefaultPermissions operator +(DefaultPermissions left, PermissionDeclaration[] right)
        {
            DefaultPermissions permissions = left;

            foreach (var v in right)
                left.Permissions[v.Permission] = v.Default;

            return permissions;
        }
    }
}
