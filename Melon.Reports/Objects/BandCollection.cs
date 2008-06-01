using System;
using System.Collections;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for BandCollection.
	/// </summary>
	public class BandCollection
	{
		ArrayList m_bandCollection = new ArrayList();
		int height = 0 ;

		public BandCollection()
		{}

		public void AddBand(Band band)
		{
			this.m_bandCollection.Add(band);
			height+=band.Height ;
		}

		public ArrayList Bands 
		{
			get
			{
				return m_bandCollection;
			}
		}

		public int Height 
		{
			get 
			{
				return this.height;
			}
		}
	}
}
