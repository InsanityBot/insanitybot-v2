using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emzi0767;

namespace InsanityBot.Utility.Permissions.Model
{
    public class UserPermissions : PermissionBase
    {
        public override UInt64 SnowflakeIdentifier
        {
            get => base.SnowflakeIdentifier;
            set
            {
                if (value < 0 || value.CalculateLength() < 16)
                    throw new ArgumentException($"Invalid user snowflake identifier: {value}", nameof(value));
                base.SnowflakeIdentifier = value;
            }
        }

        public UserPermissions(UInt64 id)
        {
            this.SnowflakeIdentifier = id;
            this.Permissions = new();
            this.IsAdministrator = false;
        }

        public static UserPermissions operator +(UserPermissions left, RolePermissions right)
        {
            UserPermissions permissions = left;

            if (right.IsAdministrator)
                permissions.IsAdministrator = true;

            foreach(var v in right.Permissions)
            {
                if (v.Value)
                    left.Permissions[v.Key] = true;
            }

            return permissions;
        }

        public static UserPermissions operator +(UserPermissions left, DefaultPermissions right)
        {
            UserPermissions permissions = left;

            foreach (var v in right.Permissions)
            {
                if (!permissions.Permissions.ContainsKey(v.Key))
                    permissions.Permissions.Add(v.Key, v.Value);
            }

            return permissions;
        }

        public static UserPermissions operator -(UserPermissions left, RolePermissions right)
        {
            UserPermissions permissions = left;

            if (!right.IsAdministrator)
                permissions.IsAdministrator = false;

            foreach(var v in right.Permissions)
            {
                if (!v.Value)
                    left.Permissions[v.Key] = false;
            }

            return permissions;
        }

        public static UserPermissions operator -(UserPermissions left, DefaultPermissions right)
        {
            UserPermissions permissions = new(left.SnowflakeIdentifier);

            foreach (var v in right.Permissions)
            {
                if (left.Permissions.ContainsKey(v.Key))
                    permissions.Permissions.Add(v.Key, left.Permissions[v.Key]);
                else
                    permissions.Permissions.Add(v.Key, v.Value);
            }

            return permissions;
        }

        public Boolean this[String key]
        {
            get => Permissions[key];
            set => Permissions[key] = value;
        }
    }
}
