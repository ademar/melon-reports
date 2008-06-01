namespace Melon.Reports.Objects
{
	public class Image : BasicElement 
	{
		public int x,y ;
		public int height,width ;
		public string url ;
//		public string name = null ;
		ImageName iname ;
		public Image(string url ,int x, int y)
		{
			this.x=x;
			this.y=y;
			this.url = url ;
			//iname = new ImageName();
		}
		public ImageName IName 
		{
			get 
			{
				return iname ;
			}
			set 
			{
				iname = value ; 
			}
		}

	}
}
