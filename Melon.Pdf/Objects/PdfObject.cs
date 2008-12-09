// created on 3/13/2002 at 3:29 PM
using System.Globalization;
using System.IO;
using System.Text;

namespace Melon.Pdf.Objects
{
	public abstract class PdfObject
	{
		protected int Generation;

		protected PdfObject(int number)
		{
			Number = number;
		}

		public int Number { get; set; }

		public string Reference
		{
			get { return string.Format(CultureInfo.InvariantCulture, "{0} {1} R", Number, Generation); }
		}


		public abstract string ToPdf();

		public virtual int Output(Stream stream)
		{
			var buffer = (new ASCIIEncoding()).GetBytes(ToPdf());
			stream.Write(buffer, 0, buffer.Length);
			stream.Flush();
			return buffer.Length;
		}
	}
}