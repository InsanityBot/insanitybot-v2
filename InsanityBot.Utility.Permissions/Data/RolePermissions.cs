using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;

namespace InsanityBot.Utility.Permissions.Data
{
    public class RolePermissions : PermissionBase
    {
        public UInt64 Parent { get; set; }

        public RolePermissions() : base() => Parent = 0;

        public RolePermissions(UInt64 id) : base() => Parent = 0;

        public static RolePermissions operator +(RolePermissions left, RolePermissions right)
        {
            RolePermissions final = left;
            final.Parent = right.SnowflakeIdentifier;
            return final;
        }

        public static RolePermissions operator -(RolePermissions left, RolePermissions right)
        {
            RolePermissions final = left;
            if (final.Parent == right.SnowflakeIdentifier)
            {
                final.Parent = 0;
            }

            return final;
        }

        public static RolePermissions operator +(RolePermissions left, String right)
        {
            RolePermissions final = left;
            if (final[right] != PermissionValue.Allowed)
            {
                final[right]++;
            }

            return final;
        }

        public static RolePermissions operator -(RolePermissions left, String right)
        {
            RolePermissions final = left;
            if (final[right] != PermissionValue.Denied)
            {
                final[right]--;
            }

            return final;
        }

        public RolePermissions Update(DefaultPermissions defaults)
        {
            if (defaults.UpdateGuid == this.UpdateGuid)
            {
                return this;
            }

            foreach (KeyValuePair<String, PermissionValue> v in defaults.Permissions)
            {
                if (!this.Permissions.ContainsKey(v.Key))
                {
                    this.Permissions.Add(v.Key, PermissionValue.Inherited);
                }
            }
            foreach (KeyValuePair<String, PermissionValue> v in this.Permissions)
            {
                if (!defaults.Permissions.ContainsKey(v.Key))
                {
                    this.Permissions.Remove(v.Key);
                }
            }
            this.UpdateGuid = defaults.UpdateGuid;

            Serialize(this);

            return this;
        }

        public static RolePermissions Create(UInt64 roleId, DefaultPermissions defaults)
        {
            RolePermissions permissions = new();
            permissions.SnowflakeIdentifier = roleId;
            permissions.UpdateGuid = defaults.UpdateGuid;

            foreach (KeyValuePair<String, PermissionValue> v in defaults.Permissions)
            {
                permissions.Permissions.Add(v.Key, PermissionValue.Inherited);
            }

            return permissions;
        }

        public static RolePermissions Deserialize(UInt64 id)
        {
            StreamReader reader = new(DefaultPermissionFileSpecifications.Role.GetFilePath().Replace("{ID}", id.ToString()));
            RolePermissions permissions = JsonConvert.DeserializeObject<RolePermissions>(reader.ReadToEnd());
            reader.Close();

            if (PermissionSettings.UpdateRolePermissions)
            {
                DefaultPermissions defaults = DefaultPermissions.Deserialize();
                if (permissions.UpdateGuid == defaults.UpdateGuid)
                {
                    return permissions;
                }
                else
                {
                    return permissions.Update(defaults);
                }
            }
            return permissions;
        }

        public static void Serialize(RolePermissions permissions)
        {
            StreamWriter writer = new(DefaultPermissionFileSpecifications.Role.GetFilePath().Replace("{ID}",
                permissions.SnowflakeIdentifier.ToString()));
            writer.Write(JsonConvert.SerializeObject(permissions, Formatting.Indented));
            writer.Close();
        }
    }
}
