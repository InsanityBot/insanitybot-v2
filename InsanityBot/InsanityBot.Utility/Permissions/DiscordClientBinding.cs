using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using InsanityBot.Utility.Permissions.Reference;

using Newtonsoft.Json;

#pragma warning disable CS1998 // these async methods dont technically need to run async, its just the API requiring them to be marked as async

namespace InsanityBot.Utility.Permissions
{
    public static class DiscordClientBinding
    {
        public static void InitializePermissionFramework(this DiscordClient client)
        {
            client.GuildMemberRemoved += GuildMemberRemoved;
            client.GuildMemberUpdated += GuildMemberUpdated;
        }

        private static async Task GuildMemberUpdated(DiscordClient client, GuildMemberUpdateEventArgs e)
        {
            List<DiscordRole> RolesAdded = (from v in e.RolesAfter
                                            where !e.RolesBefore.Contains(v)
                                            select v)
                                            .ToList();

            List<DiscordRole> RolesRemoved = (from v in e.RolesBefore
                                              where !e.RolesAfter.Contains(v)
                                              select v)
                                              .ToList();
        }

        private static async Task GuildMemberRemoved(DiscordClient client, GuildMemberRemoveEventArgs e)
        {
            Directory.Delete($"./data/{e.Member.Id}", true);
        }
    }
}
