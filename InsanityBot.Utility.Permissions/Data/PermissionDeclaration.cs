using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Data
{
    public struct PermissionDeclaration
    {
        public String Permission { get; set; }
        public Boolean Value { get; set; }

        public static PermissionDeclaration[] Deserialize(String modName = "vanilla")
        {
            StreamReader reader = modName switch
            {
                "vanilla" => new("./config/permissions/vanilla.pdecl.json"),
                _ => new($"./mod-data/permissions/{modName}.pdecl.json")
            };

            return JsonConvert.DeserializeObject<PermissionDeclaration[]>(reader.ReadToEnd());
        }

        //yknow, there'd be a serialize here but these are not intended to be written
    }
}
