using Helium.Commons.Logging;

using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Datafixers.Reference;
using InsanityBot.Utility.Language;

using System;

namespace InsanityBot.Datafixers.Language
{
    /*
     * Datafixer Identifier: 0.2.4
     * Datafixer Upgrade: 2.0.0-dev.00018 to 2.0.0-dev.00020 
     * Datafixer Reason: Addition of slowmode command, requiring new messages
    */
    public class Lang0004_AddSlowmode : IDatafixer<LanguageConfiguration>
    {
        public String NewDataVersion => "2.0.0-dev.00020";

        public String OldDataVersion => "2.0.0-dev.00018";

        public UInt32 DatafixerId => 3;

        public Boolean BreakingChange => false;

        public DatafixerDowngradeResult DowngradeData(ref LanguageConfiguration data)
        {
            if(data.DataVersion == "2.0.0-dev.00018")
            {
                return DatafixerDowngradeResult.AlreadyDowngraded;
            }

            data.Configuration.Remove("insanitybot.moderation.slowmode.success");
            data.Configuration.Remove("insanitybot.moderation.slowmode.failure");
            data.DataVersion = "2.0.0-dev.00018";

            DatafixerLogger.LogInformation(new EventData(0, 2, 4, 1, "Downgrade"), "Downgraded successfully to version 2.0.0-dev.00018");
            return DatafixerDowngradeResult.Success;
        }

        public LanguageConfiguration ExportDowngradedData(LanguageConfiguration data)
        {
            if(data.DataVersion == "2.0.0-dev.00018")
            {
                return data;
            }

            data.Configuration.Remove("insanitybot.moderation.slowmode.success");
            data.Configuration.Remove("insanitybot.moderation.slowmode.failure");
            data.DataVersion = "2.0.0-dev.00018";

            DatafixerLogger.LogInformation(new EventData(0, 2, 4, 2, "ExportDowngrade"), "Downgraded successfully to version 2.0.0-dev.00018");
            return data;
        }

        public LanguageConfiguration ExportUpgradedData(LanguageConfiguration data)
        {
            if(data.DataVersion != "2.0.0-dev.00018")
            {
                return data;
            }

            data.Configuration.Add("insanitybot.moderation.slowmode.success", "{CHANNEL} slowmode was set to {TIME}.");
            data.Configuration.Add("insanitybot.moderation.slowmode.failure", "Could not set slowmode for {CHANNEL}.");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 2, 4, 3, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00020");
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref LanguageConfiguration data)
        {
            if(data.DataVersion != "2.0.0-dev.00018")
            {
                return DatafixerUpgradeResult.AlreadyUpgraded;
            }

            data.Configuration.Add("insanitybot.moderation.slowmode.success", "{CHANNEL} slowmode was set to {TIME}.");
            data.Configuration.Add("insanitybot.moderation.slowmode.failure", "Could not set slowmode for {CHANNEL}.");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 2, 4, 4, "Upgrade"), "Upgraded successfully to version 2.0.0-dev.00020");
            return DatafixerUpgradeResult.Success;
        }
    }
}
