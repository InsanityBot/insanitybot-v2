using System;

namespace InsanityBot.Core.Formatters.Abstractions
{
    public interface IFormatter<T> : ITypelessFormatter
    {
        public String Format(T value);
        public T Read(String value);
    }
}
