using System.Collections.Generic;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for BandCollection.
	/// </summary>
	public class BandCollection
	{
		public BandCollection()
		{
			Bands = new List<Band>();
		}

		public void AddBand(Band band)
		{
			Bands.Add(band); Height += band.Height;
		}

		public IList<Band> Bands { get; private set; }

		public int Height { get; private set; }
	}
}