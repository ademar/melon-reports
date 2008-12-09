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


		public void Text(string text, double x, double y, string font, double fontSize, Color color)
		{
			Content.SaveState();
			Content.SetRGBColorFill(color.Red, color.Green, color.Blue);
			Content.BeginText();
			Content.SetFont(font, fontSize);
			Content.SetTextPos(x, y);
			Content.ShowText(text);
			Content.EndText();
			Content.RestoreState();
		}

		public void Image(string imageName, double x, double y, double width, double height)
		{
			Content.ShowImage(imageName,x,y,width,height);
		}
	}
}