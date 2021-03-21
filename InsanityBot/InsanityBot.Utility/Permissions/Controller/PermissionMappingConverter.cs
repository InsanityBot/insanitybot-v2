using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Permissions.Controller
{
    public static class PermissionMappingConverter
    {
        public static Dictionary<String, String[]> ToHumanReadable(this Dictionary<DSharpPlus.Permissions, String[]> mapping)
        {
            Dictionary<String, String[]> readableMapping = new();

            foreach(var v in mapping)
            {
                readableMapping.Add(v.Key.ToString(), v.Value);
            }

            return readableMapping;
        }

        public static Dictionary<DSharpPlus.Permissions, String[]> ToIntermediary(this Dictionary<String, String[]> mapping)
        {
            Dictionary<DSharpPlus.Permissions, String[]> intermediary = new();

            foreach(var v in mapping)
            {
                intermediary.Add(
                    Enum.Parse<DSharpPlus.Permissions>(v.Key),
                    v.Value);
            }

            return intermediary;
        }
    }
}
