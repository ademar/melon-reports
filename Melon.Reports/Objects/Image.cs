namespace Melon.Reports.Objects
{
	public class Image : BasicElement
	{
		public int x, y;
		public int height, width;
		public string url;

		public Image(string url, int x, int y)
		{
			this.x = x;
			this.y = y;
			this.url = url;
		}

		public ImageName IName { get; set; }
	}
}