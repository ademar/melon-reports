// created on 3/13/2002 at 5:33 PM
using System.Globalization;
using Melon.Commons;

namespace Melon.Pdf.Objects
{
	public class PdfPage : PdfObject
	{
		private readonly string resourcesRef;

		protected int width;
		protected int height;

		public PdfPage(int number, string resourcesRef, PdfStream content, int width, int height)
			: base(number)
		{
			this.resourcesRef = resourcesRef;
			this.width = width;
			this.height = height;

			Content = content;
		}

		public string Parent { private get; set; }

		public PdfStream Content { get; private set; }

		public override string ToPdf()
		{
			return string.Format(CultureInfo.InvariantCulture,
			                     "{0} {1} obj\n<< /Type /Page\n/Parent {2}\n/MediaBox [ 0 0 {3} {4} ]\n/Resources {5}\n/Contents {6}\n>>\nendobj\n",
								 Number, Generation, Parent, width, height, resourcesRef, Content.Reference);
		}


		public void DrawText(string text, double x, double y, string font, double fontSize, RgbColor rgbColor)
		{
			Content.SaveState();
			Content.SetRGBColorFill(rgbColor.Red, rgbColor.Green, rgbColor.Blue);
			Content.BeginText();
			Content.SetFont(font, fontSize);
			Content.SetTextPos(x, y);
			Content.ShowText(text);
			Content.EndText();
			Content.RestoreState();
		}

		public void DrawImage(string imageName, double x, double y, double _width, double _height)
		{
			Content.ShowImage(imageName, x, y, _width, _height);
		}

		public void DrawRectangle(double x, double y, double _width, double _height, RgbColor bordercolor, RgbColor fillcolor)
		{
			Content.SaveState();
			Content.SetRGBColorFill(fillcolor.Red, fillcolor.Green, fillcolor.Blue);
			Content.SetRGBColorStroke(bordercolor.Red, bordercolor.Green, bordercolor.Blue);
			Content.ShowRectangle(x, y , _width, _height);
			Content.RestoreState();
		}

		public void DrawText(string text, double x, double y, PdfFont font, double fontzsize, RgbColor color)
		{
			DrawText(text, x, y, font.FontName, fontzsize, color);
		}

		public void DrawImage(PdfImage pdfImage, double x, double y, double _width, double _height)
		{
			DrawImage(pdfImage.Name,x,y,_width,_height);
		}

		public void DrawImage(PdfImage pdfImage, double x, double y)
		{
			DrawImage(pdfImage.Name, x, y, pdfImage.Width, pdfImage.Height);
		}
	}
}