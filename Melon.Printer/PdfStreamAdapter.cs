using Melon.Pdf.Objects;
using Melon.Reports.Objects;

namespace Melon.Printer
{
	public class PdfStreamAdapter
	{
		private readonly PdfStream pdfStream;
		private readonly string defaultFont;

		public PdfStreamAdapter(PdfStream pdfStream, string defaultFont)
		{
			this.pdfStream = pdfStream;
			this.defaultFont = defaultFont;
		}

		public void PrintImage(Image image)
		{
			pdfStream.SaveState();
			pdfStream.ShowImage(image.ImageName.Name, image.x, (image.H - image.y - image.height), image.width, image.height);
			pdfStream.RestoreState();
		}

		public void PrintText(Text text)
		{
			pdfStream.SaveState();
			pdfStream.SetRGBColorFill(text.rgbColor.Red, text.rgbColor.Green, text.rgbColor.Blue);
			pdfStream.BeginText();
            var fontName = text.FontName == null ? defaultFont : text.FontName;
			pdfStream.SetFont(fontName, text.FontSize);
			pdfStream.SetTextPos(text.X, (text.H - text.Y));
			pdfStream.ShowText(text.Label);
			pdfStream.EndText();
			pdfStream.RestoreState();
		}

		public void PrintExpression(Expression e)
		{
			pdfStream.BeginText();
            var fontName = e.FontName == null ? defaultFont : e.FontName;
            pdfStream.SetFont(fontName, e.FontSize);
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