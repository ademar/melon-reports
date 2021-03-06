// created on 3/13/2002 at 7:06 PM
using System.Globalization;

namespace Melon.Pdf.Objects
{
	public class PdfFont : PdfObject
	{
		protected string Subtype;
		protected string BaseFont;

		public PdfFont(int number, string subtype, string basefont) : base(number)
		{
			Subtype = subtype;
			BaseFont = basefont;
            Encoding = "WinAnsiEncoding";
		}

		public string FontName
		{
			get { return "Font" + Number; }
		}

		public object Encoding { get; set; }

		public override string ToPdf()
		{
			var s = string.Format(CultureInfo.InvariantCulture,
			                      "{0} {1} obj\n<< /Type /Font\n/Subtype /{2}\n/Name /{3}\n/BaseFont /{4}", Number, Generation,
			                      Subtype, FontName, BaseFont.Replace(" ", "_"));

			if (Encoding != null)
			{
				s = s + "\n/Encoding /" + (string) Encoding;
			}

			s = s + " >>\nendobj\n";

			return s;
		}
	}
}