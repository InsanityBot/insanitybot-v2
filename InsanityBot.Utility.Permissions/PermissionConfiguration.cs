using System;

namespace InsanityBot.Utility.Permissions
{
    public record PermissionConfiguration
    {
        public Boolean UpdateUserPermissions { get; init; }
        public Boolean UpdateRolePermissions { get; init; }
        public Boolean PrecompiledScripts { get; init; }
    }
}
