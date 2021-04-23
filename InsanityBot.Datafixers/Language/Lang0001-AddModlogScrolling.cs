using System;

using Helium.Commons.Logging;

using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Datafixers.Reference;
using InsanityBot.Utility.Language;

namespace InsanityBot.Datafixers.Language
{
    /*
     * Datafixer Identifier: 0.2.1
     * Datafixer Upgrade: beta.001 to dev.00016
     * Datafixer Reason: Addition of modlog embed scrolling, requiring language data for the new bottom line
    */
    public class Lang0001_AddModlogScrolling : IDatafixer<LanguageConfiguration>
    {
        public String NewDataVersion => "2.0.0-dev.00016";

        public String OldDataVersion => "2.0.0.0-beta.001";

        public UInt32 DatafixerId => 0;

        public Boolean BreakingChange => false;

        public DatafixerDowngradeResult DowngradeData(ref LanguageConfiguration data)
        {
            if (data.DataVersion == "2.0.0.0-beta.001")
            {
                return DatafixerDowngradeResult.AlreadyDowngraded;
            }

            data.Configuration.Remove("insanitybot.commands.modlog.paged.page_number");
            data.Configuration.Remove("insanitybot.commands.verbal_log.paged.page_number");
            data.DataVersion = "2.0.0.0-beta.001";

            DatafixerLogger.LogInformation(new EventData(0, 2, 1, 1, "Downgrade"), "Downgraded successfully to version 2.0.0.0-beta.001");
            return DatafixerDowngradeResult.Success;
        }

        public LanguageConfiguration ExportDowngradedData(LanguageConfiguration data)
        {
            if (data.DataVersion == "2.0.0.0-beta.001")
            {
                return data;
            }

            data.Configuration.Remove("insanitybot.commands.modlog.paged.page_number");
            data.Configuration.Remove("insanitybot.commands.verbal_log.paged.page_number");
            data.DataVersion = "2.0.0.0-beta.001";

            DatafixerLogger.LogInformation(new EventData(0, 2, 1, 2, "DowngradeExport"), "Downgraded successfully to version 2.0.0.0-beta.001");
            return data;
        }

        public LanguageConfiguration ExportUpgradedData(LanguageConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00016")
            {
                return data;
            }

            data.Configuration.Add("insanitybot.commands.modlog.paged.page_number", "Page {PAGE}/{PAGE_TOTAL}");
            data.Configuration.Add("insanitybot.commands.verbal_log.paged.page_number", "Page {PAGE}/{PAGE_TOTAL}");
            data.DataVersion = "2.0.0-dev.00016";

            DatafixerLogger.LogInformation(new EventData(0, 2, 1, 3, "UpgradeExport"), "Upgraded successfully to version 2.0.0-dev.00016");
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref LanguageConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00016")
            {
                return DatafixerUpgradeResult.AlreadyUpgraded;
            }

            data.Configuration.Add("insanitybot.commands.modlog.paged.page_number", "Page {PAGE}/{PAGE_TOTAL}");
            data.Configuration.Add("insanitybot.commands.verbal_log.paged.page_number", "Page {PAGE}/{PAGE_TOTAL}");
            data.DataVersion = "2.0.0-dev.00016";

            DatafixerLogger.LogInformation(new EventData(0, 2, 1, 4, "Upgrade"), "Upgraded successfully to version 2.0.0-dev.00016");
            return DatafixerUpgradeResult.Success;
        }
    }
}
