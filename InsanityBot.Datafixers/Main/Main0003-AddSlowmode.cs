using Helium.Commons.Logging;

using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Datafixers.Reference;

using System;

namespace InsanityBot.Datafixers.Main
{
    /*
     * Datafixer Identifier: 0.1.3
     * Datafixer Upgrade: 2.0.0-dev.00017 to 2.0.0-dev.00020
     * Datafixer Reason: Add a slowmode command, requiring a default time
     * Also adds interactivity emote support, requiring three new IDs and changing one existing ID
    */
    public class Main0003_AddSlowmode : IDatafixer<MainConfiguration>
    {
        public String NewDataVersion => "2.0.0-dev.00020";

        public String OldDataVersion => "2.0.0-dev.00017";

        public UInt32 DatafixerId => 2;

        public Boolean BreakingChange => false;

        public DatafixerDowngradeResult DowngradeData(ref MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00017")
            {
                return DatafixerDowngradeResult.AlreadyDowngraded;
            }

            data.Configuration.Add("insanitybot.identifiers.modlog.scroll_right_emote_id", data["insanitybot.identifiers.interactivity.scroll_right_emote_id"]);
            data.Configuration.Add("insanitybot.identifiers.modlog.scroll_left_emote_id", data["insanitybot.identifiers.interactivity.scroll_left_emote_id"]);

            data.Configuration.Remove("insanitybot.identifiers.interactivity.scroll_right_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.interactivity.scroll_left_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.interactivity.skip_right_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.interactivity.skip_left_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.interactivity.stop_emote_id");

            data.Configuration.Remove("insanitybot.commands.slowmode.default_slowmode");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 1, 3, 1, "Downgrade"), "Downgraded successfully to version 2.0.0-dev.00017");
            return DatafixerDowngradeResult.Success;
        }

        public MainConfiguration ExportDowngradedData(MainConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00017")
            {
                return data;
            }

            data.Configuration.Add("insanitybot.identifiers.modlog.scroll_right_emote_id", data["insanitybot.identifiers.interactivity.scroll_right_emote_id"]);
            data.Configuration.Add("insanitybot.identifiers.modlog.scroll_left_emote_id", data["insanitybot.identifiers.interactivity.scroll_left_emote_id"]);

            data.Configuration.Remove("insanitybot.identifiers.interactivity.scroll_right_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.interactivity.scroll_left_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.interactivity.skip_right_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.interactivity.skip_left_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.interactivity.stop_emote_id");

            data.Configuration.Remove("insanitybot.commands.slowmode.default_slowmode");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 1, 3, 2, "ExportDowngrade"), "Downgraded successfully to version 2.0.0-dev.00017");
            return data;
        }

        public MainConfiguration ExportUpgradedData(MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00017")
            {
                return data;
            }

            data.Configuration.Add("insanitybot.identifiers.interactivity.scroll_right_emote_id", data["insanitybot.identifiers.modlog.scroll_right_emote_id"]);
            data.Configuration.Add("insanitybot.identifiers.interactivity.scroll_left_emote_id", data["insanitybot.identifiers.modlog.scroll_left_emote_id"]);
            data.Configuration.Add("insanitybot.identifiers.interactivity.skip_right_emote_id", 0);
            data.Configuration.Add("insanitybot.identifiers.interactivity.skip_left_emote_id", 0);
            data.Configuration.Add("insanitybot.identifiers.interactivity.stop_emote_id", 0);

            data.Configuration.Add("insanitybot.commands.slowmode.default_slowmode", 5);

            data.Configuration.Remove("insanitybot.identifiers.modlog.scroll_right_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.modlog_scroll_left_emote_id");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 1, 3, 3, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00020");
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref MainConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00017")
            {
                return DatafixerUpgradeResult.AlreadyUpgraded;
            }

            data.Configuration.Add("insanitybot.identifiers.interactivity.scroll_right_emote_id", data["insanitybot.identifiers.modlog.scroll_right_emote_id"]);
            data.Configuration.Add("insanitybot.identifiers.interactivity.scroll_left_emote_id", data["insanitybot.identifiers.modlog.scroll_left_emote_id"]);
            data.Configuration.Add("insanitybot.identifiers.interactivity.skip_right_emote_id", 0);
            data.Configuration.Add("insanitybot.identifiers.interactivity.skip_left_emote_id", 0);
            data.Configuration.Add("insanitybot.identifiers.interactivity.stop_emote_id", 0);

            data.Configuration.Add("insanitybot.commands.slowmode.default_slowmode", 5);

            data.Configuration.Remove("insanitybot.identifiers.modlog.scroll_right_emote_id");
            data.Configuration.Remove("insanitybot.identifiers.modlog_scroll_left_emote_id");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 1, 3, 4, "Upgrade"), "Upgraded successfully to version 2.0.0-dev.00020");
            return DatafixerUpgradeResult.Success;
        }
    }
}
