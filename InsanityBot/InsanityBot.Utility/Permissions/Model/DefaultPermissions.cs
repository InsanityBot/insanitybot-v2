using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Permissions.Model
{
    public class DefaultPermissions : PermissionBase
    {
        public override UInt64 SnowflakeIdentifier
        {
            get => 0;
            set => base.SnowflakeIdentifier = 0;
        }
    }
}
