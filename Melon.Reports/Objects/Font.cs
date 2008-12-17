namespace Melon.Reports.Objects
{
	public class Font
	{
		public Font(string name, string fontName)
		{
			Name = name;
			FontName = fontName;
		}

		public string Name { get; set; }
        public string FontName { get; set; }
        public bool IsDefault { get; set; }
	}
}