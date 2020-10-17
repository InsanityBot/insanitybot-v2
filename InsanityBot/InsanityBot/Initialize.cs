using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Config;
using InsanityBot.Utility.Language;

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
        {
            ConfigManager.Config = new MainConfiguration();

            ConfigManager
                .AddConfigEntry("insanitybot.modules.logging", true)
                .AddConfigEntry("insanitybot.modules.miscellaneous", true)
                .AddConfigEntry("insanitybot.modules.moderation", true)
                .AddConfigEntry("insanitybot.modules.permissions", true)
                .AddConfigEntry("insanitybot.modules.suggestion", true)
                .AddConfigEntry("insanitybot.modules.tags", true)
                .AddConfigEntry("insanitybot.modules.tickets", true)
                .AddConfigEntry("insanitybot.modules.admin", true)
                .AddConfigEntry("insanitybot.modules.console", true)

                .AddConfigEntry("insanitybot.commands.prefixes", new List<String>().Append("i.").Append("admin."))
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
                .AddConfigEntry("insanitybot.commands.suggestions.block_role_pings", true)
                .AddConfigEntry("insanitybot.commands.suggestions.block_user_pings", true)
                .AddConfigEntry("insanitybot.commands.tickets.use_presets", true)
                .AddConfigEntry("insanitybot.commands.tickets.create_ticket_by_reaction", true)
                .AddConfigEntry("insanitybot.commands.tickets.presets.allow_create_command", true)
                .AddConfigEntry("insanitybot.commands.tickets.presets.allow_delete_command", true)
                .AddConfigEntry("insanitybot.commands.tickets.presets.allow_creation_by_preset_identifier", true)
                .AddConfigEntry("insanitybot.logging.log_message_delete", true)
                .AddConfigEntry("insanitybot.logging.log_message_edit", true)
                .AddConfigEntry("insanitybot.logging.log_member_join", true)
                .AddConfigEntry("insanitybot.logging.log_member_leave", true)
                .AddConfigEntry("insanitybot.logging.log_commands", false)
                .AddConfigEntry("insanitybot.logging.members.use_mentions", true)
                .AddConfigEntry("insanitybot.miscellaneous.join_roles", true)
                .AddConfigEntry("insanitybot.miscellaneous.block_say_role_pings", true)
                .AddConfigEntry("insanitybot.miscellaneous.block_say_user_pings", false)

                .AddConfigEntry("insanitybot.identifiers.commands.modlog_channel_id", 0)
                .AddConfigEntry("insanitybot.identifiers.commands.suggestion_channel_id", 0)
                .AddConfigEntry("insanitybot.identifiers.commands.accepted_suggestions_channel_id", 0)
                .AddConfigEntry("insanitybot.identifiers.commands.denied_suggestions_channel_id", 0)
                .AddConfigEntry("insanitybot.identifiers.commands.suggestion_upvote_emote_id", 0)
                .AddConfigEntry("insanitybot.identifiers.commands.suggestion_downvote_emote_id", 0)
                .AddConfigEntry("insanitybot.identifiers.commands.suggestion_accept_emote_id", 0)
                .AddConfigEntry("insanitybot.identifiers.commands.suggestion_deny_emote_id", 0)
                .AddConfigEntry("insanitybot.identifiers.commands.ticket_category_id", 0)
                .AddConfigEntry("insanitybot.identifiers.commands.ticket_reaction_message_id", 0)
                .AddConfigEntry("insanitybot.identifiers.commands.ticket_reaction_emote_id", 0)
                .AddConfigEntry("insanitybot.identifiers.logging.message_delete_log_channel_id", 0)
                .AddConfigEntry("insanitybot.identifiers.logging.message_edit_log_channel_id", 0)
                .AddConfigEntry("insanitybot.identifiers.logging.member_join_log_channel_id", 0)
                .AddConfigEntry("insanitybot.identifiers.logging.member_leave_log_channel_id", 0)
                .AddConfigEntry("insanitybot.identifiers.miscellaneous.join_role_ids", new List<Int64>().Append(0))

                .Serialize(ConfigManager.Config, "./config/main.json");
        }

        public static async Task CreateLangConfig()
        {
            LanguageManager.Config = new LanguageConfiguration();

            LanguageManager.AddConfigEntry("insanitybot.error.lacking_permission",
                "You are lacking permission to perform this command! Please contact your administrators if you believe this is an error.")
                .AddConfigEntry("insanitybot.error.generic",
                "The command failed to execute. Please retry or contact your administrators.")
                .AddConfigEntry("insanitybot.moderation.success",
                "{MENTION} was {OPERATION} successfully.")
                .AddConfigEntry("insanitybot.moderation.failure",
                "{MENTION} could not be {OPERATION}.")

                .Serialize(LanguageManager.Config, "./config/lang.json");
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
