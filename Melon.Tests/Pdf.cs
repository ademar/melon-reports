using System.Drawing;
using System.IO;
using Melon.Commons;
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

			var font  = pdf.CreateFont(PdfFontTypes.TYPE1, "Helvetica");
            var image = pdf.CreateImage(@"C:\Temp\dotnet.gif");

			var page = pdf.CreatePage( 612, 792);

			page.DrawText("a GIF image :",50,550,font,24, new RgbColor());

			page.DrawImage(image, 20, 100);
			page.DrawImage(image, 250, 570, 61, 35);
			
			page.DrawRectangle(20, 40, 400, 50, new RgbColor("#330000"), new RgbColor(Color.Red));

			pdf.MakeOutline(pdf.OutlineRoot, "root", page);

			pdf.Print(new FileStream(@"C:\Temp\test.pdf", FileMode.Create, FileAccess.Write));

			
		}

	}
}