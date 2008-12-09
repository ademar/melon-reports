using System.IO;
using Melon.Pdf.Imaging;
using Melon.Pdf.Objects;
using Melon.Reports.Objects;

namespace Melon.Printer
{
	public class PDFDriver : AbstractDriver
	{
		private PDFPage currentPage;
		
		private string defaultFont;

		
		public override void Print(Document document, Stream printStream)
		{
			var pdf = new PDFDocument();

			foreach (Font font in document.Fonts)
			{
				pdf.MakeFont(font.Name, PDFFont.TYPE1, font.FontName);
				if (font.IsDefault) defaultFont = font.Name;
			}

			foreach (Image image in document.Images)
			{
				var wi = new WinImage(image.url);
				image.IName.Name = string.Format("Im{0}", pdf.AddImage(wi));
			}

			foreach (Page page in document.Pages)
			{
				PrintPage(page,pdf);
			}

			pdf.outputHeader(printStream);
			var offset = pdf.outputXref(printStream);
			pdf.outputTrailer(printStream, offset);

			printStream.Flush();

		}

		
		public void PrintPage(Page page, PDFDocument pdf)
		{
			var currentPDFStream = pdf.MakeStream();
            
			currentPage = pdf.MakePage(currentPDFStream,page.Width,page.Height);

			foreach (BasicElement be in page.Elements)
			{
				PrintElement(be, pdf, currentPDFStream);
			}
			
		}
		
		public void PrintElement(BasicElement element, PDFDocument pdf, PDFStream pdfStream)
		{
			var type = element.GetType();

			var adapter = new PDFStreamAdapter(pdfStream, defaultFont);

			if(type == typeof(Text)) 
			{
				adapter.PrintText((Text)element);
			}
			else if(type == typeof(Expression)) 
			{
				adapter.PrintExpression((Expression)element);
			}
			else if(type == typeof(Image)) 
			{
				adapter.PrintImage((Image)element);
			}
			else if(type == typeof(Rectangle)) 
			{
				adapter.PrintRectangle((Rectangle)element);
			}
			else if(type == typeof(Bookmark)) 
			{
				PrintBookmark((Bookmark)element,pdf);
			}
		}

		public void PrintBookmark(Bookmark b, PDFDocument pdf)
		{
			pdf.MakeOutline(pdf.OutlineRoot,b.VarName,currentPage);
		}
		
	}

	public class PDFStreamAdapter
	{
		private readonly PDFStream pdfStream;
		private readonly string defaultFont;

		public PDFStreamAdapter(PDFStream pdfStream, string defaultFont)
		{
			this.pdfStream = pdfStream;
			this.defaultFont = defaultFont;
		}

		public void PrintImage(Image image)
		{
			pdfStream.SaveState();
			pdfStream.ShowImage(image.IName.Name, image.x, (image.H - image.y - image.height), image.width, image.height);
			pdfStream.RestoreState();
		}

		public void PrintText(Text text)
		{
			pdfStream.SaveState();
			pdfStream.SetRGBColorFill(text.color.Red, text.color.Green, text.color.Blue);
			pdfStream.BeginText();
			pdfStream.SetFont(defaultFont, text.FontSize);
			pdfStream.SetTextPos(text.X, (text.H - text.Y));
			pdfStream.ShowText(text.Label);
			pdfStream.EndText();
			pdfStream.RestoreState();
		}

		public void PrintExpression(Expression e)
		{
			pdfStream.BeginText();
			pdfStream.SetFont(defaultFont, e.FontSize);
			pdfStream.SetTextPos(e.X, (e.H - e.Y));
			pdfStream.ShowText(e.Value.ToString());
			pdfStream.EndText();
		}

		public void PrintRectangle(Rectangle r)
		{
			pdfStream.SaveState();
			pdfStream.SetRGBColorFill(r.fillcolor.Red, r.fillcolor.Green, r.fillcolor.Blue);
			pdfStream.SetRGBColorStroke(r.bordercolor.Red, r.bordercolor.Green, r.bordercolor.Blue);
			pdfStream.ShowRectangle(r.x, r.H - r.y - r.height, r.width, r.height);
			pdfStream.RestoreState();
		}
	}
}
