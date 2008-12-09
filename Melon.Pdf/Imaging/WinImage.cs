// created on 3/21/2002 at 12:33 PM
namespace Melon.Pdf.Imaging
{
	using System.Drawing;

	public class WinImage : AbstractImage
	{
		public WinImage(string href) : base(href)
		{
		}

		protected override void LoadImage()
		{
			var ImageHolder = new Bitmap(m_href);

			m_height = ImageHolder.Height;
			m_width = ImageHolder.Width;

			m_bitsPerPixel = 8; //forced for GIF
			m_colorSpace = new ColorSpace(ColorDevice.DeviceRGB);

			m_isTransparent = false;

			m_bitmapsSize = m_width*m_height*3;
			m_bitmaps = new byte[m_bitmapsSize];

			for (int i = 0; i < m_height; i++)
			{
				//indexes were swapped
				for (int j = 0; j < m_width; j++)
				{
					Color pixelColor = ImageHolder.GetPixel(j, i);
					m_bitmaps[3*(i*m_width + j) + 0] = (byte) (pixelColor.R & 0xFF);
					m_bitmaps[3*(i*m_width + j) + 1] = (byte) (pixelColor.G & 0xFF);
					m_bitmaps[3*(i*m_width + j) + 2] = (byte) (pixelColor.B & 0xFF);
				}
			}
		}
	}
}