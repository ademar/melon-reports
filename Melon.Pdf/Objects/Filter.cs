// created on 3/13/2002 at 3:43 PM
namespace Melon.Pdf.Objects
{

	///<summary>
	/// Represents a Filter used to decode/encode a stream.
	/// Filters suported by PDF are (as of version 1.4)
	/// /ASCIIHexDecodeFilter
	/// /ASCII85DecodeFilter
	/// /LZWDecode
	/// /FlateDecode
	/// /RunLengthDecode
	/// /CCITTFaxDecode
	/// /JBIG2Decode
	/// /DCTDDecode
	/// </summary>
	public abstract class Filter{
		
			
		public abstract string Name();
		public abstract byte[] encode(byte[] data);
		public abstract string GetDecodeParameters();
		
	}
}
