using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Text.
	/// </summary>
	public class Text : BasicElement
	{
		public enum Alignment
		{
			Right,
			Center,
			Left,
            Justified
		}
		string m_text ;
		Alignment m_alignment;
		int m_x,m_y ;
		int m_fontSize ;
		public Color color ;

		public Text()
		{}
		/**/
		public Text(string text)
		{
			this.m_text = text ;
		}
		/**/
		public Text(string text,Alignment align,int x, int y)
		{
			this.m_text = text ;
			this.m_alignment = align ;
			this.m_x = x ;
			this.m_y = y ;
		}
		/**/
		public Alignment Align
		{
			get 
			{
				return this.m_alignment;
			}
			set
			{
				this.m_alignment = value;
			}
		}
		public string Label 
		{
			get
			{
				return this.m_text;
			}
		}
		public int X 
		{
			get
			{
				return this.m_x ;
			}
		}
		public int Y
		{
			get
			{
				return this.m_y;
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
	}
}
