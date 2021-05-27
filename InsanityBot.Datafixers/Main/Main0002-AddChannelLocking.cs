using Helium.Commons.Logging;

using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Datafixers.Reference;

using System;

namespace InsanityBot.Datafixers.Main
{
    /*
     * Datafixer Identifier: 0.1.2
     * Datafixer Upgrade: 2.0.0-dev.00016 to 2.0.0-dev.00017
     * Datafixer Reason: Add channel locking, requiring an exempt role ID
    */
    public class Main0002_AddChannelLocking : IDatafixer<MainConfiguration>
    {
        public String NewDataVersion => "2.0.0-dev.00017";

        public String OldDataVersion => "2.0.0-dev.00016";

        public UInt32 DatafixerId => 1;

        public Boolean BreakingChange => false;

        public DatafixerDowngradeResult DowngradeData(ref MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00016")
            {
                return DatafixerDowngradeResult.AlreadyDowngraded;
            }

            data.Configuration.Remove("insanitybot.identifiers.moderation.lock_exempt_role_id");
            data.DataVersion = "2.0.0-dev.00016";

            DatafixerLogger.LogInformation(new EventData(0, 1, 2, 1, "Downgrade"), "Downgraded successfully to version 2.0.0-dev.00016");
            return DatafixerDowngradeResult.Success;
        }

        public MainConfiguration ExportDowngradedData(MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00016")
            {
                return data;
            }

            data.Configuration.Remove("insanitybot.identifiers.moderation.lock_exempt_role_id");
            data.DataVersion = "2.0.0-dev.00016";

            DatafixerLogger.LogInformation(new EventData(0, 1, 2, 2, "ExportDowngrade"), "Downgraded successfully to version 2.0.0-dev.00016");
            return data;
        }

        public MainConfiguration ExportUpgradedData(MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00016")
            {
                return data;
            }

            data.Configuration.Add("insanitybot.identifiers.moderation.lock_exempt_role_id", 0);
            data.DataVersion = "2.0.0-dev.00017";

            DatafixerLogger.LogInformation(new EventData(0, 1, 2, 3, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00017");
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00016")
            {
                return DatafixerUpgradeResult.AlreadyUpgraded;
            }

            data.Configuration.Add("insanitybot.identifiers.moderation.lock_exempt_role_id", 0);
            data.DataVersion = "2.0.0-dev.00017";

            DatafixerLogger.LogInformation(new EventData(0, 1, 2, 4, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00017");
            return DatafixerUpgradeResult.Success;
        }
    }
}
