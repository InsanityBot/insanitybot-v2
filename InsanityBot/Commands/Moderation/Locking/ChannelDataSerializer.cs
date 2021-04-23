using System.Collections.Generic;
using System.IO;

using DSharpPlus.Entities;

using Newtonsoft.Json;

namespace InsanityBot.Commands.Moderation.Locking
{
    public static class ChannelDataSerializer
    {
        public static void SerializeChannelData(this DiscordChannel channel)
        {
            LockedChannelData data = new()
            {
                ChannelId = channel.Id,
                Overwrites = (List<DiscordOverwrite>)channel.PermissionOverwrites
            };

            if (!Directory.Exists($"./cache/lock"))
            {
                Directory.CreateDirectory($"./cache/lock");
            }

            if (!File.Exists($"./cache/lock/{channel.Id}"))
            {
                File.Create($"./cache/lock/{channel.Id}").Close();
            }

            StreamWriter writer = new($"./cache/lock/{channel.Id}");
            writer.Write(JsonConvert.SerializeObject(data));

            writer.Close();
        }

        public static List<DiscordOverwrite> GetChannelData(this DiscordChannel channel)
        {
            if (!File.Exists($"./cache/lock/{channel.Id}"))
            {
                return (List<DiscordOverwrite>)channel.PermissionOverwrites;
            }

            StreamReader reader = new($"./cache/lock/{channel.Id}");
            return JsonConvert.DeserializeObject<LockedChannelData>(reader.ReadToEnd()).Overwrites;
        }

        public static void SerializeLockingOverwrites(this DiscordChannel channel, ChannelData data)
        {
            if (!Directory.Exists($"./cache/lock"))
            {
                Directory.CreateDirectory($"./cache/lock");
            }

            if (!File.Exists($"./cache/lock/{channel.Id}.ibcd"))
            {
                File.Create($"./cache/lock/{channel.Id}.ibcd").Close();
            }

            StreamWriter writer = new($"./cache/lock/{channel.Id}.ibcd");
            writer.Write(JsonConvert.SerializeObject(data));

            writer.Close();
        }

        public static ChannelData GetCachedChannelData(this DiscordChannel channel)
        {
            if (!File.Exists($"./cache/lock/{channel.Id}.ibcd"))
            {
                return ChannelData.CreateNew();
            }

            StreamReader reader = new($"./cache/lock/{channel.Id}.ibcd");
            return JsonConvert.DeserializeObject<ChannelData>(reader.ReadToEnd());
        }
    }
}
