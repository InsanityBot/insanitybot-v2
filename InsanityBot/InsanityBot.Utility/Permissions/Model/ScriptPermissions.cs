using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Permissions.Model
{
    public class ScriptPermissions : PermissionBase
    {
        public ScriptPermissions()
        {
            this.SnowflakeIdentifier = 0;
            this.Permissions = new();
            this.IsAdministrator = true;
        }
    }
}
