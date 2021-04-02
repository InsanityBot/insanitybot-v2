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
     * Datafixer Identifier: 0.2.3
     * Datafixer Upgrade: 2.0.0-dev.00017 to 2.0.0-dev.00018
     * Datafixer Reason: Addition of permission commands, requiring new embeds
    */
    public class Lang0003_AddPermissions : IDatafixer<LanguageConfiguration>
    {
        public String NewDataVersion { get => "2.0.0-dev.00018"; }

        public String OldDataVersion { get => "2.0.0-dev.00017"; }

        public UInt32 DatafixerId { get => 2; }

        public Boolean BreakingChange { get => false; }

        public DatafixerDowngradeResult DowngradeData(ref LanguageConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00017")
                return DatafixerDowngradeResult.AlreadyDowngraded;

            data.Configuration.Remove("insanitybot.permissions.error.permission_not_found");
            data.Configuration.Remove("insanitybot.permissions.error.no_permission_passed");
            data.Configuration.Remove("insanitybot.permissions.error.could_not_grant");
            data.Configuration.Remove("insanitybot.permissions.error.could_not_neutralize");
            data.Configuration.Remove("insanitybot.permissions.permission_granted");
            data.Configuration.Remove("insanitybot.permissions.permission_neutralized");
            data.DataVersion = "2.0.0-dev.00017";

            DatafixerLogger.LogInformation(new EventData(0, 2, 3, 1, "Downgrade"), "Downgraded successfully to version 2.0.0-dev.00017");
            return DatafixerDowngradeResult.Success;
        }

        public LanguageConfiguration ExportDowngradedData(LanguageConfiguration data)
        {
            if (data.DataVersion == "2.0.0-dev.00017")
                return data;

            data.Configuration.Remove("insanitybot.permissions.error.permission_not_found");
            data.Configuration.Remove("insanitybot.permissions.error.no_permission_passed");
            data.Configuration.Remove("insanitybot.permissions.error.could_not_grant");
            data.Configuration.Remove("insanitybot.permissions.error.could_not_neutralize");
            data.Configuration.Remove("insanitybot.permissions.permission_granted");
            data.Configuration.Remove("insanitybot.permissions.permission_neutralized");
            data.DataVersion = "2.0.0-dev.00017";

            DatafixerLogger.LogInformation(new EventData(0, 2, 3, 2, "ExportDowngrade"), "Downgraded successfully to version 2.0.0-dev.00017");
            return data;
        }

        public LanguageConfiguration ExportUpgradedData(LanguageConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00017")
                return data;

            data.Configuration.Add("insanitybot.permissions.error.permission_not_found", "Permission {PERMISSION} could not be found.");
            data.Configuration.Add("insanitybot.permissions.error.no_permission_passed", "No permission was passed to the command.");
            data.Configuration.Add("insanitybot.permissions.error.could_not_grant", "Could not grant permission {PERMISSION} to {MENTION}.");
            data.Configuration.Add("insanitybot.permissions.error.could_not_neutralize", "Could not neutralize permission override {PERMISSION} from {MENTION}");
            data.Configuration.Add("insanitybot.permissions.permission_granted", "Granted permission {PERMISSION} to {MENTION}");
            data.Configuration.Add("insanitybot.permissions.permission_neutralized", "Neutralized permission override {PERMISSION} from {MENTION}");
            data.DataVersion = "2.0.0-dev.00018";

            DatafixerLogger.LogInformation(new EventData(0, 2, 3, 3, "ExportUpgrade"), "Upgraded successfully to version 2.0.0-dev.00018");
            return data;
        }

        public DatafixerUpgradeResult UpgradeData(ref LanguageConfiguration data)
        {
            if (data.DataVersion != "2.0.0-dev.00017")
                return DatafixerUpgradeResult.AlreadyUpgraded;

            data.Configuration.Add("insanitybot.permissions.error.permission_not_found", "Permission {PERMISSION} could not be found.");
            data.Configuration.Add("insanitybot.permissions.error.no_permission_passed", "No permission was passed to the command.");
            data.Configuration.Add("insanitybot.permissions.error.could_not_grant", "Could not grant permission {PERMISSION} to {MENTION}.");
            data.Configuration.Add("insanitybot.permissions.error.could_not_neutralize", "Could not neutralize permission override {PERMISSION} from {MENTION}");
            data.Configuration.Add("insanitybot.permissions.permission_granted", "Granted permission {PERMISSION} to {MENTION}");
            data.Configuration.Add("insanitybot.permissions.permission_neutralized", "Neutralized permission override {PERMISSION} from {MENTION}");
            data.DataVersion = "2.0.0-dev.00018";

            DatafixerLogger.LogInformation(new EventData(0, 2, 3, 4, "Upgrade"), "Upgraded successfully to version 2.0.0-dev.00018");
            return DatafixerUpgradeResult.Success; 
        }
    }
}
