// created on 3/14/2002 at 12:18 PM
using System.Globalization;

namespace Melon.Pdf.Objects
{
	using System.Collections;
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

		protected ArrayList trailer = new ArrayList();

		public PdfDocument()
		{
			pages = MakePages();
			root = MakeRoot(pages);
			resources = MakeResources();
			info = MakeInfo();
		}

		protected PdfRoot MakeRoot(PdfPages pages)
		{
			var root = new PdfRoot(++objectcounter, pages);
			trailer.Add(root);
			return root;
		}

		protected PdfPages MakePages()
		{
			var pages = new PdfPages(++objectcounter);
			trailer.Add(pages);
			return pages;
		}

		protected PdfResources MakeResources()
		{
			var resources = new PdfResources(++objectcounter);
			trailer.Add(resources);
			return resources;
		}

		protected PdfInfo MakeInfo()
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

		public PdfPage MakePage(PdfStream content, int width, int height)
		{
			var page = new PdfPage(++objectcounter, resources, content, width, height);
			trailer.Add(page);
			root.addPage(page);

			return page;
		}

		public PdfStream MakeStream()
		{
			var ps = new PdfStream(++objectcounter);
			trailer.Add(ps);

			return ps;
		}

		public string MakeFont(string fontname, string subtype, string basefont)
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

		public void outputHeader(Stream stream)
		{
			byte[] pdfHeader = (new ASCIIEncoding()).GetBytes("%PDF-1.4\n");
			stream.Write(pdfHeader, 0, pdfHeader.Length);
			stream.Flush();
			currentoffset = pdfHeader.Length;

			//binary remmark as adviced by Adobe
			byte[] rem = {(byte) '%', 0xAA, 0xAB, 0xAC, 0xAD, (byte) '\n'};
			stream.Write(rem, 0, rem.Length);
			stream.Flush();
			currentoffset += rem.Length;
		}

		public void outputTrailer(Stream stream, int offset)
		{
			currentoffset += outputXref(stream);

			var pdfTrailer = string.Format(CultureInfo.InvariantCulture,
			                               "trailer\n<<\n/Size {0}{1}\n/Root {2}\n/Info {3} >>\nstartxref\n{4}\n%%EOF\n",
			                               objectcounter, 1, root.Reference, info.Reference, xrefOffset);

			byte[] bytes = (new ASCIIEncoding()).GetBytes(pdfTrailer);
			stream.Write(bytes, 0, bytes.Length);
			stream.Flush();
		}

		public int outputXref(Stream stream)
		{
			var location = new ArrayList();

			var it = trailer.GetEnumerator();

			while (it.MoveNext())
			{
				var o = (PdfObject) it.Current;
				location.Insert(o.Number - 1, currentoffset);
				currentoffset += o.Output(stream);
			}

			xrefOffset = currentoffset;

			var pdf =
				new StringBuilder(string.Format(CultureInfo.InvariantCulture, "xref\n0 {0}\n0000000000 65535 f\x0d\x0a",
				                                (objectcounter + 1)));

			var ot = location.GetEnumerator();

			while (ot.MoveNext())
			{
				var offset = ot.Current.ToString();
				const string padding = "0000000000";
				var loc = padding.Substring(offset.Length) + offset;
				pdf.Append(loc + " 00000 n\x0d\x0a");
			}

			byte[] bytes = (new ASCIIEncoding()).GetBytes(pdf.ToString());
			stream.Write(bytes, 0, bytes.Length);
			stream.Flush();
			return bytes.Length;
		}
	}
}