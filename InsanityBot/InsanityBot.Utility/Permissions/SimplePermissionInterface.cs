using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Interface;

namespace InsanityBot.Utility.Permissions
{
    public static class SimplePermissionInterface // we keep this for convenience only. complex requests should use InsanityBot.Utility.Permissions.Interface
    {
        /// <summary>
        /// Obtains the value of a certain permission from a member
        /// </summary>
        /// <param name="permission">String ID of the requested permission. Can be omitted if requiresAdmin is true</param>
        /// <param name="requiresAdmin">Asks whether the user has the administrator permission</param>
        /// <returns>true if the user is authorized, false if they arent.</returns>
        /// <exception cref="ArgumentException">Thrown if permission is null but requiresAdmin is true, alternatively also thrown if the permission does not exist</exception>
        public static Boolean HasPermission(this DiscordMember member, String permission = null, Boolean requiresAdmin = false)
        {
            if (permission == "admin" || permission == "administrator")
                return member.HasAdministrator();

            if (requiresAdmin)
                return member.HasAdministrator();

            if (permission.StartsWith("script"))
                return member.CheckScriptPermission(permission);
            return member.CheckUserPermission(permission);
        }
    }
}
