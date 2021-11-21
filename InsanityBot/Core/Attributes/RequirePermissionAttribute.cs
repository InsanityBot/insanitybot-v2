namespace InsanityBot.Core.Attributes;
using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using global::InsanityBot.Utility.Permissions;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : CheckBaseAttribute
{
	internal String Permission { get; private set; }

	public RequirePermissionAttribute(String permission) => this.Permission = permission;

	public override Task<Boolean> ExecuteCheckAsync(CommandContext ctx, Boolean help) => Task.FromResult(ctx.Member.HasPermission(this.Permission));
}