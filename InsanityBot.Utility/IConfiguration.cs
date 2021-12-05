namespace InsanityBot.Utility;
using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json.Linq;

public interface IConfiguration : IDatafixable
{
	public JObject Configuration { get; set; }
}