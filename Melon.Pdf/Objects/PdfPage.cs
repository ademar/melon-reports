// created on 3/13/2002 at 5:33 PM
using System.Globalization;

namespace Melon.Pdf.Objects
{

	public class PdfPage : PdfObject{
		
		protected string parent ;
		protected PdfResources resources ;
		protected PdfStream content ;
		
		protected int width ;
		protected int height ;
		
		public PdfPage(int number,PdfResources resources,PdfStream content,int width,int height):base(number){
			this.resources = resources;
			this.content = content;
			this.width = width ;
			this.height = height ;
		}
		
		public void setParent(PdfPages parent){
			this.parent = parent.Reference ;
		}
		
		public override string ToPdf(){
			return string.Format(CultureInfo.InvariantCulture, "{0} {1} obj\n<< /Type /Page\n/Parent {2}\n/MediaBox [ 0 0 {3} {4} ]\n/Resources {5}\n/Contents {6}\n>>\nendobj\n", Number, Generation, parent, width, height, resources.Reference, content.Reference);
		}
	}
		
		
	}
