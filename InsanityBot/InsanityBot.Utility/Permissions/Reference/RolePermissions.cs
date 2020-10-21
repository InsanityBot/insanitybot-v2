using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Reference
{
    public class RolePermissions : PermissionBase
    {
        public static RolePermissions Deserialize(UInt64 Identifier)
        {
            PermissionManager.GeneratePermissionFile(Identifier, PermissionFileType.Role);
            StreamReader reader = new StreamReader($"./data/permissions/{Identifier}.json");
            return JsonConvert.DeserializeObject<RolePermissions>(reader.ReadToEnd());
        }

        public static void Serialize(RolePermissions permissions)
        {
            StreamWriter writer = new StreamWriter($"./data/permissions/{permissions.SnowflakeIdentifier}.json");
            writer.BaseStream.SetLength(0);
            writer.Flush();
            writer.Write(JsonConvert.SerializeObject(permissions));
            writer.Close();
        }

        public RolePermissions(UInt64 Id, Dictionary<String, Boolean> permissions) : base(Id, permissions)
        { }

        public RolePermissions(UInt64 Id) : base(Id)
        { }
    }
}
