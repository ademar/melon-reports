using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Bookmark.
	/// </summary>
	public class Bookmark : BasicElement
	{
		string varName ;
		Bookmark parent = null ;
		public Bookmark(string varName)
		{
			this.varName = varName ;
		}
		public Bookmark(string varName,Bookmark parent)
		{
			this.varName = varName ;
			this.parent = parent;
		}

		public string VarName 
		{
			get 
			{
				return varName ;
			}
			set 
			{
				varName = value ;
			}
		}
	}
}
