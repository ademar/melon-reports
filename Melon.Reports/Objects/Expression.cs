namespace Melon.Reports.Objects
{
	public class Expression : BasicElement
	{
		public Expression(string content)
		{
			Content = content;
		}

		public int X { get; set; }
        public int Y { get; set; }
        public int FontSize { get; set; }
        public string FontName { get; set; }
        public string Content { get; set; }
        public object Value { get; set; }
        public string Type { set; get; }
	}
}