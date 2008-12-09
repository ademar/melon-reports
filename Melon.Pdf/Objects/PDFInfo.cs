// created on 3/13/2002 at 4:12 PM
using System.Globalization;

namespace Melon.Pdf.Objects
{
	
	using System ;
	
	///<summary>
	/// Represents a PDF Info dictionary object.
	/// tags suported :
	/// /Title, /Author, /Subject, /Keywords, /Creator, /Producer, /CreationDate
	/// </summary>
	public class PDFInfo : PDFObject{
		
		public string Title;
		public string Author;
		public string Subject;
		public string Keywords;
		public string Creator;

		private const string producer = "Melon v0.1";
		
		public PDFInfo(int number):base(number)
		{
		}

		public override string ToPDF()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} {1} obj\n<< /Type /Info\n/Producer ({2})\n/CreationDate ({3}) >>\nendobj\n", Number, Generation, producer, DateTime.Now);
		}
	}
}
