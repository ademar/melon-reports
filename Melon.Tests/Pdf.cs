using System.IO;
using Melon.Commons;
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

			pdf.AddFont("F1", PdfFontTypes.TYPE1, "Helvetica");
            pdf.AddImage(AbstractImage.Make(@"C:\Temp\dotnet.gif"));

			var page = pdf.CreatePage( 612, 792);

			page.Text("a GIF image :",50,550,"F1",24, new Color());
            page.Image("Im1", 250, 570, 61, 35);

			pdf.MakeOutline(pdf.OutlineRoot, "root", page);

			pdf.Print(new FileStream(@"C:\Temp\test.pdf", FileMode.Create, FileAccess.Write));

			
		}

	}
}