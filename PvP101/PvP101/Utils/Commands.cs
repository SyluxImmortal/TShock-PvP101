using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace PvP101.Utils
{
	class Commands
	{
		public static bool SetGameCommand(CommandArgs args, bool inGameOnly, int paramCount, string syntax)
		{
			bool failed = true;
			if (inGameOnly && !args.Player.RealPlayer)
			{
				args.Player.SendErrorMessage("This command must be used in-game");
				failed = true;
			}
			else
			{
				if (args.Parameters.Count < paramCount)
				{
					args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}", syntax);
					failed = true;
				}
				else
				{
					failed = false;
				}
			}
			return failed;
		}

		public static string OptionalParam(CommandArgs args, int paramIndex, string defaultValue)
		{
			string output = defaultValue;
			if (args.Parameters.Count > paramIndex)
			{
				output = args.Parameters[paramIndex];
			}
			return output;
		}
	}
}
