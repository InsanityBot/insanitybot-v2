using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Permissions.Data
{
    public enum PermissionValue : SByte
    {
        Allowed = 1,
        Denied = -1,
        Inherited = 0
    }
}
