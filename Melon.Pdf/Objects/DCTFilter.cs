namespace Melon.Pdf.Objects
{
	public class DCTFilter : Filter
	{
		public override string Name
		{
			get { return "/DCTDecode"; }
		}

		public override string DecodeParameters
		{
            get { return null; }
		}

		public override byte[] Encode(byte[] data)
		{
			return data;
		}

		
	}
}