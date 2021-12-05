namespace InsanityBot.Utility.Permissions.Data;

public static class DefaultPermissionFileSpecifications
{
	public static readonly PermissionFileSpecification User = new()
	{
		NameConvention = PermissionFileNameConvention.Permissions,
		Path = "./data/{USERID}/",
		PermissionFileType = PermissionFileType.Permissions,
		GetFilePath = () =>
		{
			return "./data/{ID}/permissions.json";
		}
	};

	public static readonly PermissionFileSpecification Default = new()
	{
		NameConvention = PermissionFileNameConvention.Default,
		Path = "./config/permissions/",
		PermissionFileType = PermissionFileType.Permissions,
		GetFilePath = () =>
		{
			return "./config/permissions/default.json";
		}
	};

	public static readonly PermissionFileSpecification Role = new()
	{
		NameConvention = PermissionFileNameConvention.Role,
		Path = "./data/role-permissions/",
		PermissionFileType = PermissionFileType.Permissions,
		GetFilePath = () =>
		{
			return "./data/role-permissions/{ID}.json";
		}
	};

	public static readonly PermissionFileSpecification Script = new()
	{
		NameConvention = PermissionFileNameConvention.Scripts,
		Path = "./cache/scripts/compiled/",
		PermissionFileType = PermissionFileType.Declaration,
		GetFilePath = () =>
		{
			if(PermissionSettings.PrecompiledScripts)
			{
				return "./cache/scripts/compiled/decl.json";
			}
			else
			{
				return "./data/scripts/decl.json";
			}
		}
	};
}