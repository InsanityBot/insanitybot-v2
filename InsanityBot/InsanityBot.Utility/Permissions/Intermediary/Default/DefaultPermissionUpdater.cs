using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Intermediary.Default
{
    public static class DefaultPermissionUpdater
    {

        public static Boolean CheckForUpdate()
        {
            if (!File.Exists("./cache/permissions/intermediary-pdecl-checksums.json"))
                return true;

            StreamReader reader = new("./cache/permissions/intermediary-pdecl-checksums.json");
            List<String> permissionDeclarationChecksums = JsonConvert.DeserializeObject<List<String>>(reader.ReadToEnd());
            reader.Close();

            if (!permissionDeclarationChecksums.Contains("./config/permissions/vanilla.pdecl.json".GetSHA512Checksum()))
                return true;

            if (!Directory.Exists("./mod-data/permissions"))
                return false;

            List<String> filenames = (from v in Directory.GetFiles("./mod-data/permissions")
                                      where v.EndsWith(".pdecl.json")
                                      select v).ToList();

            foreach (var v in filenames)
                if (!permissionDeclarationChecksums.Contains(v.GetSHA512Checksum()))
                    return true;

            return false;
        }

        public static void UpdateDefaultPermissions()
        {

        }
    }
}
