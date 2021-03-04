using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Reference
{
    public class UserPermissions : PermissionBase
    {
        public UserPermissions(UInt64 Id, Dictionary<String, Boolean> Permissions) : base(Id, Permissions)
        { }

        public UserPermissions(UInt64 Id) : base(Id)
        { }

        public UserPermissions() : base()
        { }

        public static UserPermissions Deserialize(UInt64 Identifier)
        {
            PermissionManager.GeneratePermissionFile(Identifier, PermissionFileType.User);
            StreamReader reader = new($"./data/{Identifier}/permissions.json");

            UserPermissions perms = JsonConvert.DeserializeObject<UserPermissions>(reader.ReadToEnd());
            perms = (UserPermissions)DataFixerLower.UpgradeData(perms);
            reader.Close();

            Serialize(perms);
            return perms;
        }

        public static void Serialize(PermissionBase permissions)
        {
            StreamWriter writer = new($"./data/{permissions.SnowflakeIdentifier}/permissions.json");
            writer.BaseStream.SetLength(0);
            writer.Flush();
            writer.Write(JsonConvert.SerializeObject(permissions));
            writer.Close();
        }
    }
}
