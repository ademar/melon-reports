using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Parameter.
	/// </summary>
	public class Parameter
	{

		private string m_name ;
		private Type m_type ;

		private Object m_value ;

		public Parameter()
		{}
		public Parameter(string name)
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

		public Type Type 
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
