using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Field.
	/// </summary>
	public class Field 
	{
		private string m_name ;
		private string m_type ;

		private Object m_value ;
		
		public Field()
		{}

		public Field(string name)
		{
			this.m_name = name;
		}
		
		public string Name 
		{
			set
			{
				this.m_name = value ;
			}
			get 
			{
				return this.m_name;
			}
		}

		public string Type 
		{
			set 
			{
				this.m_type = value ;
			}
			get
			{
				return this.m_type;
			}
		}

		public Object Value 
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value ;
			}
		}
	}
}
