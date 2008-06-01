// created on 3/13/2002 at 4:12 PM
namespace Melon.Pdf.Objects
{
	
	using System ;
	
	///<summary>
	/// Represents a PDF Info dictionary object.
	/// tags suported :
	/// /Title, /Author, /Subject, /Keywords, /Creator, /Producer, /CreationDate
	/// </summary>
	public class PDFInfo : PDFObject{
		
		private string title;
		private string author;
		private string subject;
		private string keywords;
		private string creator;

		private const string producer = "Melon v0.1";
		
		public PDFInfo(int number):base(number){
		}

		public string setTitle{
			set {
				title = value ;
			}
		}
		public string setAuthor{
			set {
				author = value ;
			}
		}
		public string setSubject{
			set {
				subject = value ;
			}
		}
		public string setKeyWords{
			set {
				keywords = value ;
			}
		}
		public string setCreator{
			set {
				creator = value ;
			}
		}
		
		public override string ToPDF(){
			
			String s = string.Format("{0} {1} obj\n<< /Type /Info\n/Producer ({2})\n/CreationDate ({3}) >>\nendobj\n", number, generation, producer, DateTime.Now);
			return s ;
		}
	}
}
