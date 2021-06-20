using System;

using CommandLine;

namespace InsanityBot.Commands.Permissions
{
	public class RoleOptions
	{
		[Option('s', "silent", Default = false, Required = false)]
		public Boolean Silent { get; set; }

		[Option('r', "role", Required = true)]
		public UInt64 RoleId { get; set; }
	}
}
