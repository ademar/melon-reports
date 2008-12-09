// created on 3/13/2002 at 7:06 PM
using System.Globalization;

namespace Melon.Pdf.Objects
{

	public class PDFFont : PDFObject{
		
		public static byte TYPE0 = 0 ;
		public static byte TYPE1 = 1 ;
		public static byte MMTYPE  = 2 ;
		public static byte TYPE3 = 3 ;
		public static byte TRUETYPE = 4 ;
		
		protected static string[] TypeNames = {"Type0", "Type1", "MMType1", "Type3", "TrueType"};
		
		
		protected byte Subtype;
		protected string BaseFont;

		public PDFFont(int number,string fontname,byte subtype,
		               string basefont):base(number){
		    FontName = fontname;
		    Subtype = subtype;
		    BaseFont = basefont;
			
			Encoding = "WinAnsiEncoding";// default
		}

		public string FontName { get; set; }
		public object Encoding { get; set; }

		public override string ToPDF(){

			var s = string.Format(CultureInfo.InvariantCulture,"{0} {1} obj\n<< /Type /Font\n/Subtype /{2}\n/Name /{3}\n/BaseFont /{4}", Number, Generation, TypeNames[Subtype], FontName, BaseFont);

			if (Encoding!=null) 
			{
				s = s + "\n/Encoding /"+(string)Encoding ;
			}
			
			s = s	+ " >>\nendobj\n";
						
			return s;
		}
	}
}
