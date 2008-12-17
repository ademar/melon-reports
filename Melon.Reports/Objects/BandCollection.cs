using System.Collections.Generic;

namespace Melon.Reports.Objects
{
	public class BandCollection
	{
		public BandCollection()
		{
			Bands = new List<Band>();
		}

		public void Add(Band band)
		{
			Bands.Add(band); Height += band.Height;
		}

		public IList<Band> Bands { get; private set; }

		public int Height { get; private set; }
	}
}