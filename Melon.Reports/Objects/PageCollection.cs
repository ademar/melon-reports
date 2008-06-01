using System;
using System.Collections;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for PageCollection.
	/// </summary>
	public class PageCollection
	{
		ArrayList pages = new ArrayList();

		public PageCollection()
		{}

		public void AddPage(Page page)
		{
			this.pages.Add(page);
		}

		public ArrayList Pages 
		{
			get 
			{
				return this.pages ;
			}
		}
	}
}
