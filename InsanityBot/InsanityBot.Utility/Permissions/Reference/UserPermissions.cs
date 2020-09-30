using System;
using System.Collections.Generic;

namespace InsanityBot.Utility.Permissions.Reference
{
    public class UserPermissions : PermissionBase, ICacheable
    {
        public Guid CacheEntryGuid { get; set; }
        public DateTime LastUsedAt { get; set; }

        public UserPermissions(UInt64 UserId, Dictionary<String, Boolean> Permissions)
        {
            this.Permissions = Permissions;
            this.CacheEntryGuid = Guid.NewGuid();
            this.LastUsedAt = DateTime.UtcNow;
            base.SnowflakeIdentifier = UserId;
        }
        public UserPermissions(UInt64 UserId) : this(UserId, GetDefaultPermissions()) { }
    }
}
