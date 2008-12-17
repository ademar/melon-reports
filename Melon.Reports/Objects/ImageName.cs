namespace Melon.Reports.Objects
{
	public class ImageName
	{
		public ImageName(string name)
		{
			Name = name;
		}

		public ImageName()
		{
			Name = string.Empty;
		}

		public string Name { get; set; }
	}
}