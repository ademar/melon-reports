using System;
using System.Globalization;

namespace Melon.Reports
{
	/// <summary>
	/// Summary description for Color.
	/// </summary>
	public class Color
	{
		//double red =0.0,green=0.0,blue=0.0 ;

		string red="0.0",green="0.0",blue="0.0" ;
		
		public Color()
		{			
		}
		public Color (string color)
		{
			System.Drawing.Color c  = System.Drawing.Color.FromArgb(Convert.ToInt32(color.Substring(1),16));

			red   = FixDouble((double)c.R / 255); //TODO : check pdf literals definition
			green = FixDouble((double)c.G / 255); 
			blue  = FixDouble((double)c.B / 255);


		}

		private string FixDouble(double dbl)
		{
			NumberFormatInfo formater = new NumberFormatInfo();
			formater.NumberDecimalSeparator = "." ;
			
			String str = dbl.ToString(formater);
			int dec = str.IndexOf(".");

			if (dec!=-1)
			{
				if ((str.Length - dec)>6) str = str.Substring(0,dec+6);
				return str;
			}
			return str;
		}
		public string Red 
		{
			get
			{
				return this.red;
			}
		}
		public string Green 
		{
			get
			{
				return this.green;
			}
		}
		public string Blue
		{
			get
			{
				return this.blue;
			}
		}
	}
}
