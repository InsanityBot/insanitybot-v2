using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Permissions
{
    public record PermissionConfiguration
    {
        public Boolean UpdateUserPermissions { get; init; }
        public Boolean UpdateRolePermissions { get; init; }
        public Boolean PrecompiledScripts { get; init; }
    }
}
