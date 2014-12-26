using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace PvP101.Commands
{
	class Utility
	{
		public static void Init()
		{
			TShockAPI.Commands.ChatCommands.Add(new Command("pvp101.utility.setlife", SetLife, "setlife"));
			TShockAPI.Commands.ChatCommands.Add(new Command("pvp101.utility.setmana", SetMana, "setmana"));
		}

		private static void SetLife(CommandArgs args)
		{
			TSPlayer player = args.Player;

			if (Utils.Commands.SetGameCommand(args, true, 1, "/setlife <100~600>")) { return; }

			int hp;

			if (!int.TryParse(args.Parameters[0], out hp) || hp < 100 || hp > 600)
			{
				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /setlife <100~600>");
				return;
			}

			player.TPlayer.statLife = hp;
			player.TPlayer.statLifeMax = hp;

			NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, -1, player.Name, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData((int)PacketTypes.PlayerHp, -1, -1, "", player.Index, 0f, 0f, 0f, 0);
		}
		private static void SetMana(CommandArgs args)
		{
			TSPlayer player = args.Player;
			if (Utils.Commands.SetGameCommand(args, true, 1, "/setmana <0~400>")) { return; }

				int mp;

				if (!int.TryParse(args.Parameters[0], out mp) || mp < 0 || mp > 400)
				{
					args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /setmana <0~400>");
					return;
				}

				player.TPlayer.statMana = mp;
				player.TPlayer.statManaMax = mp;

				NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, -1, player.Name, player.Index, 0f, 0f, 0f, 0);
				NetMessage.SendData((int)PacketTypes.PlayerMana, -1, -1, "", player.Index, 0f, 0f, 0f, 0);
		}
	}
}
