using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using PvP101.Structure;
using TShockAPI.DB;
using TShockAPI;

namespace PvP101.Data
{
	class CharacterBackups
	{
		public static bool Save(int AccountID, CharacterData classData)
		{
			QueryResult result = Main.DB.QueryReader("SELECT COUNT(*) AS BackupCount FROM CharacterBackups WHERE Account = @0", AccountID);

			result.Read();
			int value = result.Get<int>("BackupCount");
			result.Connection.Close();
			if (value == 0)
			{
				Main.DB.Query("INSERT INTO CharacterBackups (Account, HP, MP, Inventory) VALUES (@0, @1, @2, @3)",
					AccountID, classData.HP, classData.MP, classData.Inventory);
				Main.DB.Close();
			}
			else
			{
				Main.DB.Query("UPDATE CharacterBackups SET HP = @1, MP = @2, Inventory = @3 WHERE Account = @0",
					AccountID, classData.HP, classData.MP, classData.Inventory);
				Main.DB.Close();
			}
			return true;
		}
		public static bool Delete(int AccountID)
		{
			Main.DB.Query("DELETE FROM CharacterBackups WHERE Account = @0", AccountID);
			Main.DB.Close();
			return true;
		}
		public static CharacterData Get(int AccountID)
		{
			QueryResult result = Main.DB.QueryReader("SELECT * FROM CharacterBackups WHERE Account = @0", AccountID);
			CharacterData data = null;
			if (!result.Reader.IsDBNull(0))
			{
				result.Read();
				data = new CharacterData();
				data.Title = result.Get<string>("Title");
				data.HP = result.Get<int>("HP");
				data.MP = result.Get<int>("MP");
				data.Inventory = result.Get<string>("Inventory");
				result.Connection.Close();
			}
			return data;
		}
	}
}
