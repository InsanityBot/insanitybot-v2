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
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    public class DefaultPermissions : PermissionBase
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
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
            var value = JsonConvert.DeserializeObject<DefaultPermissions>(reader.ReadToEnd());
            reader.Close();
            return value;
        }

        public static void Serialize(DefaultPermissions permissions)
        {
            StreamWriter writer = new(DefaultPermissionFileSpecifications.Default.GetFilePath());
            writer.Write(JsonConvert.SerializeObject(permissions, Formatting.Indented));
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

        /// <summary>
        /// THIS IS NOT AN EQUALITY CHECK!
        /// This solely tests compatibility between the existing defaults and updated defaults and determines whether the defaults have to be rebuilt entirely.
        /// </summary>
        public static Boolean operator == (DefaultPermissions left, DefaultPermissions right)
        {
            if (left.Permissions.Keys == right.Permissions.Keys)
                return true;
            return false;
        }

        public static Boolean operator != (DefaultPermissions left, DefaultPermissions right)
        {
            return !(left == right);
        }
    }
}
