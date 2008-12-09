using System.Collections;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Page.
	/// </summary>
	public class Page
	{
		public Page()
		{
			Elements = new ArrayList();
		}

		public Page(int height, int width)
		{
			Elements = new ArrayList();
			Height =  height ;
			Width = width ;
		}

		public void AddElement(BasicElement e,int h)
		{
			e.H = h ;
			Elements.Add(e);
		}

		public ArrayList Elements { get; private set; }

		public int Height { get; set; }

		public int Width { get; set; }

		public void PutBands(BandCollection bands,ref int h)
		{
			
			var it = bands.Bands.GetEnumerator();
			while(it.MoveNext())
			{
				
				PutBand((Band)it.Current,h);
				h -= ((Band)it.Current).Height;
			}
			
		}

		public void PutBand(Band band,int h)
		{
			var it = band.Elements.GetEnumerator();

			while(it.MoveNext())
			{
				var current = it.Current ;

				if (current is Text)
				{
					var t1 = (Text)it.Current ;
					var t2 = new Text(t1.Label,t1.Align,t1.X,t1.Y) {FontSize = t1.FontSize, color = t1.color};
					
					AddElement(t2,h);

				}
				else if (current is Expression)
				{
					var e1 = (Expression)it.Current;
					
					var t2 = new Text(e1.Value.ToString(),Text.Alignment.Left,e1.X,e1.Y)
					         	{
					         		FontSize = e1.FontSize,
					         		color = new Color("#000000")
					         	};
					AddElement(t2,h);
				}
				else if (current is Image)
				{
					var i1 = (Image)it.Current;
					AddElement(i1,h);
				}
				else if (current is Rectangle)
				{
					var r1 = (Rectangle)it.Current;
					var r2 = new Rectangle
					         	{
					         		x = r1.x,
					         		y = r1.y,
					         		height = r1.height,
					         		width = r1.width,
					         		bordercolor = r1.bordercolor,
					         		fillcolor = r1.fillcolor
					         	};

					AddElement(r2,h);
				}
				else if (current is Bookmark)
				{
					AddElement((Bookmark)current,h);
				}
				
			}
		}
	}
}
