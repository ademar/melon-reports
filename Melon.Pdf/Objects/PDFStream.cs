// created on 3/13/2002 at 6:56 PM
using System.Collections;
using System.IO;
using System.Text;

namespace Melon.Pdf.Objects
{
	public class PDFStream : PDFObject{
		
		protected MemoryStream streamdata = new MemoryStream();
		
		protected ArrayList filters;

		public static readonly int DefaultEncoding = 1252;
		
		public PDFStream(int number):base(number){
			filters = new ArrayList();
		}
		
		public void  AddData(string s){
			byte[] databytes = (new ASCIIEncoding()).GetBytes(s);
			streamdata.Write(databytes,0,databytes.Length);
		}
		
		public void AddData(byte[] s){
			streamdata.Write(s,0,s.Length);
		}
		
		public void AddFilter(Filter f){
			filters.Add(f);
		}
		
		public void AddFilter(string f){
			if (f=="flate"){
				AddFilter(new FlateFilter());
			}
		}
		
		protected void AddDefaultFilters(){
			AddFilter("flate");
		}
		
		public int Length {
			get 
			{
				return (int)streamdata.Length;
			}
		}
		
		public override string ToPDF(){
			
			/*string s = this.number + " " + this.generation + " obj\n"
						+ "<< /Length " + streamdata.Length + " >>\nstream\n"
						+ streamdata + "\nendstream\nendobj\n";
			return s;*/
			return null;
		}
		
		public override int output(Stream stream){
			int marker;
			
			string filterEntry = ApplyFilters();
			
			string s = string.Format("{0} {1} obj\n<< /Length {2} {3} >>\n", number, generation, (streamdata.Length), filterEntry);
			ASCIIEncoding encoder = new ASCIIEncoding();
			byte[] byteholder = encoder.GetBytes(s);
			stream.Write(byteholder,0,byteholder.Length);
			marker = byteholder.Length;
			
			//the pdf stream 
			marker += outputPDFStream(stream);
			byteholder = encoder.GetBytes("endobj\n");
			stream.Write(byteholder,0,byteholder.Length);
			marker += byteholder.Length;
			return marker;
		}
		
		public int outputPDFStream(Stream stream){
			ASCIIEncoding encoder = new ASCIIEncoding();
			int marker;
			byte[] b = encoder.GetBytes("stream\n");
			stream.Write(b,0,b.Length);
			stream.Flush();
			marker = b.Length;
			marker += (int)streamdata.Length;
			streamdata.WriteTo(stream);
			b = encoder.GetBytes("\nendstream\n");
			stream.Write(b,0,b.Length);
			stream.Flush();
			marker += b.Length;
			return marker;
		}
		
		public string ApplyFilters(){
			ArrayList names = new ArrayList();
			ArrayList parms = new ArrayList();
			if(filters.Count>0){
				IEnumerator it = filters.GetEnumerator();
				while(it.MoveNext()){
					Filter f = (Filter)it.Current ;
					byte[] tmp = f.encode(streamdata.ToArray());
					streamdata.SetLength(0);//reset ??
					streamdata.Write(tmp,0,tmp.Length);
					streamdata.Flush();
					names.Add(f.Name());//no decode parms so far
					if (f.GetDecodeParameters()!=null)parms.Add(f.GetDecodeParameters());
				}
				//build the filters entry
				string filternames = "/Filter [ ";
				it = names.GetEnumerator();
				while (it.MoveNext()){
					filternames=string.Format("{0}{1} ", filternames, it.Current);
				}
				filternames+="]\n";

				//build the parameters string
				bool aflag = false ;
				string decodeparms = "/DecodeParms [ ";
				it = parms.GetEnumerator();
				while(it.MoveNext())
				{
					string s  = it.Current.ToString();
					if (s!=null)
					{	
						aflag = true ;
						decodeparms = decodeparms + s + " " ;
					}
				}
				decodeparms += "]\n";

				if (!aflag) decodeparms = "";

				return filternames + decodeparms;
			}
			return "";
		}
		
		public void BeginText()
		{
			AddData("BT\n");
		}
		
		public void EndText()
		{
			AddData("ET\n");
		}
		
		public void SaveState()
		{
			AddData("q\n");
		}
		
		public void RestoreState()
		{
			AddData("Q\n");
		}
		
		public void ShowText(string text)
		{
			AddData("(" );
			AddData( Encoding.GetEncoding(DefaultEncoding).GetBytes(text));
			AddData(") Tj\n");
		}
		
		public void SetTextPos(double x, double y)
		{
			AddData(x + " " + y + " Td\n");
		}
		
        public void SetRGBColorFill(string red, string green, string blue)
		{
			AddData(red + " " + green + " " + blue + " rg\n");
		}
		
		public void SetRGBColorStroke(string red, string green, string blue)
		{
			AddData(red + " " + green + " " + blue + " RG\n");
		}
		
		public void SetFont(string baseFont,double size)
		{
			AddData("/" + baseFont + " " + size + " Tf\n");
		}
		
		public void ShowImage(string imageName, double x , double y, double width,double height)
		{
			SaveState();
			AddData(width + " 0 0 " + height + " " + x + " " + y + " cm\n");
			AddData("/" + imageName + " Do\n");
			RestoreState();
		}
		
		public void ShowRectangle(double x, double y, double width, double height)
		{
			AddData(x + " " + y + " " + width + " " + height + " re\n");
			FillAndStroke();
		}
		
		public void FillAndStroke()
		{
			AddData("B\n");
		}
	}
}
