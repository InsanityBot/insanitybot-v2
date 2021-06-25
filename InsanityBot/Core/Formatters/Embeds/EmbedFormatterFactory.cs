using InsanityBot.Core.Formatters.Abstractions;

namespace InsanityBot.Core.Formatters.Embeds
{
    public class EmbedFormatterFactory : IFormatterFactory
    {
        public ITypelessFormatter GetFormatter()
            => new EmbedFormatter();
    }
}
