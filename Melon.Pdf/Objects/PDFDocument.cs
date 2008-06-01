// created on 3/14/2002 at 12:18 PM
namespace Melon.Pdf.Objects
{
	
	using System;
	using System.Collections;
	using System.Text;
	using System.IO;
	using Imaging;
	
	public class PDFDocument{
		
		protected PDFRoot root ;
		protected PDFPages pages;
		protected PDFInfo info;
		protected PDFResources resources;
		protected PDFOutline outlineRoot = null ;
		
		protected int objectcounter = 0 ;
		protected int imagecounter = 0 ;
		protected int currentoffset = 0 ;
		protected int xrefOffset;
		
		protected ArrayList trailer = new ArrayList();
		
		public PDFDocument(){
			pages = MakePages();
			root = MakeRoot(pages);
			resources = MakeResources();
			info = MakeInfo();
		}
		
		protected PDFRoot MakeRoot(PDFPages pages){

			PDFRoot root = new PDFRoot(++objectcounter,pages);
			trailer.Add(root);
			return root;
		}

		protected PDFPages MakePages(){

			PDFPages pages = new PDFPages(++objectcounter);
			trailer.Add(pages);
			return pages;
		}

		protected PDFResources MakeResources(){

			PDFResources resources = new PDFResources(++objectcounter);
			trailer.Add(resources);
			return resources;
		}

		protected PDFInfo MakeInfo(){

			PDFInfo info = new PDFInfo(++objectcounter);
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
			string target = "["+ page.getReference + " /XYZ null null 0]";

			PDFOutline outline = new PDFOutline(++objectcounter,title,target);

			if(parent!=null)
			{
				parent.AddItem(outline);
			}

			trailer.Add(outline);
			return outline ;
		}

		public PDFPage MakePage(PDFStream content, int width,int height){
			
			PDFPage page = new PDFPage(++objectcounter,resources,content,width,height);
			trailer.Add(page);
			root.addPage(page);
			
			return page;
		}

		public PDFStream MakeStream(){

			PDFStream ps = new PDFStream(++objectcounter);
			trailer.Add(ps);

			return ps;
		}
		
		public string MakeFont(string fontname, byte subtype,string basefont){

			PDFFont f = new PDFFont(++objectcounter,fontname,subtype,basefont);

			trailer.Add(f);
			resources.addFont(f);

			return f.getName ;
		}
		
		public int AddImage(AbstractImage img){

			PDFImage pdfImg = new PDFImage(++objectcounter,++imagecounter,img);
			trailer.Add(pdfImg);
			resources.addImage(pdfImg);
			return imagecounter ;
		}
				
		public void outputHeader(Stream stream){

			byte[] pdf = (new ASCIIEncoding()).GetBytes("%PDF-1.4\n");
			stream.Write(pdf,0,pdf.Length);
			stream.Flush();
			currentoffset = pdf.Length ;
			
			//binary remmark as adviced by Adobe
			byte[] rem ={(byte)'%', 0xAA, 0xAB, 0xAC, 0xAD, (byte)'\n'};
			stream.Write(rem,0,rem.Length);
			stream.Flush();
			currentoffset+=rem.Length;
		}
		
		public void outputTrailer(Stream stream){
			
			currentoffset += outputXref(stream);

			String pdf = string.Format("trailer\n<<\n/Size {0}{1}\n/Root {2}\n/Info {3} >>\nstartxref\n{4}\n%%EOF\n", objectcounter, 1, root.getReference, info.getReference, xrefOffset);

			byte[] bytes = (new ASCIIEncoding()).GetBytes(pdf);
			stream.Write(bytes,0,bytes.Length);
			stream.Flush();
			
		}
		
		private int outputXref(Stream stream){
			
			ArrayList location = new ArrayList();
			
			IEnumerator it = trailer.GetEnumerator();

			while(it.MoveNext()){

				PDFObject o = (PDFObject)it.Current;
				location.Insert(o.getNumber-1,currentoffset);
				currentoffset+=o.output(stream);
			}

			xrefOffset = currentoffset;

			StringBuilder pdf = new StringBuilder(string.Format("xref\n0 {0}\n0000000000 65535 f\x0d\x0a", (objectcounter + 1)));

			IEnumerator ot = location.GetEnumerator();

			while(ot.MoveNext()){
				string offset = ot.Current.ToString();
				string padding = "0000000000";
				string loc = padding.Substring(offset.Length)+offset;
				pdf.Append(loc + " 00000 n\x0d\x0a");
			}

			byte[] bytes = (new ASCIIEncoding()).GetBytes(pdf.ToString());
			stream.Write(bytes,0,bytes.Length);
			stream.Flush();
			return bytes.Length;
		}
	}
}
	
