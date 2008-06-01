// created on 3/13/2002 at 3:29 PM
using System.IO;
using System.Text;

namespace Melon.Pdf.Objects
{
	
	public abstract class PDFObject{
		
		protected int number ;
		protected int generation = 0 ; 
	
		public PDFObject(int number){
			this.number = number;
		}
		
		public string getReference {
			get {
				return string.Format("{0} {1} R", number, generation);
			}
		}
		public int getNumber {
			get {
				return number;
			}
		}
		public abstract string ToPDF();
		
		public virtual int output(Stream stream){

			byte[] buffer = (new ASCIIEncoding()).GetBytes(ToPDF());
			stream.Write(buffer,0,buffer.Length);
			stream.Flush();
			return buffer.Length;
		}
		
		
	}
	
}
