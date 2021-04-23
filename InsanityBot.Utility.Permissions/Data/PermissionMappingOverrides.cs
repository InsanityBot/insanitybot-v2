using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Data
{
    public static class PermissionMappingOverrides
    {
        public static PermissionMapping Deserialize()
        {
            StreamReader reader = new("./config/overrides.mappings.json");
            Dictionary<String, String[]> readableMappings = JsonConvert.DeserializeObject<Dictionary<String, String[]>>(reader.ReadToEnd());

            Dictionary<Int64, String[]> finalMappings = new();

            foreach (KeyValuePair<String, String[]> v in readableMappings)
            {
                Int64 finalMappingValue = PermissionMapping.MappingTranslation.FirstOrDefault(xm => v.Key == xm.Value).Key;
                finalMappings.Add(finalMappingValue, v.Value);
            }

            return new PermissionMapping
            {
                Mappings = finalMappings
            };
        }

        // no serialize. this is user input only, for now.
    }
}
