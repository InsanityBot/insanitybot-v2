using System;
using System.Collections.Generic;
using System.Text;

using Helium.Commons.Logging;

using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Datafixers.Reference;

namespace InsanityBot.Datafixers.Main
{
    /*
     * Datafixer Identifier: 0.1.1
     * Datafixer Upgrade: dev.00014 to dev.00016
     * Datafixer Reason: Addition of modlog embed scrolling, requiring toggling and reaction emote IDs
    */
    public class Main0001_AddModlogScrolling : IDatafixer<MainConfiguration>
    {
        public String NewDataVersion { get => "2.0.0-dev.00016"; }

        public String OldDataVersion { get => "2.0.0-dev.00014"; }

        public UInt32 DatafixerId { get => 0; }

        public Boolean BreakingChange { get => false; }

        public DatafixerDowngradeResult DowngradeData(ref MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00014")
                return DatafixerDowngradeResult.AlreadyDowngraded;

            data.Configuration.Remove("insanitybot.modlog.allow_scrolling");
            data.Configuration.Remove("insanitybot.modlog.allow_verballog_scrolling");
            data.Configuration.Remove("insanitybot.identifiers.modlog.scroll_right_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.modlog.scroll_left_emote_id");
            data.DataVersion = "2.0.0-dev.00014";

            DatafixerLogger.LogInformation(new EventData(0, 1, 1, 1, "Downgrade"), "Downgraded successfully to version 2.0.0-dev.00014");
            return DatafixerDowngradeResult.Success;
        }

        public MainConfiguration ExportDowngradedData(MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00014")
                return data;

            data.Configuration.Remove("insanitybot.modlog.allow_scrolling");
            data.Configuration.Remove("insanitybot.modlog.allow_verballog_scrolling");
            data.Configuration.Remove("insanitybot.identifiers.modlog.scroll_right_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.modlog.scroll_left_emote_id");
            data.DataVersion = "2.0.0-dev.00014";

            DatafixerLogger.LogInformation(new EventData(0, 1, 1, 2, "DowngradeExport"), "Downgraded successfully to version 2.0.0-dev.00014");
            return data;
        }

        public MainConfiguration ExportUpgradedData(MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00014")
                return data;

            data.Configuration.Add("insanitybot.modlog.allow_scrolling", true);
            data.Configuration.Add("insanitybot.modlog.allow_verballog_scrolling", true);
            data.Configuration.Add("insanitybot.identifiers.modlog.scroll_right_emote_id", 0);
            data.Configuration.Add("insanitybot.identifiers.modlog.scroll_left_emote_id", 0);
            data.DataVersion = "2.0.0-dev.00016";

            DatafixerLogger.LogInformation(new EventData(0, 1, 1, 3, "UpgradeExport"), "Upgraded successfully to version 2.0.0-dev.00016");
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00014")
                return DatafixerUpgradeResult.AlreadyUpgraded;

            data.Configuration.Add("insanitybot.modlog.allow_scrolling", true);
            data.Configuration.Add("insanitybot.modlog.allow_verballog_scrolling", true);
            data.Configuration.Add("insanitybot.identifiers.modlog.scroll_right_emote_id", 0);
            data.Configuration.Add("insanitybot.identifiers.modlog.scroll_left_emote_id", 0);
            data.DataVersion = "2.0.0-dev.00016";

            DatafixerLogger.LogInformation(new EventData(0, 1, 1, 4, "Upgrade"), "Upgraded successfully to version 2.0.0-dev.00016");
            return DatafixerUpgradeResult.Success;
        }
    }
}
