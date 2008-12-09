using Melon.Commons;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Text.
	/// </summary>
	public class Text : BasicElement
	{
		public enum Alignment
		{
			Right,
			Center,
			Left,
			Justified
		}

		public RgbColor rgbColor;

		public Text()
		{
		}

		public Text(string text)
		{
			Label = text;
		}

		public Text(string text, Alignment align, int x, int y)
		{
			Label = text;
			Align = align;
			X = x;
			Y = y;
		}

		public Alignment Align { get; set; }

		public string Label { get; private set; }

		public int X { get; private set; }

		public int Y { get; private set; }

		public int FontSize { get; set; }
	}
}