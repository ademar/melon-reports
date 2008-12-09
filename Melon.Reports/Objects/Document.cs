using System;
using System.Collections;

namespace Melon.Reports.Objects
{
	public class Document
	{
		readonly PageCollection pages = new PageCollection();

		public Document()
		{
			Fonts = new ArrayList();
		}

		public Document(int height, int width)
		{
			Fonts = new ArrayList();
			Height =  height ;
			Width = width ;
		}
		
		public Page AddPage()
		{
			var page = new Page(Height,Width);
			pages.AddPage(page);
			return page;
		}

		public int Height { get; set; }

		public int Width { get; set; }

		public ArrayList Fonts { get; set; }

		public Array Images { get; set; }

		public ArrayList Pages 
		{
			get
			{
				return pages.Pages ;
			}
		}


	}
}
