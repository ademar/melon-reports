// created on 3/14/2002 at 12:18 PM
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Melon.Pdf.Objects
{
	using System.Text;
	using System.IO;
	using Imaging;

	public class PdfDocument
	{
		protected PdfRoot root;
		protected PdfPages pages;
		protected PdfInfo info;
		protected PdfResources resources;
		protected PdfOutline outlineRoot;

		protected int objectcounter;
		protected int imagecounter;
		protected int currentoffset;
		protected int xrefOffset;

		protected IList<PdfObject> trailer = new List<PdfObject>();

		public PdfDocument()
		{
			pages = MakePages();
			root = MakeRoot(pages);
			resources = MakeResources();
			info = MakeInfo();
		}

		private PdfRoot MakeRoot(PdfPages pages)
		{
			var root = new PdfRoot(++objectcounter, pages);
			trailer.Add(root);
			return root;
		}

		private PdfPages MakePages()
		{
			var pages = new PdfPages(++objectcounter);
			trailer.Add(pages);
			return pages;
		}

		private PdfResources MakeResources()
		{
			var resources = new PdfResources(++objectcounter);
			trailer.Add(resources);
			return resources;
		}

		private PdfInfo MakeInfo()
		{
			var info = new PdfInfo(++objectcounter);
			trailer.Add(info);
			return info;
		}

		public PdfOutline OutlineRoot
		{
			get
			{
				if (outlineRoot != null)
				{
					return outlineRoot;
				}

				outlineRoot = new PdfOutline(++objectcounter, null, null);
				root.RootOutlines = outlineRoot;
				trailer.Add(outlineRoot);
				return outlineRoot;
			}
		}

		public PdfOutline MakeOutline(PdfOutline parent, string title, PdfPage page)
		{
			var target = string.Format(CultureInfo.InvariantCulture, "[{0} /XYZ null null 0]", page.Reference);

			var outline = new PdfOutline(++objectcounter, title, target);

			if (parent != null)
			{
				parent.AddItem(outline);
			}

			trailer.Add(outline);
			return outline;
		}

		public PdfPage CreatePage(int width, int height)
		{
			var content = MakeStream();

			var page = new PdfPage(++objectcounter, resources.Reference, content, width, height);
			trailer.Add(page);
			root.addPage(page);

			return page;
		}

		private PdfStream MakeStream()
		{
			var ps = new PdfStream(++objectcounter);
			trailer.Add(ps);

			return ps;
		}

		public string AddFont(string fontname, string subtype, string basefont)
		{
			var f = new PdfFont(++objectcounter, fontname, subtype, basefont);

			trailer.Add(f);
			resources.addFont(f);

			return f.FontName;
		}

		public int AddImage(AbstractImage img)
		{
			var pdfImg = new PdfImage(++objectcounter, ++imagecounter, img);

			trailer.Add(pdfImg);
			resources.addImage(pdfImg);

			return imagecounter;
		}

		public void Print(Stream stream)
		{
			outputHeader(stream);
			var offset = outputXref(stream);
			outputTrailer(stream, offset);
		}

		private void outputHeader(Stream stream)
		{
			var pdfHeader = (new ASCIIEncoding()).GetBytes("%PDF-1.4\n");
			stream.Write(pdfHeader, 0, pdfHeader.Length);
			stream.Flush();
			currentoffset = pdfHeader.Length;

			//binary remmark as adviced by Adobe
			byte[] rem = {(byte) '%', 0xAA, 0xAB, 0xAC, 0xAD, (byte) '\n'};
			stream.Write(rem, 0, rem.Length);
			stream.Flush();
			currentoffset += rem.Length;
		}

		private void outputTrailer(Stream stream, int offset)
		{
			currentoffset += outputXref(stream);

			var pdfTrailer = string.Format(CultureInfo.InvariantCulture,
			                               "trailer\n<<\n/Size {0}{1}\n/Root {2}\n/Info {3} >>\nstartxref\n{4}\n%%EOF\n",
			                               objectcounter, 1, root.Reference, info.Reference, xrefOffset);

			var bytes = (new ASCIIEncoding()).GetBytes(pdfTrailer);
			stream.Write(bytes, 0, bytes.Length);
			stream.Flush();
		}

		private int outputXref(Stream stream)
		{
			var location = new int[trailer.Count];

			foreach (var o in trailer)
			{
				location[o.Number - 1] = currentoffset;
				currentoffset += o.Output(stream);
			}

			xrefOffset = currentoffset;

			var pdfBuilder = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "xref\n0 {0}\n0000000000 65535 f\x0d\x0a",
				                                (objectcounter + 1)));

			//var ot = location.GetEnumerator();

			foreach (var offset in location)
			{
				//const string padding = "0000000000";
				//var loc = padding.Substring(offset.Length) + offset;
				var loc = string.Format(CultureInfo.InvariantCulture,"{0:0000000000}", offset);
				pdfBuilder.Append(loc + " 00000 n\x0d\x0a");
			}

			/*while (ot.MoveNext())
			{
				var offset = ot.Current.ToString();
				const string padding = "0000000000";
				var loc = padding.Substring(offset.Length) + offset;
				pdfBuilder.Append(loc + " 00000 n\x0d\x0a");
			}*/

			var bytes = (new ASCIIEncoding()).GetBytes(pdfBuilder.ToString());
			stream.Write(bytes, 0, bytes.Length);
			stream.Flush();
			return bytes.Length;
		}

		
	}
}