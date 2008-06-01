// created on 3/21/2002 at 12:20 PM
using System;
using System.IO;
using Melon.Pdf.Objects;

namespace Melon.Pdf.Imaging{
	public abstract class AbstractImage{
		
		protected int m_width = 0;
		protected int m_height = 0;
		protected string m_href = null;
		protected Uri m_uri = null ;
		
		protected byte[] m_bitmaps = null;
		
		protected int m_bitmapsSize = 0;
		
		protected bool m_isTransparent = false;
		
		protected int m_transparentColor = 0;
		
		protected int m_bitsPerPixel = 0 ;

		protected bool m_adobeFlag = false ;
		
		protected ColorSpace m_colorSpace = null ;
		protected Filter m_filter = null ;
		
				
		public AbstractImage(string href){
			m_href = href;
			m_uri =  new Uri(href);
		}

		public AbstractImage(Uri the_uri)
		{
			m_uri = the_uri;
		}
		
		protected abstract void LoadImage();
		
		public string GetURL{
			get {
				return m_href;
			}
		}
		public Uri Uri 
		{
			get
			{
				return m_uri ;
			}
		}
		public int Width {
			get {
				if (m_width==0) LoadImage();
				return m_width;
			}
		}
		public int Height {
			get {
				if (m_height==0) LoadImage();
				return m_height;
			}
		}
		public int BitsPerPixel {
			get {
				if (m_bitsPerPixel==0) LoadImage();
				return m_bitsPerPixel;
			}
		}
		public bool IsTransparent
		{
			get
			{
				return m_isTransparent ;
			}
		}
		public int TransparentColor 
		{
			get
			{
				return m_transparentColor ;
			}
		}
		public bool IsAdobe 
		{
			get
			{
				return m_adobeFlag ;
			}
		}
		public Filter Filter 
		{
			get
			{
				return m_filter;
			}
		}

		public ColorSpace ColorSpace 
		{
			get
			{
				return m_colorSpace ;
			}
		}

		public byte[] GetBitmaps() {

			if (m_bitmaps == null ) LoadImage();

			return m_bitmaps;
		}

		public static AbstractImage Make(string href)
		{
			//TODO:Consider using a generic URI
			// -- don't like this way
			FileStream ImageHolder;
			try
			{
				ImageHolder = new FileStream(href,FileMode.Open,FileAccess.Read,FileShare.Read,8192);
			}
			catch(ArgumentException )
			{
				throw new ImageFormatException("Invalid image URI: " + href);
			}
			byte[] header = new byte[2] ;
			ImageHolder.Read(header,0,2);
			ImageHolder.Close();
			if (header[0]==0x47/*G*/ && header[1]==0x49/*I*/)
			{
				return new GifImage(href);

			}
			else if (header[0]==0xFF && header[1]==JpgImage.M_SOI)
			{
				return new JpgImage(href);
			}
			else throw new ImageFormatException();
		}

		protected static int toShort(int firstByte,int secondByte)
		{
			return ((firstByte << 8 ) + secondByte);
		}
		
		protected static int toShortInverted(int firstByte,int secondByte)
		{
			return ((secondByte<< 8 ) + firstByte );
		}
	}
}
