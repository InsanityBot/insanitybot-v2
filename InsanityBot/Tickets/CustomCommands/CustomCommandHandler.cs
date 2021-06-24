using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;

namespace InsanityBot.Tickets.CustomCommands
{
	public sealed class CustomCommandHandler
	{
		public static readonly Dictionary<InternalCommand, Action<CommandContext, Object>> internalCommandMap = new()
		{
		};

		public List<Command> Commands { get; private set; }

		public void HandleCommand(CommandContext context, String command)
		{
			IEnumerable<Command> commands = from i in Commands
											where i.Trigger == command
											select i;
		}
	}
}
