using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Datafixers.Reference;

namespace InsanityBot.Utility.Datafixers
{
    public interface IDatafixer
    {
        public DatafixerUpgradeResult UpgradeData(ref IDatafixable data);

        public DatafixerDowngradeResult DowngradeData(ref IDatafixable data);

        public static DatafixerLoadingResult Load()
        {
            return DatafixerLoadingResult.Success;
        }

        public IDatafixable ExportUpgradedData(IDatafixable data);

        public IDatafixable ExportDowngradedData(IDatafixable data);

        public static String NewDataVersion { get; }
        public static String OldDataVersion { get; }
        public static UInt32 DatafixerId { get; }
        public static Boolean BreakingChange { get; }
    }
}
