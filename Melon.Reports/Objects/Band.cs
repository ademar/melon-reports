using System.Collections.Generic;

namespace Melon.Reports.Objects
{
	public class Band
	{
		public Report parent;

		public Band()
		{
			Elements = new List<BasicElement>();
		}

		public Band(int height):this()
		{
			Height = height;
		}

		public int Height { get; private set; }
        public IList<BasicElement> Elements { get; private set; }
	}
}