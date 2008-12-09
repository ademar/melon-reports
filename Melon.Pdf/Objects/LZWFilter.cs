namespace Melon.Pdf.Objects
{
	public class LZWFilter : Filter
	{
		public bool EarlyChange { get; set; }

		public override string Name
		{
			get
			{
				return "/LZWDecode";
			}
		}

		public override string DecodeParameters
		{
			get
			{
				return EarlyChange ? "<< /EarlyChange 1 >>" : "<< /EarlyChange 0 >> ";
			}
		}

		public override byte[] Encode(byte[] data)
		{
			return data;
		}

		
	}
}