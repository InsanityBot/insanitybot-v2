using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Data
{
    public class DefaultPermissions : PermissionBase
    {
        // PermissionValue.Inherited is invalid here, for obvious reasons
        public override PermissionValue this[String key]
        {
            get => Permissions[key];
            set
            {
                if (value == PermissionValue.Inherited)
                    Permissions[key] = PermissionValue.Denied; // fall back to denied
                else
                    Permissions[key] = value;
            }
        }

        public static DefaultPermissions Deserialize()
        {
            StreamReader reader = new(DefaultPermissionFileSpecifications.Default.GetFilePath());
            return JsonConvert.DeserializeObject<DefaultPermissions>(reader.ReadToEnd());
        }

        public static void Serialize(DefaultPermissions permissions)
        {
            StreamWriter writer = new(DefaultPermissionFileSpecifications.Default.GetFilePath());
            writer.Write(JsonConvert.SerializeObject(permissions));
            writer.Close();
        }

        public static DefaultPermissions operator + (DefaultPermissions left, PermissionDeclaration[] right)
        {
            DefaultPermissions retValue = left;
            
            foreach(var v in right)
            {
                retValue.Permissions.Add(v.Permission,
                    v.Value switch
                    {
                        true => PermissionValue.Allowed,
                        false => PermissionValue.Denied
                    });
            }

            return retValue;
        }

        public static DefaultPermissions Empty
        {
            get => new();
        }
    }
}
