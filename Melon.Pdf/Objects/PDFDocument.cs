// created on 3/14/2002 at 12:18 PM
using System.Globalization;

namespace Melon.Pdf.Objects
{
	using System.Collections;
	using System.Text;
	using System.IO;
	using Imaging;
	
	public class PDFDocument{
		
		protected PDFRoot root ;
		protected PDFPages pages;
		protected PDFInfo info;
		protected PDFResources resources;
		protected PDFOutline outlineRoot;
		
		protected int objectcounter;
		protected int imagecounter;
		protected int currentoffset;
		protected int xrefOffset;
		
		protected ArrayList trailer = new ArrayList();
		
		public PDFDocument(){
			pages = MakePages();
			root = MakeRoot(pages);
			resources = MakeResources();
			info = MakeInfo();
		}
		
		protected PDFRoot MakeRoot(PDFPages pages){

			var root = new PDFRoot(++objectcounter,pages);
			trailer.Add(root);
			return root;
		}

		protected PDFPages MakePages(){

			var pages = new PDFPages(++objectcounter);
			trailer.Add(pages);
			return pages;
		}

		protected PDFResources MakeResources(){

			var resources = new PDFResources(++objectcounter);
			trailer.Add(resources);
			return resources;
		}

		protected PDFInfo MakeInfo(){

			var info = new PDFInfo(++objectcounter);
			trailer.Add(info);
			return info;
		}

		public PDFOutline OutlineRoot 
		{
			get
			{
				if (outlineRoot!=null)
				{
					return outlineRoot;
				}

				outlineRoot = new PDFOutline(++objectcounter,null,null);
				root.RootOutlines = outlineRoot;
				trailer.Add(outlineRoot);
				return outlineRoot;
			}
		}

		public PDFOutline MakeOutline(PDFOutline parent,string title,PDFPage page)
		{
			var target = string.Format(CultureInfo.InvariantCulture, "[{0} /XYZ null null 0]", page.Reference);

			var outline = new PDFOutline(++objectcounter,title,target);

			if(parent!=null)
			{
				parent.AddItem(outline);
			}

			trailer.Add(outline);
			return outline ;
		}

		public PDFPage MakePage(PDFStream content, int width,int height){
			
			var page = new PDFPage(++objectcounter,resources,content,width,height);
			trailer.Add(page);
			root.addPage(page);
			
			return page;
		}

		public PDFStream MakeStream(){

			var ps = new PDFStream(++objectcounter);
			trailer.Add(ps);

			return ps;
		}
		
		public string MakeFont(string fontname, byte subtype,string basefont){

			var f = new PDFFont(++objectcounter,fontname,subtype,basefont);

			trailer.Add(f);
			resources.addFont(f);

			return f.FontName ;
		}
		
		public int AddImage(AbstractImage img){

			var pdfImg = new PDFImage(++objectcounter,++imagecounter,img);
			trailer.Add(pdfImg);
			resources.addImage(pdfImg);
			return imagecounter ;
		}
				
		public void outputHeader(Stream stream){

			byte[] pdfHeader = (new ASCIIEncoding()).GetBytes("%PDF-1.4\n");
			stream.Write(pdfHeader,0,pdfHeader.Length);
			stream.Flush();
			currentoffset = pdfHeader.Length ;
			
			//binary remmark as adviced by Adobe
			byte[] rem ={(byte)'%', 0xAA, 0xAB, 0xAC, 0xAD, (byte)'\n'};
			stream.Write(rem,0,rem.Length);
			stream.Flush();
			currentoffset+=rem.Length;
		}
		
		public void outputTrailer(Stream stream, int offset){
			
			currentoffset += outputXref(stream);

			var pdfTrailer = string.Format(CultureInfo.InvariantCulture, "trailer\n<<\n/Size {0}{1}\n/Root {2}\n/Info {3} >>\nstartxref\n{4}\n%%EOF\n", objectcounter, 1, root.Reference, info.Reference, xrefOffset);

			byte[] bytes = (new ASCIIEncoding()).GetBytes(pdfTrailer);
			stream.Write(bytes,0,bytes.Length);
			stream.Flush();
			
		}

		public int outputXref(Stream stream){
			
			var location = new ArrayList();
			
			var it = trailer.GetEnumerator();

			while(it.MoveNext()){

				var o = (PDFObject)it.Current;
				location.Insert(o.Number-1,currentoffset);
				currentoffset+=o.output(stream);
			}

			xrefOffset = currentoffset;

			var pdf = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "xref\n0 {0}\n0000000000 65535 f\x0d\x0a", (objectcounter + 1)));

			var ot = location.GetEnumerator();

			while(ot.MoveNext()){
				var offset = ot.Current.ToString();
				const string padding = "0000000000";
				var loc = padding.Substring(offset.Length)+offset;
				pdf.Append(loc + " 00000 n\x0d\x0a");
			}

			byte[] bytes = (new ASCIIEncoding()).GetBytes(pdf.ToString());
			stream.Write(bytes,0,bytes.Length);
			stream.Flush();
			return bytes.Length;
		}
	}
}
	
