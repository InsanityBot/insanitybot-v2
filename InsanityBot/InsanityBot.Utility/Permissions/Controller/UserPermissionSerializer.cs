using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Permissions.Model;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Controller
{
    public static class UserPermissionSerializer
    {
        public static UserPermissions Deserialize(UInt64 userId)
        {
            if (!Directory.Exists($"./data/{userId}"))
                Directory.CreateDirectory($"./data/{userId}");

            if(!File.Exists($"./data/{userId}/permissions.json"))
            {
                FileInfo defaultFile = new("./config/permissions/default.json");
                defaultFile.CopyTo($"./data/{userId}/permissions.json");
            }

            StreamReader reader = new($"./data/{userId}/permissions.json");
            return JsonConvert.DeserializeObject<UserPermissions>(reader.ReadToEnd());
        }

        public static void Serialize(UserPermissions permissions)
        {
            StreamWriter writer = new($"./data/{permissions.SnowflakeIdentifier}/permissions.json");
            writer.Write(JsonConvert.SerializeObject(permissions));
            writer.Close();
        }
    }
}
