using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Reference
{
    public class UserPermissions : PermissionBase
    {
        public UserPermissions(UInt64 Id, Dictionary<String, Boolean> Permissions) : base(Id, Permissions)
        { }

        public UserPermissions(UInt64 Id) : base(Id)
        { }

        public static PermissionBase Deserialize(UInt64 Identifier)
        {
            PermissionManager.GeneratePermissionFile(Identifier, PermissionFileType.User);
            StreamReader reader = new StreamReader($"./data/{Identifier}/permissions.json");
            return JsonConvert.DeserializeObject<PermissionBase>(reader.ReadToEnd());
        }

        public static void Serialize(PermissionBase permissions)
        {
            StreamWriter writer = new StreamWriter($"./data/{permissions.SnowflakeIdentifier}/permissions.json");
            writer.BaseStream.SetLength(0);
            writer.Flush();
            writer.Write(JsonConvert.SerializeObject(permissions));
            writer.Close();
        }
    }
}
