using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Model;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Controller
{
    public static class RolePermissionSerializer
    {
        public const String RolePermissionFilePath = "./data/role-permissions";

        public static RolePermissions Deserialize(UInt64 roleId)
        {
            if(!File.Exists($"{RolePermissionFilePath}/{roleId}.json"))
            {
                FileInfo defaultFile = new("./config/permissions/default.json");
                defaultFile.CopyTo($"{RolePermissionFilePath}/{roleId}.json");
            }

            StreamReader reader = new($"{RolePermissionFilePath}/{roleId}.json");
            return JsonConvert.DeserializeObject<RolePermissions>(reader.ReadToEnd());
        }

        public static void Serialize(RolePermissions permissions)
        {
            StreamWriter writer = new($"{RolePermissionFilePath}/{permissions.SnowflakeIdentifier}.json");
            writer.Write(JsonConvert.SerializeObject(permissions));
            writer.Close();
        }
    }
}
