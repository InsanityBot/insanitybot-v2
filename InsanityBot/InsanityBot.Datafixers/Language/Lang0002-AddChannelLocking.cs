using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public String NewDataVersion { get => "2.0.0-dev.00017"; }

        public String OldDataVersion { get => "2.0.0-dev.00016"; }

        public UInt32 DatafixerId { get => 1; }

        public Boolean BreakingChange { get => false; }

        public DatafixerDowngradeResult DowngradeData(ref LanguageConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00016")
                return DatafixerDowngradeResult.AlreadyDowngraded;

            data.Configuration.Remove("insanitybot.moderation.lock.success");
            data.Configuration.Remove("insanitybot.moderation.lock.failure");
            data.DataVersion = "2.0.0-dev.00016";

            DatafixerLogger.LogInformation(new EventData(0, 2, 2, 1, "Downgrade"), "Downgraded successfully to version 2.0.0-dev.00016");
            return DatafixerDowngradeResult.Success;
        }

        public LanguageConfiguration ExportDowngradedData(LanguageConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00016")
                return data;

            data.Configuration.Remove("insanitybot.moderation.lock.success");
            data.Configuration.Remove("insanitybot.moderation.lock.failure");
            data.DataVersion = "2.0.0-dev.00016";

            DatafixerLogger.LogInformation(new EventData(0, 2, 2, 2, "ExportDowngrade"), "Downgraded successfully to version 2.0.0-dev.00016");
            return data;
        }

        public LanguageConfiguration ExportUpgradedData(LanguageConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00016")
                return data;

            data.Configuration.Add("insanitybot.moderation.lock.success", "{CHANNEL} was locked successfully.");
            data.Configuration.Add("insanitybot.moderation.lock.failure", "{CHANNEL} could not be locked.");
            data.DataVersion = "2.0.0-dev.00017";

            DatafixerLogger.LogInformation(new EventData(0, 2, 2, 3, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00017");
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref LanguageConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00016")
                return DatafixerUpgradeResult.AlreadyUpgraded;

            data.Configuration.Add("insanitybot.moderation.lock.success", "{CHANNEL} was locked successfully.");
            data.Configuration.Add("insanitybot.moderation.lock.failure", "{CHANNEL} could not be locked.");
            data.DataVersion = "2.0.0-dev.00017";

            DatafixerLogger.LogInformation(new EventData(0, 2, 2, 4, "Upgrade"), "Upgraded successfully to version 2.0.0-dev.00017");
            return DatafixerUpgradeResult.AlreadyUpgraded;
        }
    }
}
