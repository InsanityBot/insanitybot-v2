using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Config;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static async Task HardReset()
        {
            //fully resets all bot configs, data files etc.
            Directory.Delete("./config", true);
            Directory.Delete("./data", true);
            await Initialize();
        }

        public static async Task Initialize()
        {
            //generates all bot configs and data files that dont exist yet
            if (!Directory.Exists("./config"))
                Directory.CreateDirectory("./config");

            if (!Directory.Exists("./data"))
                Directory.CreateDirectory("./data");

            if (!File.Exists("./config/main.json"))
            {
                File.Create("./config/main.json");
                await CreateMainConfig();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async Task CreateMainConfig()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ConfigManager.Config.Token = "";
            ConfigManager.Config.GuildId = 0;

            ConfigManager.AddConfigEntry("insanitybot.commands.prefixes", new List<String>().Append("i.").Append("admin."))
                .AddConfigEntry("insanitybot.commands.modules.moderation", true)
                .AddConfigEntry("insanitybot.commands.modules.permissions", true)
                .AddConfigEntry("insanitybot.commands.modules.suggestion", true)
                .AddConfigEntry("insanitybot.commands.modules.tags", true)
                .AddConfigEntry("insanitybot.commands.modules.tickets", true)
                .AddConfigEntry("insanitybot.commands.modules.admin", true)

                .AddConfigEntry("insanitybot.commands.help.send_dms", true)
                .AddConfigEntry("insanitybot.commands.moderation.allow_minor_warns", true)
                .AddConfigEntry("insanitybot.commands.moderation.minor_warns_equal_full_warn", 3)
                .AddConfigEntry("insanitybot.commands.moderation.default_mute_time", new TimeSpan(0, 30, 0))
                .AddConfigEntry("insanitybot.commands.moderation.default_ban_time", new TimeSpan(0, 30, 0))
                .AddConfigEntry("insanitybot.commands.suggestions.allow_community_denial", true)
                .AddConfigEntry("insanitybot.commands.suggestions.denial_by_downvote_percentage", false)
                .AddConfigEntry("insanitybot.commands.suggestions.percentage_for_community_denial", 0)
                .AddConfigEntry("insanitybot.commands.suggestions.downvotes_for_community_denial", 10)
                .AddConfigEntry("insanitybot.commands.suggestions.allow_community_acceptance", false)
                .AddConfigEntry("insanitybot.commands.suggestions.allow_by_upvote_percentage", false)
                .AddConfigEntry("insanitybot.commands.suggestions.percentage_for_community_acceptance", 0)
                .AddConfigEntry("insanitybot.commands.suggestions.upvotes_for_community_acceptance", 0)
                .AddConfigEntry("insanitybot.commands.suggestions.allow_forcible_denial", true)
                .AddConfigEntry("insanitybot.commands.suggestions.allow_forcible_acceptance", true)
                .AddConfigEntry("insanitybot.commands.tickets.use_presets", true)
                .AddConfigEntry("insanitybot.commands.tickets.presets.allow_create_command", true)
                .AddConfigEntry("insanitybot.commands.tickets.presets.allow_delete_command", true)
                .AddConfigEntry("insanitybot.commands.tickets.presets.allow_creation_by_ticket_identifier", true)

                .Serialize(ConfigManager.Config, "./config/main.json");
        }
    }
}
