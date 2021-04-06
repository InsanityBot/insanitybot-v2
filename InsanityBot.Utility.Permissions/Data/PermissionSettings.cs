using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Permissions.Data
{
    public static class PermissionSettings
    {
        public static Boolean UpdateUserPermissions { get; internal set; }
        public static Boolean UpdateRolePermissions { get; internal set; }
        public static Boolean PrecompiledScripts { get; internal set; }
    }
}
