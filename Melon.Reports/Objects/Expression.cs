using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Expression.
	/// </summary>
	public class Expression : BasicElement
	{
		int m_x,m_y ;
		int m_fontSize ;
		string content ; // esto es por el momento
		object m_value ;
		string type ;

		public Expression(string content)
		{
			this.content = content;
		}

		public int X 
		{
			get
			{
				return this.m_x ;
			}
			set 
			{
				this.m_x = value;
			}
		}
		public int Y
		{
			get
			{
				return this.m_y;
			}
			set 
			{
				this.m_y = value;
			}
		}

		public int FontSize 
		{
			get
			{
				return this.m_fontSize ;
			}
			set 
			{
				this.m_fontSize = value ;
			}
		}
		public string Content 
		{
			get 
			{
				return this.content ;
			}
			set
			{
				this.content = value ;
			}
		}
		public object Value 
		{
			get 
			{
				return this.m_value;
			}
			set 
			{
				this.m_value = value ;
			}
		}
		public string Type 
		{
			set 
			{
				this.type = value ;
			}
			get
			{
				return this.type;
			}
		}
	}
}
