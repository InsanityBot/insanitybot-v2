using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;

using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Reference
{
    public abstract class PermissionBase
    {
        public virtual UInt64 SnowflakeIdentifier { get; set; }
        public virtual Dictionary<String, Boolean> Permissions { get; set; }
        public virtual Boolean IsAdministrator { get; set; }
    }
}
