using System;
using System.Collections;

namespace Melon.Reports.Objects
{
	public class Band
	{
		private int m_height;
		private ArrayList ElementList = new ArrayList();

		public Report parent = null;

		public Band(){}

		public Band(int height)
		{
			this.m_height = height;
		}

		public void AddElement(BasicElement e)
		{
			this.ElementList.Add(e);
		}

		public int Height 
		{
			get 
			{
				return this.m_height;
			}
		}

		public ArrayList Elements 
		{
			get 
			{
				return this.ElementList ;
			}
		}
	}
}
