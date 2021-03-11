using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Controller
{
    public static class PermissionMappingSerializer
    {
        public const String VanillaMappingFile = "./config/permissions/vanilla.mappings.json";
        public const String ModMappingFilePath = "./mod-data/permissions";
        public const String FinalMappingFile = "./config/permissions/mappings.json";

        public static Dictionary<DSharpPlus.Permissions, String[]> GetIntermediaryMappings(String name)
        {
            StreamReader reader;

            if (name == "vanilla")
            {
                reader = new(VanillaMappingFile);
                return JsonConvert.DeserializeObject<Dictionary<DSharpPlus.Permissions, String[]>>(reader.ReadToEnd());
            }

            reader = new($"{ModMappingFilePath}/{name}.mappings.json");
            return JsonConvert.DeserializeObject<Dictionary<DSharpPlus.Permissions, String[]>>(reader.ReadToEnd());
        }

        public static Dictionary<String, String[]> GetFinalMappings()
        {
            StreamReader reader = new(FinalMappingFile);

            return JsonConvert.DeserializeObject<Dictionary<String, String[]>>(reader.ReadToEnd());
        }

        public static void SetFinalMappings(Dictionary<String, String[]> mappings)
        {
            StreamWriter writer = new(FinalMappingFile);
            writer.Write(JsonConvert.SerializeObject(mappings));
            writer.Close();
        }
    }
}
