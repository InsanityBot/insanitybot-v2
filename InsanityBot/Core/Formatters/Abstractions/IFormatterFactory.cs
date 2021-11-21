namespace InsanityBot.Core.Formatters.Abstractions;

public interface IFormatterFactory
{
	public ITypelessFormatter GetFormatter();
}