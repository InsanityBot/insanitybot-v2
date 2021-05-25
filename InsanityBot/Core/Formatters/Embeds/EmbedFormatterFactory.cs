using InsanityBot.Core.Formatters.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Core.Formatters.Embeds
{
    public class EmbedFormatterFactory : IFormatterFactory
    {
        public ITypelessFormatter GetFormatter()
            => new EmbedFormatter();
    }
}
