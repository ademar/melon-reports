using System.Collections;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for PageCollection.
	/// </summary>
	public class PageCollection
	{
		public PageCollection()
		{
			Pages = new ArrayList();
		}

		public void AddPage(Page page)
		{
			Pages.Add(page);
		}

		public ArrayList Pages { get; private set; }
	}
}