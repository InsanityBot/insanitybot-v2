namespace InsanityBot.Core.Formatters.Abstractions;
using System;

public interface IFormatter<T> : ITypelessFormatter
{
	public String Format(T value);
	public T Read(String value);
}