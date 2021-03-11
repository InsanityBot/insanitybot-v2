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
    public static class DefaultPermissionUpdater
    {
        public const String IntermediateFilePath = "./cache/permissions/intermediate";
        public const String ModPermissionFilePath = "./mod-data/permissions";

        public static Boolean CheckForUpdates()
        {
            if(!File.Exists($"{IntermediateFilePath}/default-checksums"))
            {
                File.Create($"{IntermediateFilePath}/default-checksums").Close();
                return true;
            }

            StreamReader reader = new($"{IntermediateFilePath}/default-checksums");
            SHA512HashMap<String> fileChecksums = JsonConvert.DeserializeObject<SHA512HashMap<String>>(reader.ReadToEnd());

            if (!(fileChecksums["./config/permissions/vanilla.pdecl.json".GetSHA512Checksum()] == "vanilla"))
                return true;

            var modFiles = from x in Directory.GetFiles(ModPermissionFilePath)
                           where x.EndsWith(".pdecl.json")
                           select x;

            foreach(var v in modFiles)
            {
                if (!fileChecksums.Keys.Contains(v.GetSHA512Checksum()))
                    return true;
            }

            return false;
        }

        public static DefaultPermissions BuildDefaultPermissions()
        {
            DefaultPermissions permissions = DefaultPermissions.Empty;

            StreamReader reader = new($"./config/permissions/vanilla.pdecl.json");
            permissions += JsonConvert.DeserializeObject<PermissionDeclaration[]>(reader.ReadToEnd());
            reader.Close();

            var modFiles = from x in Directory.GetFiles(ModPermissionFilePath)
                           where x.EndsWith(".pdecl.json")
                           select x;

            foreach(var v in modFiles)
            {
                reader = new(v);
                permissions += JsonConvert.DeserializeObject<PermissionDeclaration[]>(reader.ReadToEnd());
                reader.Close();
            }

            StreamWriter writer = new($"./config/permissions/default.json");
            writer.Write(JsonConvert.SerializeObject(permissions));
            writer.Close();

            UpdateChecksumData();

            return permissions;
        }

        public static void UpdateChecksumData()
        {
            SHA512HashMap<String> map = new();

            map.Add($"./config/permissions/vanilla.pdecl.json".GetSHA512Checksum(), "vanilla");

            var modFiles = from x in Directory.GetFiles(ModPermissionFilePath)
                           where x.EndsWith(".pdecl.json")
                           select x;

            foreach(var v in modFiles)
            {
                String modName = v.Split('\\')
                    .Last()
                    .Split('.')[0];

                map.Add(v.GetSHA512Checksum(), modName);
            }

            StreamWriter writer = new(File.Open($"{IntermediateFilePath}/default-checksums", FileMode.Truncate));
            writer.Write(JsonConvert.SerializeObject(map));
            writer.Close();
        }
    }
}
