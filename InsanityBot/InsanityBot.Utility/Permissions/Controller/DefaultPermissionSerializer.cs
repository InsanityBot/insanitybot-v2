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
    public static class DefaultPermissionSerializer
    {
        public static DefaultPermissions GetDefaultPermissions()
        {
            if (DefaultPermissionUpdater.CheckForUpdates())
                DefaultPermissionUpdater.BuildDefaultPermissions();

            StreamReader reader = new($"./config/permissions/default.json");
            return JsonConvert.DeserializeObject<DefaultPermissions>(reader.ReadToEnd());
        }

        public static void SetDefaultPermissions(DefaultPermissions permissions)
        {
            StreamWriter writer = new($"./config/permissions/default.json");
            writer.Write(JsonConvert.SerializeObject(permissions));
            writer.Close();
        }
    }
}
