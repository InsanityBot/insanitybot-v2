using System;

using InsanityBot.Utility.Datafixers.Reference;

namespace InsanityBot.Utility.Exceptions
{
	public class DatafixerNotFoundException : Exception
	{
		public DatafixerRegistryEntry Datafixer { get; set; }

		public DatafixerNotFoundException(String message, DatafixerRegistryEntry datafixer) : base(message)
		{
			this.Datafixer = datafixer;
		}
	}
}
