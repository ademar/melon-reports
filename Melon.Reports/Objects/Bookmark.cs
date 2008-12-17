namespace Melon.Reports.Objects
{
	public class Bookmark : BasicElement
	{
		public Bookmark(string varName)
		{
			VarName = varName;
		}

		public Bookmark() : this("")
		{
		}

		public string VarName { get; set; }
	}
}