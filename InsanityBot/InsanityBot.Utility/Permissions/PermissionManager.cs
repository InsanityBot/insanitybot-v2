using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

using InsanityBot.Utility.Permissions.Reference;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions
{
    internal static class PermissionManager
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable IDE0060
        internal static async Task RemoveGuildMember(DiscordClient sender, GuildMemberRemoveEventArgs e)
        {
            Directory.Delete($"./data/{e.Member.Id}");
        }

        internal static async Task UpdateGuildMember(DiscordClient sender, GuildMemberUpdateEventArgs e)
        {

        }

        internal static void GeneratePermissionFile(UInt64 Identifier, PermissionFileType type)
        {
            if (type == PermissionFileType.User && File.Exists($"./data/{Identifier}/permissions.json"))
                return;
            if (type == PermissionFileType.Role && File.Exists($"./data/permissions/{Identifier}.json"))
                return;

            StreamWriter writer;
            if(type == PermissionFileType.User)
            {
                if (!Directory.Exists($"./data/{Identifier}"))
                    Directory.CreateDirectory($"./data/{Identifier}");
                writer = new StreamWriter(File.Create($"./data/{Identifier}/permissions.json"));

                writer.Write(JsonConvert.SerializeObject(new UserPermissions(Identifier), Formatting.Indented));
                writer.Close();
                return;
            }
            if(type == PermissionFileType.Role)
            {
                if (!Directory.Exists($"./data/permissions"))
                    Directory.CreateDirectory($"./data/permissions");
                writer = new StreamWriter(File.Create($"./data/permissions/{Identifier}.json"));

                writer.Write(JsonConvert.SerializeObject(new RolePermissions(Identifier), Formatting.Indented));
                writer.Close();
                return;
            }
        }
#pragma warning restore IDE0060
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
