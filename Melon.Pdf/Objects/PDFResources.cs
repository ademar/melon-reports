// created on 3/13/2002 at 5:34 PM
using System.Collections;

namespace Melon.Pdf.Objects
{
	public class PDFResources : PDFObject{
		
		protected Hashtable fonts = new Hashtable();
		protected Hashtable images = new Hashtable();
		
		public PDFResources(int number):base(number){
		}

		public void addFont(PDFFont font){
			fonts.Add(font.getName,font);
		}

		public void addImage(PDFImage image){
			images.Add(image.Name,image);
		}
		
		public override string ToPDF(){

			string s = string.Format("{0} {1} obj\n<<\n", number, generation);

			//font resources
			if(fonts.Count>0){
				s = s + "/Font << ";
				IDictionaryEnumerator it = fonts.GetEnumerator();
				while (it.MoveNext()){
					s = s + "/" + it.Key + " " + ((PDFFont)it.Value).getReference + " " ;
				}
				s = s + " >>\n";
			}

			//image resources
			s = s + "/ProcSet [ /PDF /ImageC /Text ]\n";
			if(images.Count>0){
				s = s + "/XObject << ";
				IDictionaryEnumerator it = images.GetEnumerator();
				while(it.MoveNext()){
					s = s + "/" + it.Key + " " + ((PDFImage)it.Value).getReference + " "  ;
				}
				s = s + " >>\n";
			}
			s = s + ">>\nendobj\n";

			return s;
		}
	}
}
