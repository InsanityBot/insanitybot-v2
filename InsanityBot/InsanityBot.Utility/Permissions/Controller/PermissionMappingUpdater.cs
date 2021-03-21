using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Controller
{
    public static class PermissionMappingUpdater
    {
        public const String IntermediateFilePath = "./cache/permissions/intermediate";
        public const String ModPermissionFilePath = "./mod-data/permissions";

        public static Boolean CheckForUpdates()
        {
            if (!File.Exists($"{IntermediateFilePath}/default-checksums"))
            {
                File.Create($"{IntermediateFilePath}/default-checksums").Close();
                return true;
            }

            StreamReader reader = new($"{IntermediateFilePath}/mapping-checksums");
            SHA512HashMap<String> checksums = JsonConvert.DeserializeObject<SHA512HashMap<String>>(reader.ReadToEnd());

            if (!(checksums["./config/permissions/vanilla.mappings.json".GetSHA512Checksum()] == "vanilla"))
                return true;

            var modFiles = from x in Directory.GetFiles(ModPermissionFilePath)
                           where x.EndsWith(".mappings.json")
                           select x;

            foreach (var v in modFiles)
            {
                if (!checksums.Keys.Contains(v.GetSHA512Checksum()))
                    return true;
            }

            return false;
        }

        public static Dictionary<DSharpPlus.Permissions, String[]> BuildMappings()
        {
            StreamReader reader = new($"./config/permissions/vanilla.mappings.json");
            Dictionary<DSharpPlus.Permissions, String[]> mappings = JsonConvert.DeserializeObject<Dictionary<DSharpPlus.Permissions, String[]>>(reader.ReadToEnd());
            reader.Close();

            var modFiles = from x in Directory.GetFiles(ModPermissionFilePath)
                           where x.EndsWith(".mappings.json")
                           select x;

            foreach(var v in modFiles)
            {
                reader = new(v);
                Dictionary<DSharpPlus.Permissions, String[]> temp = JsonConvert.DeserializeObject<Dictionary<DSharpPlus.Permissions, String[]>>(reader.ReadToEnd());

                foreach(var v1 in temp)
                {
                    if (!mappings.ContainsKey(v1.Key))
                        mappings.Add(v1.Key, v1.Value);
                    else
                    {
                        mappings[v1.Key] = mappings[v1.Key]
                            .Concat(v1.Value)
                            .ToArray();
                    }
                }

                reader.Close();
            }

            StreamWriter writer = new($"./config/permissions/mappings.json");
            writer.Write(JsonConvert.SerializeObject(mappings.ToHumanReadable()));
            writer.Close();

            UpdateChecksumData();

            return mappings;
        }

        public static void UpdateChecksumData()
        {
            SHA512HashMap<String> map = new();

            map.Add($"./config/permissions/vanilla.mappings.json".GetSHA512Checksum(), "vanilla");

            var modFiles = from x in Directory.GetFiles(ModPermissionFilePath)
                           where x.EndsWith(".mappings.json")
                           select x;

            foreach (var v in modFiles)
            {
                String modName = v.Split('\\')
                    .Last()
                    .Split('.')[0];

                map.Add(v.GetSHA512Checksum(), modName);
            }

            StreamWriter writer = new(File.Open($"{IntermediateFilePath}/default-mappings", FileMode.Truncate));
            writer.Write(JsonConvert.SerializeObject(map));
            writer.Close();
        }
    }
}
