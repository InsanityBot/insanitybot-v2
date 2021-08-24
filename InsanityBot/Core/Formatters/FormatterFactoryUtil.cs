using System;
using System.Collections.Generic;

using InsanityBot.Core.Formatters.Abstractions;

namespace InsanityBot.Core.Formatters
{
    public class FormatterFactoryUtil
    {
        private Dictionary<String, IFormatterFactory> Factories { get; set; }

        public void RegisterFactory(String name, IFormatterFactory factory)
            => this.Factories.Add(name, factory);

        public IFormatterFactory this[String name] => this.Factories[name];
    }
}
