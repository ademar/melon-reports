using System.IO;
using Melon.Pdf.Imaging;
using Melon.Pdf.Objects;
using NUnit.Framework;

namespace Melon.Tests
{
	[TestFixture]
	public class Pdf
	{
		[Test]
		public void Works()
		{
			var pdf = new PdfDocument();

			pdf.MakeFont("F1", PdfFontTypes.TYPE1, "Helvetica");

			var img = AbstractImage.Make(@"C:\Temp\dotnet.gif");
			pdf.AddImage(img);

			var ps = pdf.MakeStream();

			ps.BeginText();
			ps.SetFont("F1", 24);
			ps.SetTextPos(50, 550);
			ps.ShowText("a GIF image :");
			ps.EndText();

			ps.ShowImage("Im1", 250, 550, 61, 35);

			var p = pdf.MakePage(ps, 612, 792);

			pdf.MakeOutline(pdf.OutlineRoot, "root", p);

			var stream = new MemoryStream();

			pdf.outputHeader(stream);
			var offset = pdf.outputXref(stream);
			pdf.outputTrailer(stream, offset);

			var f = new FileStream(@"C:\Temp\test.pdf", FileMode.Create, FileAccess.Write);

			stream.WriteTo(f);
		}
	}
}