using System;
using System.Runtime.Serialization;

namespace Melon.Pdf.Imaging
{
	public class ImageFormatException : Exception
	{
		public ImageFormatException()
		{
		}

		public ImageFormatException(string msg) : base(msg)
		{
		}

		public ImageFormatException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ImageFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}