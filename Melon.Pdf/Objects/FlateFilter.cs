// created on 3/21/2002 at 6:25 PM
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Melon.Pdf.Objects
{
	public class FlateFilter : Filter
	{
		public override string Name
		{
			get
			{
				return "/FlateDecode";
			}
		}

		public override string DecodeParameters
		{
			get
			{
				return null;
			}
		}

		public override byte[] Encode(byte[] data)
		{
			var o = new MemoryStream();

			var compressed = new DeflaterOutputStream(o, new Deflater(1));
			compressed.Write(data, 0, data.Length);
			compressed.Flush();
			compressed.Close();

			return o.ToArray();
		}

		
	}
}