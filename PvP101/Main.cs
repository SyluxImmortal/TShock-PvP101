using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;

namespace PvP101
{
	[ApiVersion(1, 16)]
	public class Main : TerrariaPlugin
	{
		#region Plugin Info

		public override Version Version
		{
			get { return new Version("0.0.0"); }
		}
		public override string Name
		{
			get { return "PvP101"; }
		}
		public override string Author
		{
			get { return "TheWall"; }
		}
		public override string Description
		{
			get { return "All things pvp. Arenas, classes, kill counts, and blastzones"; }
		}
		public Main(Terraria.Main game)
			: base(game)
		{
			Order = 999;
		}
		#endregion

		public override void Initialize()
		{
			Commands.Classes.Init();
			Commands.Utility.Init();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			base.Dispose(disposing);
		}
	}
}
