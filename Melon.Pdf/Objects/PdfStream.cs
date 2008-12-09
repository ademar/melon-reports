// created on 3/13/2002 at 6:56 PM
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Melon.Pdf.Objects
{
	public class PdfStream : PdfObject, IDisposable
	{
		protected MemoryStream streamdata = new MemoryStream();

		protected IList<Filter> filters = new List<Filter>();

		public static readonly int DefaultEncoding = 1252;

		public PdfStream(int number) : base(number)
		{
			
		}

		public void AddData(string s)
		{
			var databytes = (new ASCIIEncoding()).GetBytes(s);
			streamdata.Write(databytes, 0, databytes.Length);
		}

		public void AddData(byte[] s)
		{
			streamdata.Write(s, 0, s.Length);
		}

		public void AddFilter(Filter f)
		{
			filters.Add(f);
		}

		public void AddFilter(string f)
		{
			if (f == "flate")
			{
				AddFilter(new FlateFilter());
			}
		}

		protected void AddDefaultFilters()
		{
			AddFilter("flate");
		}

		public int Length
		{
			get { return (int) streamdata.Length; }
		}

		public override string ToPdf()
		{
			/*string s = this.number + " " + this.Generation + " obj\n"
						+ "<< /Length " + streamdata.Length + " >>\nstream\n"
						+ streamdata + "\nendstream\nendobj\n";
			return s;*/
			return null;
		}

		public override int Output(Stream stream)
		{
			var filterEntry = ApplyFilters();

			var s = string.Format(CultureInfo.InvariantCulture,"{0} {1} obj\n<< /Length {2} {3} >>\n", Number, Generation, (streamdata.Length), filterEntry);
			var encoder = new ASCIIEncoding();
			var byteholder = encoder.GetBytes(s);
			stream.Write(byteholder, 0, byteholder.Length);
			var marker = byteholder.Length;

			//the pdf stream 
			marker += outputPDFStream(stream);
			byteholder = encoder.GetBytes("endobj\n");
			stream.Write(byteholder, 0, byteholder.Length);
			marker += byteholder.Length;
			return marker;
		}

		public int outputPDFStream(Stream stream)
		{
			var encoder = new ASCIIEncoding();

			var b = encoder.GetBytes("stream\n");
			stream.Write(b, 0, b.Length);
			stream.Flush();

			var marker = b.Length;
			marker += (int) streamdata.Length;
			streamdata.WriteTo(stream);
			b = encoder.GetBytes("\nendstream\n");
			stream.Write(b, 0, b.Length);
			stream.Flush();
			marker += b.Length;

			return marker;
		}

		public string ApplyFilters()
		{
			var parms = new List<string>();

			if (filters.Count > 0)
			{
				var filternames = "/Filter [ ";

				foreach (var filter in filters)
				{
					var tmp = filter.Encode(streamdata.ToArray());
					streamdata.SetLength(0);
					streamdata.Write(tmp, 0, tmp.Length);
					streamdata.Flush();
					

					if (filter.DecodeParameters != null)
					{
						parms.Add(filter.DecodeParameters);
					}

					filternames = string.Format(CultureInfo.InvariantCulture,"{0}{1} ", filternames, filter.Name);
				}
				
				filternames += "]\n";

				var decodeparms = string.Empty;

				if (parms.Count > 0)
				{
                    decodeparms = "/DecodeParms [ ";

					foreach (var parm in parms)
					{
						decodeparms = decodeparms + parm + " ";
					}

					decodeparms += "]\n";
				}

				return filternames + decodeparms;
			}


			return string.Empty;
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
			AddData("(");
			AddData(Encoding.GetEncoding(DefaultEncoding).GetBytes(text));
			AddData(") Tj\n");
		}

		public void SetTextPos(double x, double y)
		{
			AddData(string.Format(CultureInfo.InvariantCulture, "{0} {1} Td\n", x, y));
		}

		public void SetRGBColorFill(string red, string green, string blue)
		{
			AddData(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} rg\n", red, green, blue));
		}

		public void SetRGBColorStroke(string red, string green, string blue)
		{
			AddData(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} RG\n", red, green, blue));
		}

		public void SetFont(string baseFont, double size)
		{
			AddData(string.Format(CultureInfo.InvariantCulture, "/{0} {1} Tf\n", baseFont, size));
		}

		public void ShowImage(string imageName, double x, double y, double width, double height)
		{
			SaveState();
			AddData(string.Format(CultureInfo.InvariantCulture, "{0} 0 0 {1} {2} {3} cm\n", width, height, x, y));
			AddData(string.Format(CultureInfo.InvariantCulture, "/{0} Do\n", imageName));
			RestoreState();
		}

		public void ShowRectangle(double x, double y, double width, double height)
		{
			AddData(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3} re\n", x, y, width, height));
			FillAndStroke();
		}

		public void FillAndStroke()
		{
			AddData("B\n");
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				streamdata.Close();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}