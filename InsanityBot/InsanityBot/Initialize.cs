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
            ConfigManager.Config = new MainConfiguration
            {
                Prefixes = new List<String>().Append("i.").Append("admin.").ToList()
            };

            ConfigManager
                .AddConfigEntry("insanitybot.modules.experience", true)
                .AddConfigEntry("insanitybot.modules.logging", true)
                .AddConfigEntry("insanitybot.modules.miscellaneous", true)
                .AddConfigEntry("insanitybot.modules.moderation", true)
                .AddConfigEntry("insanitybot.modules.permissions", true)
                .AddConfigEntry("insanitybot.modules.suggestion", true)
                .AddConfigEntry("insanitybot.modules.tags", true)
                .AddConfigEntry("insanitybot.modules.tickets", true)
                .AddConfigEntry("insanitybot.modules.admin", true)
                .AddConfigEntry("insanitybot.modules.console", true)

                .AddConfigEntry("insanitybot.commands.help.send_dms", true)
                .AddConfigEntry("insanitybot.commands.moderation.allow_minor_warns", true)
                .AddConfigEntry("insanitybot.commands.moderation.convert_minor_warns_into_full_warn", true)
                .AddConfigEntry("insanitybot.commands.moderation.minor_warns_equal_full_warn", 3)
                .AddConfigEntry("insanitybot.commands.moderation.default_mute_time", new TimeSpan(0, 30, 0))
                .AddConfigEntry("insanitybot.commands.moderation.default_ban_time", new TimeSpan(0, 30, 0))
                .AddConfigEntry("insanitybot.commands.modlog.max_modlog_entries_per_embed", 25)
                .AddConfigEntry("insanitybot.commands.modlog.max_verballog_entries_per_embed", 20)
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
                .AddConfigEntry("insanitybot.identifiers.moderation.mute_role_id", 0)
                .AddConfigEntry("insanitybot.identifiers.moderation.blacklist_role_id", 0)
                .AddConfigEntry("insanitybot.tcp_port", 0)

                .Serialize(ConfigManager.Config, "./config/main.json");
        }

        public static async Task CreateLangConfig()
        {
            LanguageManager.Config = new LanguageConfiguration();

            LanguageManager.AddConfigEntry("insanitybot.error.lacking_permission",
                "You are lacking permission to perform this command! Please contact your administrators if you believe this is an error.")
                .AddConfigEntry("insanitybot.error.generic", "The command failed to execute. Please retry or contact your administrators.")

                .AddConfigEntry("insanitybot.moderation.warn.success", "{MENTION} was warned successfully.")
                .AddConfigEntry("insanitybot.moderation.unwarn.success", "{MENTION} was unwarned successfully")
                .AddConfigEntry("insanitybot.moderation.mute.success", "{MENTION} was muted successfully.")
                .AddConfigEntry("insanitybot.moderation.unmute.success", "{MENTION} was unmuted successfully.")
                .AddConfigEntry("insanitybot.moderation.blacklist.success", "{MENTION} was blacklisted successfully.")
                .AddConfigEntry("insanitybot.moderation.whitelist.success", "{MENTION} was whitelisted successfully.")
                .AddConfigEntry("insanitybot.moderation.kick.success", "{MENTION} was kicked successfully.")
                .AddConfigEntry("insanitybot.moderation.ban.success", "{MENTION} was banned successfully.")
                .AddConfigEntry("insanitybot.moderation.unban.success", "{ID} was unbanned successfully.")

                .AddConfigEntry("insanitybot.moderation.verbal_warn.failure", "{MENTION} could not be verbally warned.")
                .AddConfigEntry("insanitybot.moderation.unwarn.failure", "{MENTION} could not be unwarned.")
                .AddConfigEntry("insanitybot.moderation.warn.failure", "{MENTION} could not be warned.")
                .AddConfigEntry("insanitybot.moderation.mute.failure", "{MENTION} could not be muted.")
                .AddConfigEntry("insanitybot.moderation.unmute.failure", "{MENTION} could not be unmuted.")
                .AddConfigEntry("insanitybot.moderation.blacklist.failure", "{MENTION} could not be blacklisted.")
                .AddConfigEntry("insanitybot.moderation.whitelist.failure", "{MENTION} could not be whitelisted.")
                .AddConfigEntry("insanitybot.moderation.kick.failure", "{MENTION} could not be kicked.")
                .AddConfigEntry("insanitybot.moderation.ban.failure", "{MENTION} could not be banned.")
                .AddConfigEntry("insanitybot.moderation.unban.failure", "{ID} could not be unbanned.")

                .AddConfigEntry("insanitybot.moderation.verbal_warn.reason", "{MENTION}, {REASON}")
                .AddConfigEntry("insanitybot.moderation.warn.reason", "You were warned for {REASON}.")
                .AddConfigEntry("insanitybot.moderation.mute.reason", "You were muted for {REASON}.")
                .AddConfigEntry("insanitybot.moderation.unmute.reason", "You were unmuted!")
                .AddConfigEntry("insanitybot.moderation.blacklist.reason", "You were blacklisted for {REASON}.")
                .AddConfigEntry("insanitybot.moderation.whitelist.reason", "You were whitelisted!")
                .AddConfigEntry("insanitybot.moderation.kick.reason", "You were kicked for {REASON}.")
                .AddConfigEntry("insanitybot.moderation.ban.reason", "You were banned for {REASON}.")

                .AddConfigEntry("insanitybot.moderation.no_reason_given", "No reason given")

                .AddConfigEntry("insanitybot.commands.modlog.embed_title", "Modlog of {USERNAME}")
                .AddConfigEntry("insanitybot.commands.modlog.empty_modlog", ":white_check_mark: This member has no modlog entries")
                .AddConfigEntry("insanitybot.commands.modlog.overflow", "... and more entries")
                .AddConfigEntry("insanitybot.commands.modlog.failed", "Could not retrieve modlog for {MENTION}")
                .AddConfigEntry("insanitybot.commands.verbal_log.embed_title", "Verbal Log of {USERNAME}")
                .AddConfigEntry("insanitybot.commands.verbal_log.empty_modlog", ":white_check_mark: This member has no verbal log entries")
                .AddConfigEntry("insanitybot.commands.verbal_log.overflow", "... and more entries")
                .AddConfigEntry("insanitybot.commands.verbal_log.failed", "Could not retrieve verbal log for {MENTION}")

                .Serialize(LanguageManager.Config, "./config/lang.json");
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
