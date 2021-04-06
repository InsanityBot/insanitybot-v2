using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Data
{
    public class UserPermissions : PermissionBase
    {
        public UInt64[] AssignedRoles { get; set; }

        public UserPermissions() : base()
        {
            AssignedRoles = Array.Empty<UInt64>();
        }

        public UserPermissions(UInt64 id) : base()
        {
            SnowflakeIdentifier = id;
            AssignedRoles = Array.Empty<UInt64>();
        }

        public static UserPermissions operator + (UserPermissions left, RolePermissions right)
        {
            UserPermissions final = left;
            final.AssignedRoles = final.AssignedRoles.Append(right.SnowflakeIdentifier).ToArray();
            return final;
        }

        public static UserPermissions operator - (UserPermissions left, RolePermissions right)
        {
            UserPermissions final = left;
            final.AssignedRoles = final.AssignedRoles.Where(xm => xm != right.SnowflakeIdentifier).ToArray();
            return final;
        }

        public static UserPermissions operator + (UserPermissions left, String right)
        {
            UserPermissions final = left;

            if (right.StartsWith("script."))
                left[right] = PermissionValue.Allowed;

            if (final[right] != PermissionValue.Allowed)
                final[right]++;
            return final;
        }

        public static UserPermissions operator - (UserPermissions left, String right)
        {
            UserPermissions final = left;

            if (right.StartsWith("script."))
                left[right] = PermissionValue.Denied;

            if (final[right] != PermissionValue.Denied)
                final[right]--;
            return final;
        }

        public UserPermissions Update(DefaultPermissions defaults)
        {
            if (defaults.UpdateGuid == this.UpdateGuid)
                return this;

            foreach(var v in defaults.Permissions)
            {
                if (!this.Permissions.ContainsKey(v.Key))
                    this.Permissions.Add(v.Key, PermissionValue.Inherited);
            }
            foreach(var v in this.Permissions)
            {
                if (!defaults.Permissions.ContainsKey(v.Key))
                    this.Permissions.Remove(v.Key);
            }
            this.UpdateGuid = defaults.UpdateGuid;

            Serialize(this);

            return this;
        }

        public static UserPermissions Create(UInt64 userId, DefaultPermissions defaults)
        {
            UserPermissions permissions = new();
            permissions.SnowflakeIdentifier = userId;
            permissions.UpdateGuid = defaults.UpdateGuid;

            foreach(var v in defaults.Permissions)
            {
                permissions.Permissions.Add(v.Key, PermissionValue.Inherited);
            }

            return permissions;
        }

        public static UserPermissions Deserialize(UInt64 Id)
        {
            StreamReader reader = new(DefaultPermissionFileSpecifications.User.GetFilePath().Replace("{ID}", Id.ToString()));
            UserPermissions permissions = JsonConvert.DeserializeObject<UserPermissions>(reader.ReadToEnd());
            reader.Close();

            if (PermissionSettings.UpdateUserPermissions)
            {
                DefaultPermissions defaults = DefaultPermissions.Deserialize();
                if (permissions.UpdateGuid == defaults.UpdateGuid)
                    return permissions;
                else
                    return permissions.Update(defaults);
            }
            return permissions;
        }

        public static void Serialize(UserPermissions permissions)
        {
            StreamWriter writer = new(DefaultPermissionFileSpecifications.User.GetFilePath().Replace("{ID}",
                permissions.SnowflakeIdentifier.ToString()));
            writer.Write(JsonConvert.SerializeObject(permissions, Formatting.Indented));
            writer.Close();
        }
    }
}
