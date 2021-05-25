using InsanityBot.Core.Formatters.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Core.Formatters
{
    public class FormatterFactoryUtil
    {
        Dictionary<String, IFormatterFactory> Factories { get; set; }

        public void RegisterFactory(String name, IFormatterFactory factory)
            => Factories.Add(name, factory);

        public IFormatterFactory this[String name]
        {
            get => Factories[name];
        }
    }
}
