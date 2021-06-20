using System;

namespace InsanityBot.Utility
{
	public interface IConfigSerializer<T, U> where T : IConfiguration<U>
	{
		public T Deserialize(String Filename);
		public void Serialize(T Config, String Filename);
	}
}
