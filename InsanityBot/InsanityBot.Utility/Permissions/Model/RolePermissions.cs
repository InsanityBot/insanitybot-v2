using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Permissions.Model
{
    public class RolePermissions : PermissionBase
    {
        public Dictionary<String, Boolean> TotalPermissions { get; set; }
    }
}
