// created on 3/13/2002 at 5:34 PM
using System.Collections;
using System.Globalization;

namespace Melon.Pdf.Objects
{
	public class PDFResources : PDFObject{
		
		protected Hashtable fonts = new Hashtable();
		protected Hashtable images = new Hashtable();
		
		public PDFResources(int number):base(number){
		}

		public void addFont(PDFFont font){
			fonts.Add(font.FontName,font);
		}

		public void addImage(PDFImage image){
			images.Add(image.Name,image);
		}
		
		public override string ToPDF(){

			string s = string.Format(CultureInfo.InvariantCulture,"{0} {1} obj\n<<\n", Number, Generation);

			//font resources
			if(fonts.Count>0){
				s = s + "/Font << ";
				var it = fonts.GetEnumerator();
				while (it.MoveNext()){
					s = string.Format("{0}/{1} {2} ", s, it.Key, ((PDFFont)it.Value).Reference) ;
				}
				s = s + " >>\n";
			}

			//image resources
			s = s + "/ProcSet [ /PDF /ImageC /Text ]\n";
			if(images.Count>0){
				s = s + "/XObject << ";
				var it = images.GetEnumerator();
				while(it.MoveNext()){
					s = string.Format("{0}/{1} {2} ", s, it.Key, ((PDFImage)it.Value).Reference)  ;
				}
				s = s + " >>\n";
			}
			s = s + ">>\nendobj\n";

			return s;
		}
	}
}
