using System;

namespace Melon.Reports.Objects
{
	public class Parameter
	{
		public Parameter()
		{
		}

		public Parameter(string name)
		{
			Name = name;
		}

		public string Name { set; get; }
        public string Type { set; get; }
        public Object Value { get; set; }
	}
}