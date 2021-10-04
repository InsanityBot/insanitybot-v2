using InsanityBot.SlashCommands.Moderation;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        private static void RegisterSlashCommands()
        {
            SlashCommandsExtension.RegisterCommands<MuteSlash>(HomeGuild.Id);
            SlashCommandsExtension.RegisterCommands<TestCommand>(HomeGuild.Id);
        }
    }
}