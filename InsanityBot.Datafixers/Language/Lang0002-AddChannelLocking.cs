using System;

using Helium.Commons.Logging;

using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Datafixers.Reference;
using InsanityBot.Utility.Language;

namespace InsanityBot.Datafixers.Language
{
    /*
     * Datafixer Identifier: 0.2.2
     * Datafixer Upgrade: dev.00016 to dev.00017
     * Datafixer Reason: Addition of channel locking, requiring new embeds
    */
    public class Lang0002_AddChannelLocking : IDatafixer<LanguageConfiguration>
    {
        public String NewDataVersion => "2.0.0-dev.00017";

        public String OldDataVersion => "2.0.0-dev.00016";

        public UInt32 DatafixerId => 1;

        public Boolean BreakingChange => false;

        public DatafixerDowngradeResult DowngradeData(ref LanguageConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00016")
            {
                return DatafixerDowngradeResult.AlreadyDowngraded;
            }

            data.Configuration.Remove("insanitybot.error.lacking_admin_permission");
            data.Configuration.Remove("insanitybot.moderation.lock.success");
            data.Configuration.Remove("insanitybot.moderation.lock.failure");
            data.Configuration.Remove("insanitybot.moderation.unlock.success");
            data.Configuration.Remove("insanitybot.moderation.unlock.failure");
            data.Configuration.Remove("insanitybot.commands.lock.whitelist.add.success");
            data.Configuration.Remove("insanitybot.commands.lock.whitelist.add.failure");
            data.Configuration.Remove("insanitybot.commands.lock.whitelist.add.already_present");
            data.Configuration.Remove("insanitybot.commands.lock.whitelist.remove.success");
            data.Configuration.Remove("insanitybot.commands.lock.whitelist.remove.failure");
            data.Configuration.Remove("insanitybot.commands.lock.blacklist.add.success");
            data.Configuration.Remove("insanitybot.commands.lock.blacklist.add.failure");
            data.Configuration.Remove("insanitybot.commands.lock.blacklist.add.already_present");
            data.Configuration.Remove("insanitybot.commands.lock.blacklist.remove.success");
            data.Configuration.Remove("insanitybot.commands.lock.blacklist.remove.failure");
            data.DataVersion = "2.0.0-dev.00016";

            DatafixerLogger.LogInformation(new EventData(0, 2, 2, 1, "Downgrade"), "Downgraded successfully to version 2.0.0-dev.00016");
            return DatafixerDowngradeResult.Success;
        }

        public LanguageConfiguration ExportDowngradedData(LanguageConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00016")
            {
                return data;
            }

            data.Configuration.Remove("insanitybot.error.lacking_admin_permission");
            data.Configuration.Remove("insanitybot.moderation.lock.success");
            data.Configuration.Remove("insanitybot.moderation.lock.failure");
            data.Configuration.Remove("insanitybot.moderation.unlock.success");
            data.Configuration.Remove("insanitybot.moderation.unlock.failure");
            data.Configuration.Remove("insanitybot.commands.lock.whitelist.add.success");
            data.Configuration.Remove("insanitybot.commands.lock.whitelist.add.failure");
            data.Configuration.Remove("insanitybot.commands.lock.whitelist.add.already_present");
            data.Configuration.Remove("insanitybot.commands.lock.whitelist.remove.success");
            data.Configuration.Remove("insanitybot.commands.lock.whitelist.remove.failure");
            data.Configuration.Remove("insanitybot.commands.lock.blacklist.add.success");
            data.Configuration.Remove("insanitybot.commands.lock.blacklist.add.failure");
            data.Configuration.Remove("insanitybot.commands.lock.blacklist.add.already_present");
            data.Configuration.Remove("insanitybot.commands.lock.blacklist.remove.success");
            data.Configuration.Remove("insanitybot.commands.lock.blacklist.remove.failure");

            data.DataVersion = "2.0.0-dev.00016";

            DatafixerLogger.LogInformation(new EventData(0, 2, 2, 2, "ExportDowngrade"), "Downgraded successfully to version 2.0.0-dev.00016");
            return data;
        }

        public LanguageConfiguration ExportUpgradedData(LanguageConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00016")
            {
                return data;
            }

            data.Configuration.Add("insanitybot.error.lacking_admin_permission", "You cannot execute this command as you are not a server administrator!");
            data.Configuration.Add("insanitybot.moderation.lock.success", "{CHANNEL} was locked successfully.");
            data.Configuration.Add("insanitybot.moderation.lock.failure", "{CHANNEL} could not be locked.");
            data.Configuration.Add("insanitybot.moderation.unlock.success", "{CHANNEL} was unlocked successfully.");
            data.Configuration.Add("insanitybot.moderation.unlock.failure", "{CHANNEL} could not be unlocked.");
            data.Configuration.Add("insanitybot.commands.lock.whitelist.add.success", "{ROLE} was added successfully to the whitelist for {CHANNEL}");
            data.Configuration.Add("insanitybot.commands.lock.whitelist.add.failure", "{ROLE} could not be added to the whitelist for {CHANNEL}");
            data.Configuration.Add("insanitybot.commands.lock.whitelist.add.already_present", "{ROLE} was already whitelisted in {CHANNEL}");
            data.Configuration.Add("insanitybot.commands.lock.whitelist.remove.success", "{ROLE} was removed successfully from the whitelist for {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.whitelist.remove.failure", "{ROLE} could not be removed from the whitelist for {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.blacklist.add.success", "{ROLE} was added successfully to the blacklist for {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.blacklist.add.failure", "{ROLE} could not be added to the blacklist for {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.blacklist.add.already_present", "{ROLE} was already blacklisted in {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.blacklist.remove.success", "{ROLE} was removed successfully from the blacklist for {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.blacklist.remove.failure", "{ROLE} could not be removed from the blacklist for {CHANNEL}.");
            data.DataVersion = "2.0.0-dev.00017";

            DatafixerLogger.LogInformation(new EventData(0, 2, 2, 3, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00017");
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref LanguageConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00016")
            {
                return DatafixerUpgradeResult.AlreadyUpgraded;
            }

            data.Configuration.Add("insanitybot.moderation.lock.success", "{CHANNEL} was locked successfully.");
            data.Configuration.Add("insanitybot.moderation.lock.failure", "{CHANNEL} could not be locked.");
            data.Configuration.Add("insanitybot.moderation.unlock.success", "{CHANNEL} was unlocked successfully.");
            data.Configuration.Add("insanitybot.moderation.unlock.failure", "{CHANNEL} could not be unlocked.");
            data.Configuration.Add("insanitybot.commands.lock.whitelist.remove.success", "{ROLE} was removed successfully from the whitelist for {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.whitelist.remove.failure", "{ROLE} could not be removed from the whitelist for {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.blacklist.add.success", "{ROLE} was added successfully to the blacklist for {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.blacklist.add.failure", "{ROLE} could not be added to the blacklist for {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.blacklist.add.already_present", "{ROLE} was already blacklisted in {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.blacklist.remove.success", "{ROLE} was removed successfully from the blacklist for {CHANNEL}.");
            data.Configuration.Add("insanitybot.commands.lock.blacklist.remove.failure", "{ROLE} could not be removed from the blacklist for {CHANNEL}.");
            data.DataVersion = "2.0.0-dev.00017";

            DatafixerLogger.LogInformation(new EventData(0, 2, 2, 4, "Upgrade"), "Upgraded successfully to version 2.0.0-dev.00017");
            return DatafixerUpgradeResult.AlreadyUpgraded;
        }
    }
}
