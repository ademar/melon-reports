// created on 3/13/2002 at 5:33 PM
using System.Collections;

namespace Melon.Pdf.Objects
{

	public class PDFPages : PDFObject{
		
		protected ArrayList kids ;
		protected int  count  = 0 ;
		
		public PDFPages(int number):base(number){
			kids = new ArrayList();
		}
		
		public void addPage(PDFPage page){
			kids.Add(page.getReference);
			count++;
			page.setParent(this);
			
		}

		public int GetCount {
			get{
				return count ;
			}
		}
		public override string ToPDF(){

			string s = string.Format("{0} {1} obj\n<< /Type /Pages\n/Count {2}\n/Kids [", number, generation, count) ;

			IEnumerator it = kids.GetEnumerator();

			while(it.MoveNext())
			{
				s = s + " " + it.Current  ;
			}
			s = s + " ] >>\nendobj\n";

			return s ;
		}
		
	}
}
