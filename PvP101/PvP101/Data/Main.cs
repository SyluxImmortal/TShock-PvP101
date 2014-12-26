using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using TShockAPI;
using MySql.Data.MySqlClient;
using TShockAPI.DB;

namespace PvP101.Data
{
	class Main
	{
		private static IDbConnection _db;
		public static IDbConnection DB
		{
			get { Check(); return _db; }
		}

		private static void Check()
		{
			if (_db == null)
			{
				string sql = Path.Combine(TShock.SavePath, "PvP101.sqlite");
				_db = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));

				SqlTableCreator sqlcreator = new SqlTableCreator(_db, _db.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
				sqlcreator.EnsureExists(new SqlTable("Classes",
					new SqlColumn("Title", MySqlDbType.VarChar) { Primary = true, Unique = true },
					new SqlColumn("HP", MySqlDbType.Int32),
					new SqlColumn("MP", MySqlDbType.Int32),
					new SqlColumn("Inventory", MySqlDbType.VarChar),
					new SqlColumn("Description", MySqlDbType.VarChar),
					new SqlColumn("Category", MySqlDbType.VarChar)
					));

				sqlcreator.EnsureExists(new SqlTable("CharacterBackups",
					new SqlColumn("Account", MySqlDbType.Int32) { Primary = true, Unique = true },
					new SqlColumn("HP", MySqlDbType.Int32),
					new SqlColumn("MP", MySqlDbType.Int32),
					new SqlColumn("Inventory", MySqlDbType.VarChar)));

				sqlcreator.EnsureExists(new SqlTable("Players",
					new SqlColumn("Account", MySqlDbType.Int32),
					new SqlColumn("CurrentClass", MySqlDbType.VarChar)));
			}
		}

	}
}
