using Helium.Commons.Logging;

using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Datafixers.Reference;

using System;

namespace InsanityBot.Datafixers.Main
{
    /*
     * Datafixer Identifier: 0.1.4
     * Datafixer Upgrade: 2.0.0-dev.00020 to 2.0.0-dev.00032
     * Datafixer Reason: Multiple log channels
    */
    public class Main0004_AddAdminLogs : IDatafixer<MainConfiguration>
    {
        public String NewDataVersion => "2.0.0-dev.00032";

        public String OldDataVersion => "2.0.0-dev.00020";

        public UInt32 DatafixerId => 3;

        public Boolean BreakingChange => false;

        public DatafixerDowngradeResult DowngradeData(ref MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00020")
            {
                return DatafixerDowngradeResult.AlreadyDowngraded;
            }

            data.Configuration.Remove("insanitybot.identifiers.commands.admin_log_channel_id");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 1, 4, 1, "Downgrade"), "Downgraded successfully to version 2.0.0-dev.00020");
            return DatafixerDowngradeResult.Success;
        }

        public MainConfiguration ExportDowngradedData(MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00020")
            {
                return data;
            }

            data.Configuration.Remove("insanitybot.identifiers.commands.admin_log_channel_id");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 1, 4, 2, "ExportDowngrade"), "Downgraded successfully to version 2.0.0-dev.00020");
            return data;
        }

        public MainConfiguration ExportUpgradedData(MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00020")
            {
                return data;
            }

            data.Configuration.Add("insanitybot.identifiers.commands.admin_log_channel_id", 0);
            data.DataVersion = "2.0.0-dev.00032";

            DatafixerLogger.LogInformation(new EventData(0, 1, 4, 3, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00032");
            return data;
        }
        public DatafixerUpgradeResult UpgradeData(ref MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00020")
            {
                return DatafixerUpgradeResult.AlreadyUpgraded;
            }

            data.Configuration.Add("insanitybot.identifiers.commands.admin_log_channel_id", 0);
            data.DataVersion = "2.0.0-dev.00032";

            DatafixerLogger.LogInformation(new EventData(0, 1, 4, 3, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00032");
            return DatafixerUpgradeResult.Success;
        }
    }
}
