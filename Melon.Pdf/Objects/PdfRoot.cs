// created on 3/13/2002 at 5:32 PM
using System.Globalization;

namespace Melon.Pdf.Objects
{
	public class PdfRoot : PdfObject{
		
		protected PdfPages pages;
		protected PdfOutline outlines ;
		
		public PdfRoot(int number):base(number){
		}
		public PdfRoot(int number,PdfPages pages):base(number){
			this.pages = pages;
		}
		
		public override string ToPdf(){

			var s = string.Format(CultureInfo.InvariantCulture,"{0} {1} obj\n<< /Type /Catalog\n/Pages {2}\n", Number, Generation, pages.Reference);

			if(outlines!=null)
			{
				s = s + " /Outlines " + outlines.Reference + "\n" ;
				s = s + " /PageMode /UseOutlines >>\n" ;
			}
			s = s + "endobj\n" ;
			return s;
		}
		
		public void setPages(PdfPages pages){
			this.pages = pages ;
		}

		public void addPage(PdfPage page){
			pages.addPage(page);
		}

		public PdfOutline RootOutlines 
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
