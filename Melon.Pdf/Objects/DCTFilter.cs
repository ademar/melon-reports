namespace Melon.Pdf.Objects
{
	public class DCTFilter : Filter
	{
		public override string Name() 
		{	
			return "/DCTDecode";
		}
		public override byte[] encode(byte[] data)
		{
			return data ;
		}
		public override string GetDecodeParameters()
		{
			return null;
		}
	}
}
