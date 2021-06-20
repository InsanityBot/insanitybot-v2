using System;
using System.Collections.Generic;

namespace InsanityBot.Utility.Permissions.Data
{
	public abstract class PermissionBase
	{
		public Dictionary<String, PermissionValue> Permissions { get; set; }
		public Boolean IsAdministrator { get; set; }
		public UInt64 SnowflakeIdentifier { get; set; }
		public Guid UpdateGuid { get; set; }

		public virtual PermissionValue this[String key]
		{
			get => this.Permissions[key];
			set => this.Permissions[key] = value;
		}

		public PermissionBase()
		{
			this.Permissions = new();
			this.SnowflakeIdentifier = 0;
			this.IsAdministrator = false;
			this.UpdateGuid = new(new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
		}
	}
}
