using Melon.Commons;

namespace Melon.Reports.Objects
{
	public class Text : BasicElement
	{
		public RgbColor rgbColor;

		public Text()
		{
		}

		public Text(string text)
		{
			Label = text;
		}

		public Text(string text, TextAlignment align, int x, int y)
		{
			Label = text;
			Align = align;
			X = x;
			Y = y;
		}

		public TextAlignment Align { get; set; }
        public string Label { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int FontSize { get; set; }
	}
}