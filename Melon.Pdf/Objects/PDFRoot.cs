// created on 3/13/2002 at 5:32 PM
using System.Globalization;

namespace Melon.Pdf.Objects
{
	public class PDFRoot : PDFObject{
		
		protected PDFPages pages;
		protected PDFOutline outlines ;
		
		public PDFRoot(int number):base(number){
		}
		public PDFRoot(int number,PDFPages pages):base(number){
			this.pages = pages;
		}
		
		public override string ToPDF(){

			var s = string.Format(CultureInfo.InvariantCulture,"{0} {1} obj\n<< /Type /Catalog\n/Pages {2}\n", Number, Generation, pages.Reference);

			if(outlines!=null)
			{
				s = s + " /Outlines " + outlines.Reference + "\n" ;
				s = s + " /PageMode /UseOutlines >>\n" ;
			}
			s = s + "endobj\n" ;
			return s;
		}
		
		public void setPages(PDFPages pages){
			this.pages = pages ;
		}

		public void addPage(PDFPage page){
			pages.addPage(page);
		}

		public PDFOutline RootOutlines 
		{
			set 
			{
				outlines = value ;
			}
			get
			{
				return outlines;
			}
		}
	}
}
