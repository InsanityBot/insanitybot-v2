using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

using InsanityBot.Utility.Permissions.Serialization;

using Newtonsoft.Json;

#pragma warning disable CS1998 // these async methods dont technically need to run async, its just the API requiring them to be marked as async

namespace InsanityBot.Utility.Permissions
{
    public static class DiscordClientBinding
    {
        public static void InitializePermissionFramework(this DiscordClient client)
        {
            client.GuildMemberAdded += GuildMemberAdded;
            client.GuildMemberRemoved += GuildMemberRemoved;
            client.GuildMemberUpdated += GuildMemberUpdated;
        }

        private static async Task GuildMemberUpdated(GuildMemberUpdateEventArgs e)
        {
            if (e.RolesBefore == e.RolesAfter)
                return;

            if(e.Member.PermissionsIn(e.Guild.GetDefaultChannel()).HasPermission(DSharpPlus.Permissions.KickMembers))
            {
                UserPermission permission = e.Member.GetPermissions();
                permission.Permissions.VerbalWarn = true;
                permission.Permissions.Warn = true;
                permission.Permissions.Unwarn = true;
                permission.Permissions.Mute = true;
                permission.Permissions.Tempmute = true;
                permission.Permissions.Unmute = true;
                permission.Permissions.Blacklist = true;
                permission.Permissions.Whitelist = true;
                permission.Permissions.Kick = true;
                permission.Permissions.Slowmode = true;
            }

            if(e.Member.PermissionsIn(e.Guild.GetDefaultChannel()).HasPermission(DSharpPlus.Permissions.BanMembers))
            {
                UserPermission permission = e.Member.GetPermissions();
                permission.Permissions.Ban = true;
                permission.Permissions.Tempban = true;
                permission.Permissions.Unban = true;
                permission.Permissions.Lock = true;
                permission.Permissions.Unlock = true;
            }

            if(e.Member.PermissionsIn(e.Guild.GetDefaultChannel()).HasPermission(DSharpPlus.Permissions.Administrator))
            {
                UserPermission permission = e.Member.GetPermissions();
                permission.Permissions.SuggestionAccept = true;
                permission.Permissions.SuggestionDeny = true;
                permission.Permissions.TicketApplyAccept = true;
                permission.Permissions.TicketApplyDeny = true;

                permission.Permissions.Archive = true;
                permission.Permissions.Config = true;
                permission.Permissions.Permission = true;
            }
        }

        private static async Task GuildMemberRemoved(GuildMemberRemoveEventArgs e)
        {
            Directory.Delete($"./data/{e.Member.Id}", true);
        }

        private static async Task GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            Directory.CreateDirectory($"./data/{e.Member.Id}");
            File.Create($"./data/{e.Member.Id}/permissions.json");

            FileStream file = new FileStream($"./data/{e.Member.Id}/permissions.json", FileMode.Truncate);
            StreamWriter writer = new StreamWriter(file);

            Serialization.Reference.Permissions permissions = new Serialization.Reference.Permissions();

            await writer.WriteAsync(JsonConvert.SerializeObject(permissions));
        }
    }
}
