using System;
using System.Collections.Generic;

namespace InsanityBot.Utility.Permissions.Data
{
    public abstract class PermissionBase
    {
        public Dictionary<String, PermissionValue> Permissions { get; set; }
        public Boolean IsAdministrator { get; set; }
        public UInt64 SnowflakeIdentifier { get; set; }
        public Guid UpdateGuid { get; set; }

        public virtual PermissionValue this[String key]
        {
            get => Permissions[key];
            set => Permissions[key] = value;
        }

        public PermissionBase()
        {
            Permissions = new();
            SnowflakeIdentifier = 0;
            IsAdministrator = false;
            UpdateGuid = new(new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        }
    }
}
