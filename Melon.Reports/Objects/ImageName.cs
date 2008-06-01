using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for ImageName.
	/// </summary>
	public class ImageName
	{
		string name = "" ;
		public ImageName(){
		}

		public ImageName(string name)
		{
			this.name =  name ;
		}
		public string Name 
		{
			get 
			{
				return this.name ;
			}
			set 
			{
				this.name = value ;
			}
		}
	}
}
