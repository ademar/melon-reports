using System;

namespace Melon.Pdf.Imaging
{
	public class ImageFormatException : Exception
	{
		public ImageFormatException()
		{
		}

		public ImageFormatException(string msg):base(msg)
		{
		}
	}
}
