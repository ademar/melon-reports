// created on 3/21/2002 at 3:03 PM
using System.IO;
using System.Text;
using Melon.Pdf.Imaging;

namespace Melon.Pdf.Objects
{
	public class PdfImage : PdfObject
	{
		private readonly AbstractImage Image;

		private readonly int imgnumber;

		public PdfImage(int number, int imgnumber, AbstractImage img) : base(number)
		{
			this.imgnumber = imgnumber;
			Image = img;
		}

		public override string ToPdf()
		{
			return null;
		}

		public string Name
		{
			get { return "Im" + imgnumber; }
		}

		public double Width
		{
			get { return Image.Width; }
		}

		public double Height
		{
			get { return Image.Height; }
		}

		public override int Output(Stream stream)
		{
			var length = 0;

			var imgData = new PdfStream(0);
			imgData.AddData(Image.GetBitmaps());

			//apply filters if any
			if (Image.Filter != null) imgData.AddFilter(Image.Filter);

			var strFilters = imgData.ApplyFilters();

			var sb = new StringBuilder();

			sb.Append(Number);
			sb.Append(" ");
			sb.Append(Generation);
			sb.Append(" obj\n<< /Type /XObject\n/Subtype /Image\n/Name /Im");
			sb.Append(imgnumber);

			sb.Append("\n/Length ");
			sb.Append(imgData.Length);
			sb.Append("\n/Width ");
			sb.Append(Image.Width);
			sb.Append("\n/Height ");
			sb.Append(Image.Height);
			sb.Append("\n/BitsPerComponent ");
			sb.Append(Image.BitsPerPixel);
			sb.Append("\n/ColorSpace ");
			sb.Append(Image.ColorSpace.GetRepresentation());

			if (Image.IsAdobe && Image.ColorSpace.ColorDevice == ColorDevice.DeviceCMYK)
			{
				//photoShop CMYK values are inverted
				sb.Append("\n/Decode [ 1.0 0.0 1.0 0.0 1.0 0.0 1.1 0.0 ]\n");
			}

			sb.Append("\n");
			sb.Append(strFilters);
			sb.Append(">>\n");


			var encoder = new ASCIIEncoding();

			var bt = encoder.GetBytes(sb.ToString());
			stream.Write(bt, 0, bt.Length);
			stream.Flush();
			length += bt.Length;

			length += imgData.outputPDFStream(stream);
			bt = encoder.GetBytes("endobj\n");
			stream.Write(bt, 0, bt.Length);
			stream.Flush();
			length += bt.Length;
			return length;
		}
	}
}