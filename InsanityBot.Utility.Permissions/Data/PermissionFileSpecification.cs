namespace InsanityBot.Utility.Permissions.Data;
using System;

public record PermissionFileSpecification
{
	public PermissionFileNameConvention NameConvention { get; init; }
	public PermissionFileType PermissionFileType { get; init; }
	public String Path { get; init; }
	public Func<String> GetFilePath { get; init; } // must return the file path, with {ID} replacing the ID if needed.
}