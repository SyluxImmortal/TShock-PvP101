using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PvP101.Structure;
using PvP101.Data;
using TShockAPI;

namespace PvP101.Commands
{
	class Classes
	{
		public static void Init()
		{
			TShockAPI.Commands.ChatCommands.Add(new Command(new List<string>() { "pvp101.classes.edit", "pvp101.classes.delete", "pvp101.classes.use" }, Main, "class"));
		}

		public static void Main(CommandArgs args)
		{
			if (!Terraria.Main.ServerSideCharacter)
			{
				Log.ConsoleError("[PvP101.Classes] This plugin will not work properly with ServerSidedCharacters disabled.");
				return;
			}

			string subCommand;
			if (args.Parameters.Count > 0)
			{
				subCommand = args.Parameters[0].ToLower();
				args.Parameters.RemoveAt(0);
			}
			else
			{
				subCommand = "";
			}

			var group = args.Player.Group;

			switch (subCommand)
			{
				case "save":
				case "describe":
				case "categorize":
					if (!group.HasPermission("pvp101.classes.edit")) { subCommand = ""; }
					break;
				case "delete":
					if (!group.HasPermission("pvp101.classes.delete")) { subCommand = ""; }
					break;
				case "select":
				case "remove":
					if (!group.HasPermission("pvp101.classes.use")) { subCommand = ""; }
					break;
				default:
					break;
			}

			switch (subCommand)
			{
				case "save":
					SaveCommand(args);
					break;
				case "delete":
					DeleteCommand(args);
					break;
				case "select":
					SelectCommand(args);
					break;
				case "current":
					CurrentCommand(args);
					break;
				case "remove":
					RemoveCommand(args);
					break;
				case "list":
					ListCommand(args);
					break;
				case "info":
					InfoCommand(args);
					break;
				case "describe":
					DescribeCommand(args);
					break;
				case "categorize":
					CategorizeCommand(args);
					break;
				default:
					if (subCommand != "help")
					{
						args.Parameters.Insert(0, subCommand);
					}
					HelpCommand(args);
					break;
			}

		}

		private static void SaveCommand(CommandArgs args)
		{
			TSPlayer player = args.Player;
			if (Utils.Commands.SetGameCommand(args, true, 1, "/class save <title> [cat] [desc]")) { return; }

			CharacterData data = Data.Classes.FromCurrent(player);
			data.Title = args.Parameters[0];
			data.Category = Utils.Commands.OptionalParam(args, 1, data.Category);
			data.Description = Utils.Commands.OptionalParam(args, 2, data.Description);
			Data.Classes.Save(data);
			args.Player.SendSuccessMessage("({0}) class has been saved", args.Parameters[0]);
		}
		private static void DeleteCommand(CommandArgs args)
		{
			TSPlayer player = args.Player;
			if (Utils.Commands.SetGameCommand(args, false, 1, "/class delete <title>")) { return; }

			var classTitle = args.Parameters[0];
			Data.Classes.Delete(classTitle);
			player.SendSuccessMessage("({0}) deleted", classTitle);
		}
		private static void SelectCommand(CommandArgs args)
		{
			TSPlayer player = args.Player;

			if (Utils.Commands.SetGameCommand(args, false, 1, "/class select <title>")) { return; }

			var classTitle = args.Parameters[0];
			CharacterData classData = Data.Classes.Get(classTitle);

			if (classData != null && classData.Title != null && classData.Title != "")
			{
				Data.CharacterBackups.Save(player.UserID, Data.Classes.FromCurrent(player));
				Data.Players.Save(player.UserID, new Structure.PlayerData() { CurrentClass = classData.Title });
				Data.Classes.ToCurrent(player, classData);
				player.SendSuccessMessage("You are now a {0}", classTitle);
			}
			else
			{
				args.Player.SendInfoMessage("({0}) is not a valid class", classTitle);
			}
		}
		private static void CurrentCommand(CommandArgs args)
		{
			TSPlayer player = args.Player;
			if (Utils.Commands.SetGameCommand(args, true, 0, "/class current")) { return; }

			Structure.PlayerData playerData = Data.Players.Get(player.UserID);
			player.SendInfoMessage("Your current class is: {0}", playerData == null ? "None" : playerData.CurrentClass);
		}
		private static void RemoveCommand(CommandArgs args)
		{
			TSPlayer player = args.Player;
			if (Utils.Commands.SetGameCommand(args, false, 0, "/class remove")) { return; }

			CharacterData characterData = Data.CharacterBackups.Get(player.UserID);

			if (characterData == null)
			{
				args.Player.SendErrorMessage("You are already yourself");
			}
			else
			{
				Data.Classes.ToCurrent(player, characterData);
				Data.CharacterBackups.Delete(player.UserID);
				Data.Players.Save(player.UserID, new Structure.PlayerData() { CurrentClass = "None" });
				player.SendSuccessMessage("You are now youself again");
			}
		}
		private static void ListCommand(CommandArgs args)
		{
			TSPlayer player = args.Player;
			if (Utils.Commands.SetGameCommand(args, false, 0, "/class list [page]")) { return; }

			if (args.Parameters.Count < 1) { args.Parameters.Add("1"); }
			string nextParam = args.Parameters[0];
			List<string> list = null;
			int page = -1;
			string header = null;
			string footer = null;

			if (nextParam == "categories")
			{
				list = Data.Classes.GetCategoryList();

				if (list.Count > 0)
				{
					page = 1;
					if (!(args.Parameters.Count > 1) || !PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out page))
						page = 1;
					header = "Class Category list ({0}/{1}):";
					footer = "Type /class list category {0} for more.";
				}
			}
			else
			{
				if (Utils.Math.IsInteger(nextParam))
				{
					list = Data.Classes.GetList();

					if (list.Count > 0)
					{
						page = 1;
						if (!(args.Parameters.Count > 0) || !PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out page))
							page = 1;

						header = "Class list ({0}/{1}):";
						footer = "Type /class list {0} for more.";
					}

				}
				else
				{
					list = Data.Classes.GetList(nextParam);

					if (list.Count > 0)
					{
						page = 1;
						if (!(args.Parameters.Count > 1) || !PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out page))
							page = 1;

						header = "Filtered class list ({0}/{1}):";
						footer = "Type /class list " + nextParam + "{0} for more.";
					}
				}

			}

			if (page > 0)
			{
				PaginationTools.SendPage(args.Player, page, list,
					new PaginationTools.Settings
					{
						HeaderFormat = header,
						FooterFormat = footer
					});
			}
			else
			{
				player.SendInfoMessage("Zero items found");
			}
		}
		private static void InfoCommand(CommandArgs args)
		{
			TSPlayer player = args.Player;
			if (Utils.Commands.SetGameCommand(args, false, 1, "/class info <title>")) { return; }

			var classTitle = args.Parameters[0];
			string description = Data.Classes.GetDescription(classTitle);

			if (description == null)
			{
				player.SendInfoMessage("({0}) does not exist", classTitle);
			}
			else
			{
				player.SendSuccessMessage("({0}) - {1}", classTitle, description);
			}
		}
		private static void DescribeCommand(CommandArgs args)
		{
			TSPlayer player = args.Player;
			if (Utils.Commands.SetGameCommand(args, false, 2, "/class describe <title> <desc>")) { return; }

			var classTitle = args.Parameters[0];
			var description = args.Parameters[1];
			Data.Classes.UpdateDescription(classTitle, description);
			player.SendSuccessMessage("({0}) description saved", classTitle);
		}
		private static void CategorizeCommand(CommandArgs args)
		{
			TSPlayer player = args.Player;
			if (Utils.Commands.SetGameCommand(args, false, 2, "/class categorize <title> <category>")) { return; }

			var classTitle = args.Parameters[0];
			var category = args.Parameters[1];
			Data.Classes.UpdateCategory(classTitle, category);
			player.SendSuccessMessage("({0}) category saved", classTitle);
		}

		private static void HelpCommand(CommandArgs args)
		{
			var group = args.Player.Group;

			List<string> cmds = new List<string>();
			if (group.HasPermission("pvp101.classes.edit"))
			{
				cmds.Add("Save <title> [cat] [desc] - Saves the current character as a class.");
				cmds.Add("Describe <title> <desc> - Updates a class's description.");
				cmds.Add("Categorize <title> <category> - Updates a class's category.");
			}
			if (group.HasPermission("pvp101.classes.delete"))
			{
				cmds.Add("Delete <title> - Deletes a class completely.");
			}
			if (group.HasPermission("pvp101.classes.use"))
			{
				cmds.Add("Select <title> - Changes the player's class.");
				cmds.Add("Current - Displays the player's current class");
				cmds.Add("Remove - Restores the player's original character.");
				cmds.Add("List - Lists all of the classes");
				cmds.Add("List categories - Lists all of the classes of a specific category");
				cmds.Add("List <category> - Lists all of the class categories");
				cmds.Add("Info <title> - Displays a description of a class");
			}

			int page = 1;
			if (!(args.Parameters.Count > 0) || !PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out page))
				page = 1;

			PaginationTools.SendPage(args.Player, page, cmds,
				new PaginationTools.Settings
				{
					HeaderFormat = "Classes ({0}/{1}):",
					FooterFormat = "Type /class help {0} for more."
				});
		}
	}
}
