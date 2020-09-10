using System;
using System.Collections.Generic;
using System.Text;

using InsanityBot.Utility.Permissions.Serialization.Reference;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Serialization
{
    public class UserPermission
    {
        [JsonProperty("permissions")]
        public Reference.Permissions Permissions { get; set; }
        
        [JsonIgnore]
        public UInt64 UserID { get; set; }

        [JsonIgnore]
        public Byte HeartbeatsUnused { get; set; }
    }
}
