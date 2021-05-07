using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Kyuu.Runtime.Variables.Datatypes
{
    public struct MockGuid
    {
        public String Value { get; set; }

        public static explicit operator Guid(MockGuid guid)
        {
            return new(Encoding.UTF8.GetBytes(guid.Value));
        }

        public static explicit operator MockGuid(Guid guid)
        {
            return new()
            {
                Value = guid.ToString()
            };
        }

        public MockGuid(String @string)
        {
            this.Value = @string;
        }
    }
}
