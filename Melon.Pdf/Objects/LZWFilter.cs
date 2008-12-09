namespace Melon.Pdf.Objects
{
	public class LZWFilter : Filter
	{
		public bool EarlyChange { get; set; }

		public override string Name() 
		{	
			return "/LZWDecode";
		}
		public override byte[] Encode(byte[] data)
		{
			return data ;
		}
		
		public override string GetDecodeParameters()
		{
			if (EarlyChange)
			{
				return "<< /EarlyChange 1 >>" ;
			}

			return "<< /EarlyChange 0 >> ";
		}
	}
}
