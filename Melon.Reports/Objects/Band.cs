using System.Collections;

namespace Melon.Reports.Objects
{
	public class Band
	{
		public Report parent;

		public Band()
		{
			Elements = new ArrayList();
		}

		public Band(int height)
		{
			Elements = new ArrayList();
			Height = height;
		}

		public void AddElement(BasicElement e)
		{
			Elements.Add(e);
		}

		public int Height { get; private set; }

		public ArrayList Elements { get; private set; }
	}
}
