// created on 3/13/2002 at 5:34 PM
using System.Collections.Generic;
using System.Globalization;

namespace Melon.Pdf.Objects
{
	public class PdfResources : PdfObject
	{
		private readonly IList<PdfFont> fonts = new List<PdfFont>();
		private readonly IList<PdfImage> images = new List<PdfImage>();
		
		public PdfResources(int number) : base(number)
		{
		}

		public void addFont(PdfFont font)
		{
			fonts.Add(font);
		}

		public void addImage(PdfImage image)
		{
			images.Add(image);
		}

		public override string ToPdf()
		{
			var s = string.Format(CultureInfo.InvariantCulture, "{0} {1} obj\n<<\n", Number, Generation);

			//font resources
			if (fonts.Count > 0)
			{
				s = s + "/Font << ";
				
				foreach (var font in fonts)
				{
					s = string.Format("{0}/{1} {2} ", s, font.FontName, font.Reference);
				}

				s = s + " >>\n";
			}

			//image resources
			s = s + "/ProcSet [ /PDF /ImageC /Text ]\n";
			if (images.Count > 0)
			{
				s = s + "/XObject << ";
				
				foreach (var image in images)
				{
					s = string.Format("{0}/{1} {2} ", s, image.Name, image.Reference);
				}

				s = s + " >>\n";
			}
			s = s + ">>\nendobj\n";

			return s;
		}
	}
}