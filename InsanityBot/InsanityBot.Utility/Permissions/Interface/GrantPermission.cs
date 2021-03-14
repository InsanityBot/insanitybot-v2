using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Controller;
using InsanityBot.Utility.Permissions.Model;

namespace InsanityBot.Utility.Permissions.Interface
{
    public static class GrantPermission
    {
        public static void GrantMemberPermission(this DiscordMember member, params String[] permission)
        {
            UserPermissions permissions = UserPermissionSerializer.Deserialize(member.Id);

            List<String> resolved = permission.ResolveWildcardPermissions();

            foreach(var v in resolved)
            {
                try
                {
                    permissions[v] = true;
                }
                catch(Exception e)
                {
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                }
            }

            UserPermissionSerializer.Serialize(permissions);
        }

        public static void RevokeMemberPermission(this DiscordMember member, params String[] permission)
        {
            UserPermissions permissions = UserPermissionSerializer.Deserialize(member.Id);

            List<String> resolved = permission.ResolveWildcardPermissions();

            foreach (var v in resolved)
            {
                try
                {
                    permissions[v] = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                }
            }

            UserPermissionSerializer.Serialize(permissions);
        }
    }
}
