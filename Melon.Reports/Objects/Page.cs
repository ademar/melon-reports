using System.Collections.Generic;
using Melon.Commons;

namespace Melon.Reports.Objects
{
	public class Page
	{
		public Page()
		{
			Elements = new List<BasicElement>();
		}

		public Page(int height, int width):this()
		{
			Height = height;
			Width = width;
		}

		public void AddElement(BasicElement e, int h)
		{
			e.H = h;
			Elements.Add(e);
		}

		public IList<BasicElement> Elements { get; private set; }

		public int Height { get; set; }
        public int Width { get; set; }

		public void PutBands(BandCollection bands, ref int h)
		{
			foreach (var band in bands.Bands)
			{
				PutBand(band, h);
				h -= band.Height;
			}
		}

		public void PutBand(Band band, int h)
		{
			foreach (var current in band.Elements)
			{
				if (current is Text)
				{
					var t1 = (Text)current;
					var t2 = new Text(t1.Label, t1.Align, t1.X, t1.Y) {FontSize = t1.FontSize, rgbColor = t1.rgbColor};

					AddElement(t2, h);
				}
				else if (current is Expression)
				{
					var e1 = (Expression)current;

					var t2 = new Text(e1.Value.ToString(), TextAlignment.Left, e1.X, e1.Y)
					         	{
					         		FontSize = e1.FontSize,
					         		rgbColor = new RgbColor("#000000")
					         	};
					AddElement(t2, h);
				}
				else if (current is Image)
				{
					AddElement(current, h);
				}
				else if (current is Rectangle)
				{
					var r1 = (Rectangle)current;

					var r2 = new Rectangle
					         	{
					         		x = r1.x,
					         		y = r1.y,
					         		height = r1.height,
					         		width = r1.width,
					         		bordercolor = r1.bordercolor,
					         		fillcolor = r1.fillcolor
					         	};

					AddElement(r2, h);
				}
				else if (current is Bookmark)
				{
					AddElement(current, h);
				}
			}
		}
	}
}