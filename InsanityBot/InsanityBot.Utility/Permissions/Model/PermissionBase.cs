using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Permissions.Model
{
    public abstract class PermissionBase
    {
        public virtual UInt64 SnowflakeIdentifier { get; set; }
        public virtual Dictionary<String, Boolean> Permissions { get; set; }
        public virtual Boolean IsAdministrator { get; set; }
    }
}
