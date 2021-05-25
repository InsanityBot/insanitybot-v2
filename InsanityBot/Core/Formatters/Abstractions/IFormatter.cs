using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Core.Formatters.Abstractions
{
    public interface IFormatter<T> : ITypelessFormatter
    {
        public String Format(T value);
        public T Read(String value);
    }
}
