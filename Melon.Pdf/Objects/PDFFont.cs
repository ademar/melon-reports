// created on 3/13/2002 at 7:06 PM
namespace Melon.Pdf.Objects
{

	public class PDFFont : PDFObject{
		
		public static byte TYPE0 = 0 ;
		public static byte TYPE1 = 1 ;
		public static byte MMTYPE  = 2 ;
		public static byte TYPE3 = 3 ;
		public static byte TRUETYPE = 4 ;
		
		protected static string[] TypeNames = {"Type0", "Type1", "MMType1", "Type3", "TrueType"};
		
		protected string FontName ;
		protected byte SubType;
		protected string BaseFont;
		protected object encoding = null ;
		
		public PDFFont(int number,string fontname,byte subtype,
		               string basefont):base(number){
		    FontName = fontname;
		    SubType = subtype;
		    BaseFont = basefont;
			
			encoding = "WinAnsiEncoding";// default
		}
		
		public string getName{
			get{
				return FontName ;
			}
		}
		public object Encoding 
		{
			get 
			{
				return encoding ;
			}
			set 
			{
				encoding = value ;
			}
		}
				
		public override string ToPDF(){

			string s = string.Format("{0} {1} obj\n<< /Type /Font\n/Subtype /{2}\n/Name /{3}\n/BaseFont /{4}", number, generation, TypeNames[SubType], FontName, BaseFont);

			if (encoding!=null) 
			{
				s = s + "\n/Encoding /"+(string)encoding ;
			}
					s = s	+ " >>\nendobj\n";
						
			return s;
		}
	}
}
