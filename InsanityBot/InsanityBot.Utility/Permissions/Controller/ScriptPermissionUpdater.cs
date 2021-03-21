using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Lavalink.EventArgs;

using InsanityBot.Utility.Permissions.Model;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Controller
{
    public static class ScriptPermissionUpdater
    {
        public const String ScriptFilePath = "./cache/scripts";

        public static Boolean CheckForUpdates()
        {
            StreamReader reader = new($"./config/permissions/scripts.json");
            ScriptPermissions currentDefault = JsonConvert.DeserializeObject<ScriptPermissions>(reader.ReadToEnd());
            reader.Close();

            if (currentDefault.Permissions.Count == 0)
                return false;

            var scripts = from x in Directory.GetFiles(ScriptFilePath)
                          where x.EndsWith(".is") || x.EndsWith(".isc") || x.EndsWith(".iscript")
                          select x;

            foreach(var v in scripts)
            {
                String scriptName = v.Split('\\')
                    .Last()
                    .Split('.')[0];

                if (!currentDefault.Permissions.ContainsKey(scriptName))
                    return true;
            }

            return false;
        }

        public static ScriptPermissions BuildScriptPermissions()
        {
            ScriptPermissions permissions = new();

            var scripts = from x in Directory.GetFiles(ScriptFilePath)
                          where x.EndsWith(".is") || x.EndsWith(".isc") || x.EndsWith(".iscript")
                          select x;

            foreach(var v in scripts)
            {
                String scriptName = v.Split('\\')
                    .Last()
                    .Split('.')[0];

                permissions.Permissions.Add(scriptName, true);
            }

            StreamWriter writer = new($"./config/permissions/scripts.json");
            writer.Write(JsonConvert.SerializeObject(permissions));
            writer.Close();

            return permissions;
        }

        public static ScriptPermissions UpdateScriptPermissions(this ScriptPermissions permissions)
        {
            ScriptPermissions perms = permissions;

            var scripts = from x in Directory.GetFiles(ScriptFilePath)
                          where x.EndsWith(".is") || x.EndsWith(".isc") || x.EndsWith(".iscript")
                          select x.Split('\\').Last().Split('.')[0];

            foreach (var v in scripts)
            {
                if(!perms.Permissions.ContainsKey(v))
                    perms.Permissions.Add(v, true);
            }

            List<String> toRemove = new();

            foreach(var v in perms.Permissions)
            {
                if (!scripts.Contains(v.Key))
                    toRemove.Add(v.Key);
            }

            foreach (var v in toRemove)
                perms.Permissions.Remove(v);

            return perms;
        }
    }
}
