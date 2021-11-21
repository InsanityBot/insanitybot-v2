namespace InsanityBot.Utility.Permissions;
using System;

public record PermissionConfiguration
{
	public Boolean UpdateUserPermissions { get; init; }
	public Boolean UpdateRolePermissions { get; init; }
	public Boolean PrecompiledScripts { get; init; }
}