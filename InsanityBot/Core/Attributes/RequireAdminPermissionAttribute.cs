using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using InsanityBot.Utility.Permissions;

namespace InsanityBot.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequireAdminPermissionAttribute : CheckBaseAttribute
    {
        internal String Permission { get; private set; }

        public RequireAdminPermissionAttribute(String permission)
        {
            this.Permission = permission;
        }

        public override Task<Boolean> ExecuteCheckAsync(CommandContext ctx, Boolean help)
        {
            return Task.FromResult(ctx.Member.HasPermission(Permission));
        }
    }
}