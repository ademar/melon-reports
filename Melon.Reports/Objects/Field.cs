using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Field.
	/// </summary>
	public class Field
	{
		public Field()
		{
		}

		public Field(string name)
		{
			Name = name;
		}

		public string Name { set; get; }

		public string Type { set; get; }

		public Object Value { get; set; }
	}
}