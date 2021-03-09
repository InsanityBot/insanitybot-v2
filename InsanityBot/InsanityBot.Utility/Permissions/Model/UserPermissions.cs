using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emzi0767;

namespace InsanityBot.Utility.Permissions.Model
{
    public class UserPermissions : PermissionBase
    {
        public override UInt64 SnowflakeIdentifier
        {
            get => base.SnowflakeIdentifier;
            set
            {
                if (value < 0 || value.CalculateLength() < 16)
                    throw new ArgumentException($"Invalid user snowflake identifier: {value}", nameof(value));
                base.SnowflakeIdentifier = value;
            }
        }
    }
}
