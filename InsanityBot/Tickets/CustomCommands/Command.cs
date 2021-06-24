using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.CustomCommands
{
	public record Command
	{
		public String Trigger { get; init; }
		public InternalCommand InternalCommand { get; init; }
	}
}
