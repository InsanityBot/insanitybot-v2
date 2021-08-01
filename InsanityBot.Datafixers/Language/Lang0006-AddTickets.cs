using Helium.Commons.Logging;

using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Datafixers.Reference;
using InsanityBot.Utility.Language;

using System;

namespace InsanityBot.Datafixers.Language
{
    public class Lang0006_AddTickets : IDatafixer<LanguageConfiguration>
    {
        public String NewDataVersion => "2.0.0-dev.00035";

        public String OldDataVersion => "2.0.0-dev.00022";

        public UInt32 DatafixerId => 5;

        public Boolean BreakingChange => false;

        public DatafixerDowngradeResult DowngradeData(ref LanguageConfiguration data)
        {
            if(data.DataVersion == "2.0.0-dev.00022")
            {
                return DatafixerDowngradeResult.AlreadyDowngraded;
            }

            data.Configuration.Remove("insanitybot.tickets.new");
            data.Configuration.Remove("insanitybot.tickets.close");
            data.Configuration.Remove("insanitybot.tickets.cancelled_closing");
            data.Configuration.Remove("insanitybot.tickets.add_user.not_a_ticket_channel");
            data.Configuration.Remove("insanitybot.tickets.remove_user.not_a_ticket_channel");
            data.DataVersion = "2.0.0-dev.00022";

            DatafixerLogger.LogInformation(new EventData(0, 2, 6, 1, "Downgrade"), "Downgraded successfully to version 2.0.0-dev.00022");
            return DatafixerDowngradeResult.Success;
        }

        public LanguageConfiguration ExportDowngradedData(LanguageConfiguration data)
        {
            if(data.DataVersion == "2.0.0-dev.00022")
            {
                return data;
            }

            data.Configuration.Remove("insanitybot.tickets.new");
            data.Configuration.Remove("insanitybot.tickets.close");
            data.Configuration.Remove("insanitybot.tickets.cancelled_closing");
            data.Configuration.Remove("insanitybot.tickets.add_user.not_a_ticket_channel");
            data.Configuration.Remove("insanitybot.tickets.remove_user.not_a_ticket_channel");
            data.DataVersion = "2.0.0-dev.00022";

            DatafixerLogger.LogInformation(new EventData(0, 2, 6, 2, "ExportDowngrade"), "Downgraded successfully to version 2.0.0-dev.00022");
            return data;
        }

        public LanguageConfiguration ExportUpgradedData(LanguageConfiguration data)
        {
            if(data.DataVersion != "2.0.0-dev.00022")
            {
                return data;
            }

            data.Configuration.Add("insanitybot.tickets.new", ":white_check_mark: Successfully created ticket {TICKETCHANNEL}.");
            data.Configuration.Add("insanitybot.tickets.close", "{TICKETCHANNEL} will be closed in {TICKETCLOSETIME}.");
            data.Configuration.Add("insanitybot.tickets.cancelled_closing", "Cancelled closing of {CHANNEL}.");
            data.Configuration.Add("insanitybot.tickets.add_user.not_a_ticket_channel", "{CHANNEL} is not a ticket channel, you cannot add users here.");
            data.Configuration.Add("insanitybot.tickets.remove_user.not_a_ticket_channel", "{CHANNEL} is not a ticket channel, you cannot remove users from here.");
            data.DataVersion = "2.0.0-dev.00035";

            DatafixerLogger.LogInformation(new EventData(0, 2, 6, 3, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00035");
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref LanguageConfiguration data)
        {
            if(data.DataVersion != "2.0.0-dev.00022")
            {
                return DatafixerUpgradeResult.AlreadyUpgraded;
            }

            data.Configuration.Add("insanitybot.tickets.new", ":white_check_mark: Successfully created ticket {TICKETCHANNEL}.");
            data.Configuration.Add("insanitybot.tickets.close", "{TICKETCHANNEL} will be closed in {TICKETCLOSETIME}.");
            data.Configuration.Add("insanitybot.tickets.cancelled_closing", "Cancelled closing of {CHANNEL}.");
            data.Configuration.Add("insanitybot.tickets.add_user.not_a_ticket_channel", "{CHANNEL} is not a ticket channel, you cannot add users here.");
            data.Configuration.Add("insanitybot.tickets.remove_user.not_a_ticket_channel", "{CHANNEL} is not a ticket channel, you cannot remove users from here.");
            data.DataVersion = "2.0.0-dev.00035";

            DatafixerLogger.LogInformation(new EventData(0, 2, 6, 4, "Upgrade"), "Upgraded successfully to version 2.0.0-dev.00035");
            return DatafixerUpgradeResult.Success;
        }
    }
}
