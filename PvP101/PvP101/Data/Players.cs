using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PvP101.Structure;
using TShockAPI.DB;

namespace PvP101.Data
{
	class Players
	{
		public static bool Save(int AccountID, PlayerData data)
		{
			QueryResult result = Main.DB.QueryReader("SELECT COUNT(*) AS PlayerCount FROM Players WHERE Account = @0", AccountID);

			result.Read();
			if (result.Get<int>("PlayerCount") == 0)
			{
				result.Connection.Close();
				Main.DB.Query("INSERT INTO Players (Account, CurrentClass) VALUES (@0, @1)",
				   AccountID, data.CurrentClass);
			}
			else
			{
				result.Connection.Close();
				Main.DB.Query("UPDATE Players SET CurrentClass = @1 WHERE Account = @0",
				   AccountID, data.CurrentClass);
			}
			return true;
		}

		public static PlayerData Get(int AccountID)
		{
			QueryResult result = Main.DB.QueryReader("SELECT * FROM Players WHERE Account = @0", AccountID);
			PlayerData data = null;
			if (result.Reader.FieldCount > 0)
			{
				result.Read();
				data = new PlayerData();
				data.CurrentClass = result.Get<string>("CurrentClass");
				result.Connection.Close();
			}
			return data;
		}
	}
}
