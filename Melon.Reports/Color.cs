using System;
using System.Globalization;

namespace Melon.Reports
{
	/// <summary>
	/// Summary description for Color.
	/// </summary>
	public class Color
	{
		public Color()
		{
			Blue = "0.0";
			Green = "0.0";
			Red = "0.0";
		}

		public Color(string color)
		{
			System.Drawing.Color c = System.Drawing.Color.FromArgb(Convert.ToInt32(color.Substring(1), 16));

			Red = FixDouble((double) c.R/255); //TODO : check pdf literals definition
			Green = FixDouble((double) c.G/255);
			Blue = FixDouble((double) c.B/255);
		}

		private static string FixDouble(double dbl)
		{
			var formater = new NumberFormatInfo {NumberDecimalSeparator = "."};

			var str = dbl.ToString(formater);
			int dec = str.IndexOf(".");

			if (dec != -1)
			{
				if ((str.Length - dec) > 6) str = str.Substring(0, dec + 6);
				return str;
			}
			return str;
		}

		public string Red { get; private set; }

		public string Green { get; private set; }

		public string Blue { get; private set; }
	}
}