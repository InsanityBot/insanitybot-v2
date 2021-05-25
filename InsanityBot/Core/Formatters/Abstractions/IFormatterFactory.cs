using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Core.Formatters.Abstractions
{
    public interface IFormatterFactory
    {
        public ITypelessFormatter GetFormatter();
    }
}
