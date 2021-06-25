using System;

namespace InsanityBot.Utility.Permissions.Data
{
    public enum PermissionValue : SByte
    {
        Allowed = 1,
        Denied = -1,
        Inherited = 0
    }
}
