using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace InsanityBot.Utility.Permissions.Reference
{
    public class PermissionBase
    {
        protected static Dictionary<String, Boolean> GetDefaultPermissions()
        {
            return new Dictionary<String, Boolean>
            {
                { "insanitybot.miscellaneous.say", false },
                { "insanitybot.miscellaneous.say.embed", true }
            };
        }

        public UInt64 SnowflakeIdentifier { get; protected set; }

        public Dictionary<String, Boolean> Permissions = new Dictionary<String, Boolean>();

        public Boolean this[String parameter]
        {
            get => Permissions[parameter];
            set => Permissions[parameter] = value;
        }

        public static PermissionBase operator + (PermissionBase left, PermissionBase right)
        {
            PermissionBase returnValue = new PermissionBase(left.SnowflakeIdentifier);

            foreach(var v in left.Permissions)
                if (v.Value && !right[v.Key])
                    returnValue[v.Key] = true;

            return returnValue;
        }

        public static PermissionBase operator - (PermissionBase left, PermissionBase right)
        {
            PermissionBase returnValue = new PermissionBase(left.SnowflakeIdentifier);

            foreach (var v in left.Permissions)
                if (!v.Value && right[v.Key])
                    returnValue[v.Key] = true;

            return returnValue;
        }

        protected PermissionBase()
        {
            this.SnowflakeIdentifier = 0;
            this.Permissions = GetDefaultPermissions();
        }

        protected PermissionBase(UInt64 SnowflakeIdentifier)
        {
            this.SnowflakeIdentifier = SnowflakeIdentifier;
            this.Permissions = GetDefaultPermissions();
        }

        protected PermissionBase(UInt64 SnowflakeIdentifier, Dictionary<String, Boolean> Permissions)
        {
            this.SnowflakeIdentifier = SnowflakeIdentifier;
            this.Permissions = GetDefaultPermissions();
        }
    }
}
