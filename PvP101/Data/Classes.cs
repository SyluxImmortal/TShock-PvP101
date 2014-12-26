using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PvP101.Structure;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace PvP101.Data
{
	class Classes
	{
		public static bool Save(CharacterData classData)
		{
			QueryResult result = Main.DB.QueryReader("SELECT COUNT(*) AS ClassCount FROM Classes WHERE Title = @0", classData.Title);

			result.Read();
			if (result.Get<int>("ClassCount") == 0)
			{
				result.Connection.Close();
				Main.DB.Query("INSERT INTO Classes (Title, HP, MP, Inventory, Description, Category) VALUES (@0, @1, @2, @3, @4, @5)",
				   classData.Title, classData.HP, classData.MP, classData.Inventory, classData.Description, classData.Category);
				Main.DB.Close();
			}
			else
			{
				result.Connection.Close();
				Main.DB.Query("UPDATE Classes SET HP = @1, MP = @2, Inventory = @3, Description = @4, Category = @5 WHERE Title = @0",
				   classData.Title, classData.HP, classData.MP, classData.Inventory, classData.Description, classData.Category);
				Main.DB.Close();
			}
			return true;
		}
		public static bool UpdateDescription(string title, string description)
		{
			Main.DB.Query("UPDATE Classes SET Description = @1 WHERE Title = @0",
			   title, description);
			Main.DB.Close();
			return true;
		}
		public static bool UpdateCategory(string title, string category)
		{
			Main.DB.Query("UPDATE Classes SET Category = @1 WHERE Title = @0",
			   title, category);
			Main.DB.Close();
			return true;
		}
		public static bool Delete(string title)
		{
			Main.DB.Query("DELETE FROM Classes WHERE Title = @0", title);
			Main.DB.Close();
			return true;
		}
		public static CharacterData Get(string title)
		{
			QueryResult result = Main.DB.QueryReader("SELECT * FROM Classes WHERE Title = @0", title);
			CharacterData data = null;
			if (result.Reader.FieldCount > 0)
			{
				result.Read();
				data = new CharacterData();
				data.Title = result.Get<string>("Title");
				data.HP = result.Get<int>("HP");
				data.MP = result.Get<int>("MP");
				data.Inventory = result.Get<string>("Inventory");
				data.Description = result.Get<string>("Description");
				data.Category = result.Get<string>("Category");
				result.Connection.Close();
			}
			return data;
		}
		public static string GetDescription(string title)
		{
			QueryResult result = Main.DB.QueryReader("SELECT Description FROM Classes WHERE Title = @0", title);
			string data = null;
			if (result.Reader.FieldCount > 0)
			{
				result.Read();
				data = result.Get<string>("Description");
				result.Connection.Close();
			}
			return data;
		}
		public static List<string> GetList()
		{
			QueryResult result = Main.DB.QueryReader("SELECT Title,Description FROM Classes");
			List<string> data = new List<string>();
			while (result.Reader.Read())
			{
				if (!result.Reader.IsDBNull(0))
				{
					data.Add(String.Format("{0} - {1}", result.Get<string>("Title"), result.Get<string>("Description")));
				}
			}
			result.Connection.Close();

			return data;
		}
		public static List<string> GetList(string category)
		{
			QueryResult result = Main.DB.QueryReader("SELECT Title,Description FROM Classes WHERE Category = @0 AND Category <> ''",category);
			List<string> data = new List<string>();
			while (result.Reader.Read())
			{
				if (!result.Reader.IsDBNull(0))
				{
					data.Add(String.Format("{0} - {1}", result.Get<string>("Title"), result.Get<string>("Description")));
				}
			}
			result.Connection.Close();

			return data;
		}
		public static List<string> GetCategoryList()
		{
			QueryResult result = Main.DB.QueryReader("SELECT Category FROM Classes GROUP BY Category");
			List<string> data = new List<string>();
			while (result.Reader.Read())
			{
				if (!result.Reader.IsDBNull(0))
				{
					if (result.Get<string>("Category") != String.Empty)
					{
						data.Add(result.Get<string>("Category"));
					}
				}
			}
			result.Connection.Close();

			return data;
		}
		public static CharacterData FromCurrent(TSPlayer player)
		{
			CharacterData data = new CharacterData();
			data.HP = player.TPlayer.statLifeMax;
			data.MP = player.TPlayer.statManaMax;
			StringBuilder sb = new StringBuilder();
			foreach (var item in player.PlayerData.inventory)
			{
				sb.Append(item.netID);
				sb.Append('.');
				sb.Append(item.stack);
				sb.Append('.');
				sb.Append(item.prefix);
				sb.Append('~');
			}
			sb.Length--;
			data.Inventory = sb.ToString();
			data.Title = "None";
			data.Description = "No description yet";
			data.Category = "";
			return data;
		}
		public static void ToCurrent(TSPlayer player, CharacterData classData)
		{
			Item item = null;
			string[] itemData = null;
			string[] data = null;

			player.TPlayer.statMana = classData.MP;
			player.TPlayer.statManaMax = classData.MP;
			player.TPlayer.statLife = classData.HP;
			player.TPlayer.statLifeMax = classData.HP;

			data = classData.Inventory.Split('~');
			for (int i = 0; i < NetItem.maxNetInventory; i++)
			{
				itemData = data[i].Split('.');
				item = TShock.Utils.GetItemById(Convert.ToInt32(itemData[0]));
				item.stack = Convert.ToInt32(itemData[1]);
				item.prefix = Convert.ToByte(itemData[2]);

				if (i < NetItem.maxNetInventory - NetItem.armorSlots - NetItem.dyeSlots)
				{
					player.TPlayer.inventory[i] = item;
				}
				else if (i < NetItem.maxNetInventory - NetItem.dyeSlots)
				{
					player.TPlayer.armor[i - (NetItem.maxNetInventory - NetItem.armorSlots - NetItem.dyeSlots)] = item;
				}
				else
				{
					player.TPlayer.dye[i - (NetItem.maxNetInventory - NetItem.dyeSlots)] = item;
				}
				NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, string.Empty, player.Index, i);
			}

			NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, -1, player.Name, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData((int)PacketTypes.PlayerHp, -1, -1, "", player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData((int)PacketTypes.PlayerMana, -1, -1, "", player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData((int)PacketTypes.PlayerBuff, -1, -1, "", player.Index, 0f, 0f, 0f, 0);
		}
	}
}
