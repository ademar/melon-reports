namespace Melon.Pdf.Objects
{
	public class LZWFilter : Filter
	{
		public bool EarlyChange = false ; 

		public override string Name() 
		{	
			return "/LZWDecode";
		}
		public override byte[] encode(byte[] data)
		{
			return data ;
		}
		
		public override string GetDecodeParameters()
		{
			if (EarlyChange)
			{
				return "<< /EarlyChange 1 >>" ;
			}
			else
			{
				return "<< /EarlyChange 0 >> ";
			}
		}
	}
}
