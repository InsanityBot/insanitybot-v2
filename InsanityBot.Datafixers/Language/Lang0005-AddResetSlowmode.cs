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
     * Datafixer Identifier: 0.2.5
     * Datafixer Upgrade: 2.0.0-dev.00020 to 2.0.0-dev.00022
     * Datafixer Reason: Addition of slowmode reset command, requiring new messages
    */
    public class Lang0005_AddResetSlowmode : IDatafixer<LanguageConfiguration>
    {
        public String NewDataVersion { get => "2.0.0-dev.00022"; }

        public String OldDataVersion { get => "2.0.0-dev.00020"; }

        public UInt32 DatafixerId { get => 4; }

        public Boolean BreakingChange { get => false; }

        public DatafixerDowngradeResult DowngradeData(ref LanguageConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00020")
                return DatafixerDowngradeResult.AlreadyDowngraded;

            data.Configuration.Remove("insanitybot.moderation.slowmode_reset.success");
            data.Configuration.Remove("insanitybot.moderation.slowmode_reset.failure");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 2, 5, 1, "Downgrade"), "Downgraded successfully to version 2.0.0-dev.00020");
            return DatafixerDowngradeResult.Success;
        }

        public LanguageConfiguration ExportDowngradedData(LanguageConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00020")
                return data;

            data.Configuration.Remove("insanitybot.moderation.slowmode_reset.success");
            data.Configuration.Remove("insanitybot.moderation.slowmode_reset.failure");
            data.DataVersion = "2.0.0-dev.00020";

            DatafixerLogger.LogInformation(new EventData(0, 2, 5, 2, "ExportDowngrade"), "Downgraded successfully to version 2.0.0-dev.00020");
            return data;
        }

        public LanguageConfiguration ExportUpgradedData(LanguageConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00020")
                return data;

            data.Configuration.Add("insanitybot.moderation.slowmode_reset.success", "{CHANNEL} slowmode was removed successfully.");
            data.Configuration.Add("insanitybot.moderation.slowmode_reset.failure", "Could not remove slowmode for {CHANNEL}.");
            data.DataVersion = "2.0.0-dev.00022";

            DatafixerLogger.LogInformation(new EventData(0, 2, 5, 3, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00022");
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref LanguageConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00020")
                return DatafixerUpgradeResult.AlreadyUpgraded;

            data.Configuration.Add("insanitybot.moderation.slowmode_reset.success", "{CHANNEL} slowmode was removed successfully.");
            data.Configuration.Add("insanitybot.moderation.slowmode_reset.failure", "Could not remove slowmode for {CHANNEL}.");
            data.DataVersion = "2.0.0-dev.00022";

            DatafixerLogger.LogInformation(new EventData(0, 2, 5, 4, "Upgrade"), "Upgraded successfully to version 2.0.0-dev.00022");
            return DatafixerUpgradeResult.Success;
        }
    }
}
