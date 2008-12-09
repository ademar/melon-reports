using System.IO;
using Melon.Pdf.Imaging;
using Melon.Pdf.Objects;
using Melon.Reports.Objects;

namespace Melon.Printer
{
	public class PdfDriver : AbstractDriver
	{
		private PdfPage currentPage;

		private string defaultFont;

		public override void Print(Document document, Stream printStream)
		{
			var pdf = new PdfDocument();

			foreach (Font font in document.Fonts)
			{
				
				var pdfFont = pdf.CreateFont(/*font.Name, */PdfFontTypes.TYPE1, font.FontName);
				if (font.IsDefault) defaultFont = pdfFont.FontName;
			}

			foreach (Image image in document.Images)
			{
				var wi = new WinImage(image.url);

				var pdfImage = pdf.CreateImage(wi);

				image.ImageName.Name = pdfImage.Name;
			}

			foreach (Page page in document.Pages)
			{
				PrintPage(page, pdf);
			}

			pdf.Print(printStream);

			printStream.Flush();
		}


		public void PrintPage(Page page, PdfDocument pdf)
		{
			currentPage = pdf.CreatePage(page.Width, page.Height);

			foreach (BasicElement be in page.Elements)
			{
				PrintElement(be, pdf, currentPage.Content);
			}
		}

		public void PrintElement(BasicElement element, PdfDocument pdf, PdfStream pdfStream)
		{
			var type = element.GetType();

			var adapter = new PdfStreamAdapter(pdfStream, defaultFont);

			if (type == typeof (Text))
			{
				adapter.PrintText((Text) element);
			}
			else if (type == typeof (Expression))
			{
				adapter.PrintExpression((Expression) element);
			}
			else if (type == typeof (Image))
			{
				adapter.PrintImage((Image) element);
			}
			else if (type == typeof (Rectangle))
			{
				adapter.PrintRectangle((Rectangle) element);
			}
			else if (type == typeof (Bookmark))
			{
				PrintBookmark((Bookmark) element, pdf);
			}
		}

		public void PrintBookmark(Bookmark b, PdfDocument pdf)
		{
			pdf.MakeOutline(pdf.OutlineRoot, b.VarName, currentPage);
		}
	}
}