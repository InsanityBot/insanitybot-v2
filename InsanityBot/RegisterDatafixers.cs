using InsanityBot.Datafixers.Language;
using InsanityBot.Datafixers.Main;
using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Language;

namespace InsanityBot
{
	public partial class InsanityBot
	{
		public static void RegisterDatafixers()
		{
			DataFixerLower.AddDatafixer(new Main0001_AddModlogScrolling(), typeof(MainConfiguration));
			DataFixerLower.AddDatafixer(new Main0002_AddChannelLocking(), typeof(MainConfiguration));
			DataFixerLower.AddDatafixer(new Main0003_AddSlowmode(), typeof(MainConfiguration));

			DataFixerLower.AddDatafixer(new Lang0001_AddModlogScrolling(), typeof(LanguageConfiguration));
			DataFixerLower.AddDatafixer(new Lang0002_AddChannelLocking(), typeof(LanguageConfiguration));
			DataFixerLower.AddDatafixer(new Lang0003_AddPermissions(), typeof(LanguageConfiguration));
			DataFixerLower.AddDatafixer(new Lang0004_AddSlowmode(), typeof(LanguageConfiguration));
			DataFixerLower.AddDatafixer(new Lang0005_AddResetSlowmode(), typeof(LanguageConfiguration));
		}
	}
}
