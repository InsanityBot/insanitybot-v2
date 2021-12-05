namespace InsanityBot.Utility.Permissions.Data;
using System;

public static class PermissionSettings
{
	public static Boolean UpdateUserPermissions { get; internal set; }
	public static Boolean UpdateRolePermissions { get; internal set; }
	public static Boolean PrecompiledScripts { get; internal set; }
}