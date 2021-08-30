using System;

using InsanityBot.Commands.Miscellaneous;
using InsanityBot.Commands.Moderation.Locking;
using InsanityBot.Commands.Moderation.Modlog;
using InsanityBot.Commands.Moderation;
using InsanityBot.Commands.Permissions;
using InsanityBot.Tickets.Commands.Admin;
using InsanityBot.Tickets.Commands;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        private static void RegisterAllCommands()
        {
            CommandsExtension.RegisterCommands<PermissionCommand>();

            if(Config.Value<Boolean>("insanitybot.modules.miscellaneous"))
            {
                CommandsExtension.RegisterCommands<Say>();
                CommandsExtension.RegisterCommands<Embed>();
            }
            if(Config.Value<Boolean>("insanitybot.modules.moderation"))
            {
                CommandsExtension.RegisterCommands<VerbalWarn>();
                CommandsExtension.RegisterCommands<Warn>();
                CommandsExtension.RegisterCommands<Mute>();
                CommandsExtension.RegisterCommands<Blacklist>();
                CommandsExtension.RegisterCommands<Whitelist>();
                CommandsExtension.RegisterCommands<Kick>();
                CommandsExtension.RegisterCommands<Ban>();

                CommandsExtension.RegisterCommands<Modlog>();
                CommandsExtension.RegisterCommands<ExportModlog>();
                CommandsExtension.RegisterCommands<ClearModlog>();

                CommandsExtension.RegisterCommands<Purge>();
                CommandsExtension.RegisterCommands<Slowmode>();

                CommandsExtension.RegisterCommands<Lock>();
                CommandsExtension.RegisterCommands<Unlock>();
                CommandsExtension.RegisterCommands<LockHelperCommands>();
            }
            if(Config.Value<Boolean>("insanitybot.modules.tickets"))
            {
                CommandsExtension.RegisterCommands<NewTicketCommand>();
                CommandsExtension.RegisterCommands<CloseTicketCommand>();
                CommandsExtension.RegisterCommands<AddUserCommand>();
                CommandsExtension.RegisterCommands<RemoveUserCommand>();

                CommandsExtension.RegisterCommands<ClearTicketCache>();
            }
        }
    }
}
