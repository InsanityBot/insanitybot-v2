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
