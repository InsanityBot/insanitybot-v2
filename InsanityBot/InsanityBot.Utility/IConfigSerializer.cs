using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility
{
    interface IConfigSerializer<T, U> where T : IConfiguration<U>
    {
        public T Deserialize(String Filename);
        public void Serialize(T Config, String Filename);
    }
}
