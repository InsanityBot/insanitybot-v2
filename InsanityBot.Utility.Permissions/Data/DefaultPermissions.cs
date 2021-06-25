using Newtonsoft.Json;

using System;
using System.IO;

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
            get => this.Permissions[key];
            set
            {
                if(value == PermissionValue.Inherited)
                {
                    this.Permissions[key] = PermissionValue.Denied; // fall back to denied
                }
                else
                {
                    this.Permissions[key] = value;
                }
            }
        }

        public static DefaultPermissions Deserialize()
        {
            StreamReader reader = new(DefaultPermissionFileSpecifications.Default.GetFilePath());
            DefaultPermissions value = JsonConvert.DeserializeObject<DefaultPermissions>(reader.ReadToEnd());
            reader.Close();
            return value;
        }

        public static void Serialize(DefaultPermissions permissions)
        {
            StreamWriter writer = new(DefaultPermissionFileSpecifications.Default.GetFilePath());
            writer.Write(JsonConvert.SerializeObject(permissions, Formatting.Indented));
            writer.Close();
        }

        public static DefaultPermissions operator +(DefaultPermissions left, PermissionDeclaration[] right)
        {
            DefaultPermissions retValue = left;

            foreach(PermissionDeclaration v in right)
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

        public static DefaultPermissions Empty => new();

        /// <summary>
        /// THIS IS NOT AN EQUALITY CHECK!
        /// This solely tests compatibility between the existing defaults and updated defaults and determines whether the defaults have to be rebuilt entirely.
        /// </summary>
        public static Boolean operator ==(DefaultPermissions left, DefaultPermissions right)
        {
            if(left.Permissions.Keys == right.Permissions.Keys)
            {
                return true;
            }

            return false;
        }

        public static Boolean operator !=(DefaultPermissions left, DefaultPermissions right) => !(left == right);
    }
}
