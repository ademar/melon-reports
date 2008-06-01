// created on 3/13/2002 at 5:33 PM
namespace Melon.Pdf.Objects
{

	public class PDFPage : PDFObject{
		
		protected string parent ;
		protected PDFResources resources ;
		protected PDFStream content ;
		
		protected int width ;
		protected int height ;
		
		public PDFPage(int number,PDFResources resources,PDFStream content,int width,int height):base(number){
			this.resources = resources;
			this.content = content;
			this.width = width ;
			this.height = height ;
		}
		
		public void setParent(PDFPages parent){
			this.parent = parent.getReference ;
		}
		
		public override string ToPDF(){
			string s = string.Format("{0} {1} obj\n<< /Type /Page\n/Parent {2}\n/MediaBox [ 0 0 {3} {4} ]\n/Resources {5}\n/Contents {6}\n>>\nendobj\n", number, generation, parent, width, height, resources.getReference, content.getReference) ;
			return s ;
		}
	}
		
		
	}
