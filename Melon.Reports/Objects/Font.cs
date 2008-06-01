using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Font. -- Actually ,this class it is not being used 
	/// TODO : Delete it !
	/// </summary>
	public class Font
	{
		private string name = null;
		private string fontName = null;
		private bool m_isDefault = false ;
		
		public Font(string name, string fontName)
		{
			this.name = name ;
			this.fontName = fontName ;
		}

		public string Name 
		{
			get 
			{
				return this.name;
			}
			set 
			{
				this.name = value ;
			}
		}
		public string FontName 
		{
			get
			{
				return this.fontName ;
			}
			set
			{
				this.fontName = value;
			}
		}
		public bool IsDefault 
		{
			get
			{
				return this.m_isDefault;
			}
			set
			{
				this.m_isDefault = value ;
			}
		}
		
	}
}
