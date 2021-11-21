namespace InsanityBot.Utility.Datafixers;

using InsanityBot.Utility.Datafixers.Reference;
using InsanityBot.Utility.Reference;

public interface IDatafixer<Datafixable> : IDatafixer
	where Datafixable : IDatafixable
{
	public DatafixerUpgradeResult UpgradeData(ref Datafixable data);

	public DatafixerDowngradeResult DowngradeData(ref Datafixable data);

	public Datafixable ExportUpgradedData(Datafixable data);

	public Datafixable ExportDowngradedData(Datafixable data);
}