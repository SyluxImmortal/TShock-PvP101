using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvP101.Utils
{
	class Math
	{
		public static bool IsInteger(string text)
		{
			int output;
			return int.TryParse(text, out output);
		}
		public static bool IsDouble(string text)
		{
			double output;
			return double.TryParse(text, out output);
		}
		public static bool IsFloat(string text)
		{
			float output;
			return float.TryParse(text, out output);
		}
		public static bool IsNumeric(string text)
		{
			return IsInteger(text) || IsDouble(text) || IsFloat(text);
		}
	}
}
