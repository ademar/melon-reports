using System.IO;
using Melon.Pdf.Objects;

namespace Melon.Pdf.Imaging
{
	public class JpgImage : AbstractImage
	{
		//TODO:remember people can't call the constructor directly
		
		// JPEG markers are one 0xFF byte followed by one of this

		public const int  M_APP0 = 0xe0,// application-specific marker
						  M_APPE = 0xee,// adobe specific marker
						  M_SOF0 = 0xc0,// only SOF0-SOF2 are now in common use
						  M_SOF1 = 0xc1,
						  M_SOF2 = 0xc2,
						  M_SOF3 = 0xc3,	
						  M_SOF5 = 0xc5,// codes C4 and CC are NOT SOF markers 
						  M_SOF6 = 0xc6,	
						  M_SOF7 = 0xc7,
						  M_SOF9 = 0xc9,	
						  M_SOF10= 0xca,
						  M_SOF11= 0xcb,
						  M_SOF13= 0xcd,
						  M_SOF14= 0xce,
						  M_SOF15= 0xcf,
						  M_SOI  = 0xD8,// beginning of datastream
						  M_SOS  = 0xDA;// begins compressed data

		private readonly byte[] ADOBECODE = {0x41,0x64,0x6f,0x62,0x65};

		//used to iterate the bitmap array, already know what first 2 bytes read!
		private int m_index = 2 ; 

		public JpgImage(string href):base(href){}

		
		/// <summary>
		/// The algorithm used here was basically taken from "rdjpgcom.c" in the JPEG library 
		/// of the Independent JPEG Group. 
		/// It may not be the fastest, but certainly is the simplest!.
		/// </summary>
		protected override void LoadImage()
		{
			FileStream ImageHolder = new FileStream(m_href,FileMode.Open,FileAccess.Read,FileShare.Read,8192);
			
			//fill the bitmap
			m_bitmaps = new byte[ImageHolder.Length];
			
			ImageHolder.Read(m_bitmaps,0,(int)ImageHolder.Length);
			ImageHolder.Close();

			m_bitsPerPixel = 8 ;
			m_isTransparent = false ;

			//add DTC filter
			m_filter = new DCTFilter();

			for(;;)
			{
				int marker = NextMarker();

				switch(marker)
				{
					case M_APPE:
						CheckForAdobe();
						break;
					case M_SOF0:// only SOF0-SOF2 are now in common use
					case M_SOF1:
					case M_SOF2:
					case M_SOF3:
					case M_SOF5:
					case M_SOF6:
					case M_SOF7:
					case M_SOF9:
					case M_SOF10:
					case M_SOF11:
					case M_SOF13:
					case M_SOF14:
					case M_SOF15:
						ProcSOFMarker();
						break;
					case M_SOS:
						return ;//it is the end let's go!
					default :
						SkipVar();
						break;
					
				}
			}
		}

		protected int NextMarker()
		{
			int c = ReadOneByte();
			/* Get marker code byte, swallowing any duplicate FF bytes.*/
			while(c!= 0xFF) c = ReadOneByte();
			do c = ReadOneByte(); while (c == 0xff);

			return c ;
		}

		private void SkipVar()
		{
			//Skip over an unknown or uninteresting variable-length marker
			int length = ReadTwoBytes();
			length -=2;
			m_index += length;
		}

		

		private void ProcSOFMarker()
		{
			ReadTwoBytes();
			
			ReadOneByte();

			m_height = ReadTwoBytes();
			m_width  = ReadTwoBytes();

			int num_components = ReadOneByte(); //it is the colorspace

			if (num_components == 1)
			{
				m_colorSpace = new ColorSpace(ColorDevice.DeviceGray);
			}
			else if (num_components == 3)
			{
				m_colorSpace = new ColorSpace(ColorDevice.DeviceRGB);
			}
			else if (num_components == 4)
			{
				m_colorSpace = new ColorSpace(ColorDevice.DeviceCMYK);
			}
			else
			{
				 throw new ImageFormatException();
			}
			
			//jump the rest
			m_index += num_components*3 ;
	
		}

		private void CheckForAdobe()
		{
			int len = ReadTwoBytes();
			if (len>12)
			{
				m_adobeFlag = true ;

				for(int i=0;i<ADOBECODE.Length;i++)
				{
					if (ADOBECODE[i]!=m_bitmaps[m_index++])
					{
						m_adobeFlag = false ;
						break;
					}
				}
				
			}
		}
		private int ReadOneByte()
		{
			return m_bitmaps[m_index++];
		}
		private int ReadTwoBytes()
		{
			int firstByte = m_bitmaps[m_index++];
			int secondByte = m_bitmaps[m_index++];

			return (( firstByte << 8 ) + secondByte);
		}
		
		
	}
}
