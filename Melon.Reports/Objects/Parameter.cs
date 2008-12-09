using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Parameter.
	/// </summary>
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

		public Type Type { set; get; }

		public Object Value { get; set; }
	}
}