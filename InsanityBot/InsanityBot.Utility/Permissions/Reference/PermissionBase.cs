using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Reference
{
    public abstract class PermissionBase
    {
        public UInt64 SnowflakeIdentifier { get; set; }
        public Dictionary<String, Boolean> Permissions { get; set; }
        public Boolean IsAdministrator { get; set; }

        [JsonIgnore]
        public static Dictionary<String, Boolean> DefaultPermissions = new Dictionary<String, Boolean>
        {
            { "insanitybot.miscellaneous.say", false },
            { "insanitybot.miscellaneous.say.embed", false },
            { "insanitybot.moderation.verbal_warn", false },
            { "insanitybot.moderation.warn", false },
            { "insanitybot.moderation.mute", false },
            { "insanitybot.moderation.tempmute", false },
            { "insanitybot.moderation.unmute", false },
            { "insanitybot.moderation.blacklist", false },
            { "insanitybot.moderation.whitelist", false },
            { "insanitybot.moderation.kick", false },
            { "insanitybot.moderation.ban", false },
            { "insanitybot.moderation.tempban", false },
            { "insanitybot.moderation.unban", false },
            { "insanitybot.moderation.verballog", false },
            { "insanitybot.moderation.modlog", true }
        };

        protected PermissionBase(UInt64 Id, Dictionary<String, Boolean> Permissions)
        {
            this.SnowflakeIdentifier = Id;
            this.Permissions = Permissions;
        }

        public PermissionBase()
        { }

        protected PermissionBase(UInt64 Id) : this(Id, GetDefaultPermissions()) { }


        private static Dictionary<String, Boolean> GetDefaultPermissions()
            => DefaultPermissions;    


        public static PermissionBase operator + (PermissionBase left, PermissionBase right)
        {
            foreach(var v in right.Permissions)
            {
                if (v.Value)
                    left.Permissions[v.Key] = true;
            }

            return left;
        }

        public static PermissionBase operator - (PermissionBase left, PermissionBase right)
        {
            foreach(var v in right.Permissions)
            {
                if (!v.Value)
                    left.Permissions[v.Key] = false;
            }

            return left;
        }

        public Boolean this[String Key]
        {
            get => this.Permissions[Key];
            set => this.Permissions[Key] = value;
        }
    }
}
