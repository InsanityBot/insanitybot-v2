using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Helium.Commons.Logging;

using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Datafixers.Reference;

namespace InsanityBot.Datafixers.Main
{
    /*
     * Datafixer Identifier: 0.1.3
     * Datafixer Upgrade: 2.0.0-dev.00017 to 2.0.0-dev.00020
     * Datafixer Reason: Add a slowmode command, requiring a default time
    */
    public class Main0003_AddSlowmode : IDatafixer<MainConfiguration>
    {
        public String NewDataVersion { get => "2.0.0-dev.00020"; }

        public String OldDataVersion { get => "2.0.0-dev.00017"; }

        public UInt32 DatafixerId { get => 2; }

        public Boolean BreakingChange { get => false; }

        public DatafixerDowngradeResult DowngradeData(ref MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00017")
                return DatafixerDowngradeResult.AlreadyDowngraded;

            data.Configuration.Remove("insanitybot.commands.slowmode.default_slowmode");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 1, 3, 1, "Downgrade"), "Downgraded successfully to version 2.0.0-dev.00017");
            return DatafixerDowngradeResult.Success;
        }

        public MainConfiguration ExportDowngradedData(MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00017")
                return data;

            data.Configuration.Remove("insanitybot.commands.slowmode.default_slowmode");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 1, 3, 2, "ExportDowngrade"), "Downgraded successfully to version 2.0.0-dev.00017");
            return data;
        }

        public MainConfiguration ExportUpgradedData(MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00017")
                return data;

            data.Configuration.Add("insanitybot.commands.slowmode.default_slowmode", 5);
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 1, 3, 3, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00020");
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00017")
                return DatafixerUpgradeResult.AlreadyUpgraded;

            data.Configuration.Add("insanitybot.commands.slowmode.default_slowmode", 5);
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 1, 3, 4, "Upgrade"), "Upgraded successfully to version 2.0.0-dev.00020");
            return DatafixerUpgradeResult.Success;
        }
    }
}
