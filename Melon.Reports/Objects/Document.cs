using System;
using System.Collections.Generic;


namespace Melon.Reports.Objects
{
	public class Document
	{
		public Document()
		{
			Pages = new List<Page>();
			Fonts = new List<Font>();
		}

		public Document(int height, int width):this()
		{
			Height = height;
			Width = width;
		}

		public Page CreatePage()
		{
			var page = new Page(Height, Width);
			Pages.Add(page);
			return page;
		}

		public int Height { get; set; }

		public int Width { get; set; }

		public IList<Font> Fonts { get; set; }

		public Array Images { get; set; }

		public IList<Page> Pages { get; private set; }
	}
}