using System.Collections;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for BandCollection.
	/// </summary>
	public class BandCollection
	{
		public BandCollection()
		{
			Bands = new ArrayList();
		}

		public void AddBand(Band band)
		{
			Bands.Add(band);
			Height += band.Height;
		}

		public ArrayList Bands { get; private set; }

		public int Height { get; private set; }
	}
}