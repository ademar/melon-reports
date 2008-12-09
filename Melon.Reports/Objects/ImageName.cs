namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for ImageName.
	/// </summary>
	public class ImageName
	{
		public ImageName()
		{
			Name = "";
		}

		public ImageName(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}