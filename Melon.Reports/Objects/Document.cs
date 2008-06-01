using System;
using System.Collections;

namespace Melon.Reports.Objects
{
	public class Document
	{
		readonly PageCollection pages = new PageCollection();

		ArrayList fonts = new ArrayList();

		Array images ; 
		int m_height ;
		int m_width ;

		public Document(){} 

		public Document(int height, int width)
		{
			m_height =  height ;
			m_width = width ;
		}
		
		public Page AddPage()
		{
			Page page = new Page(m_height,m_width);
			pages.AddPage(page);
			return page;
		}
		
		public int Height
		{
			get 
			{
				return m_height ;
			}
			set
			{
				m_height = value ;
			}
		}
		
		public int Width
		{
			get
			{
				return m_width ;
			}
			set 
			{
				m_width = value ;
			}
		}
		
		public ArrayList Fonts 
		{
			get
			{
				return fonts;
			}
			set 
			{
				fonts = value ;
			}
		}
		
		public Array Images
		{
			get
			{
				return images;
			}
			set 
			{
				images = value ;
			}
		}
		
		public ArrayList Pages 
		{
			get
			{
				return pages.Pages ;
			}
		}


	}
}
