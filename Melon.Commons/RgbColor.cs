using System;
using System.Globalization;

namespace Melon.Commons
{
	public class RgbColor
	{
		public RgbColor()
		{
			Blue = "0.0";
			Green = "0.0";
			Red = "0.0";
		}

		public RgbColor(System.Drawing.Color color)
		{
			Red = FixDouble((double)color.R / 255); //TODO : check pdf literals definition
			Green = FixDouble((double)color.G / 255);
			Blue = FixDouble((double)color.B / 255);
		}

		public RgbColor(string color)
			: this(System.Drawing.Color.FromArgb(Convert.ToInt32(color.Substring(1), 16)))
		{
			
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