namespace InsanityBot.Core.Formatters;
using System;
using System.Collections.Generic;

using global::InsanityBot.Core.Formatters.Abstractions;

public class FormatterFactoryUtil
{
	private Dictionary<String, IFormatterFactory> Factories { get; set; }

	public void RegisterFactory(String name, IFormatterFactory factory)
		=> this.Factories.Add(name, factory);

	public IFormatterFactory this[String name] => this.Factories[name];
}