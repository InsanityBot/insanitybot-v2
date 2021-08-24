using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json.Linq;

namespace InsanityBot.Utility
{
    public interface IConfiguration : IDatafixable
    {
        public JObject Configuration { get; set; }
    }
}
