namespace InsanityBot.Core.Formatters.Embeds;
using global::InsanityBot.Core.Formatters.Abstractions;

public class EmbedFormatterFactory : IFormatterFactory
{
	public ITypelessFormatter GetFormatter()
		=> new EmbedFormatter();
}