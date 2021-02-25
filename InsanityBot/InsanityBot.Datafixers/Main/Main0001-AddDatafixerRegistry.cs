using System;
using System.Collections.Generic;
using System.Text;

using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Datafixers.Reference;

namespace InsanityBot.Datafixers.Main
{
    public class Main0001_AddDatafixerRegistry : IDatafixer<MainConfiguration>
    {
        public String NewDataVersion { get => "2.0.0-dev.00016"; }

        public String OldDataVersion { get => "2.0.0-dev.00014"; }

        public UInt32 DatafixerId { get => 0; }

        public Boolean BreakingChange { get => false; }

        public DatafixerDowngradeResult DowngradeData(ref MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00014")
                return DatafixerDowngradeResult.AlreadyDowngraded;

            data.Configuration.Remove("insanitybot.datafixers.registry_mode");
            data.DataVersion = "2.0.0-dev.00014";
            return DatafixerDowngradeResult.Success;
        }

        public MainConfiguration ExportDowngradedData(MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00014")
                return data;

            data.Configuration.Remove("insanitybot.datafixers.registry_mode");
            data.DataVersion = "2.0.0-dev.00014";
            return data;
        }

        public MainConfiguration ExportUpgradedData(MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00014")
                return data;

            data.Configuration.Add("insanitybot.datafixers.registry_mode", 0);
            data.DataVersion = "2.0.0-dev.00016";
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00014")
                return DatafixerUpgradeResult.AlreadyUpdated;

            data.Configuration.Add("insanitybot.datafixers.registry_mode", 0);
            data.DataVersion = "2.0.0-dev.00016";
            return DatafixerUpgradeResult.Success;
        }
    }
}
