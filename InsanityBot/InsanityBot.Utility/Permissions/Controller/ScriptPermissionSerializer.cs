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
    public class ScriptPermissionSerializer
    {
        public static ScriptPermissions GetScriptPermissions(UInt64 userId)
        {
            if (!Directory.Exists($"./data/{userId}"))
                Directory.CreateDirectory($"./data/{userId}");

            if (!File.Exists($"./data/{userId}/scripts.json"))
            {
                FileInfo defaultFile = new("./config/permissions/scripts.json");
                defaultFile.CopyTo($"./data{userId}/scripts.json");
            }

            StreamReader reader = new($"./data/{userId}/scripts.json");
            ScriptPermissions perms = JsonConvert.DeserializeObject<ScriptPermissions>(reader.ReadToEnd());

            if (ScriptPermissionUpdater.CheckForUpdates())
            {
                ScriptPermissionUpdater.BuildScriptPermissions();
                return perms.UpdateScriptPermissions();
            }

            return perms;
        }

        public static void SetScriptPermissions(ScriptPermissions permissions)
        {
            StreamWriter writer = new($"./data/{permissions.SnowflakeIdentifier}/scripts.json");
            writer.Write(JsonConvert.SerializeObject(permissions));
            writer.Close();
        }
    }
}
