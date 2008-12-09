namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Bookmark.
	/// </summary>
	public class Bookmark : BasicElement
	{
		public Bookmark(string varName)
		{
			VarName = varName ;
		}

		public Bookmark() : this("")
		{
		}
        
		public string VarName { get; set; }
	}
}
