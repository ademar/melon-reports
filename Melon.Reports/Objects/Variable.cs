using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Variable.
	/// </summary>
	public class Variable
	{
		// levels of resetting
		static public string RESET_TYPE_NONE   = "none";
		static public string RESET_TYPE_GROUP  = "group";
		static public string RESET_TYPE_REPORT = "report";
		static public string RESET_TYPE_PAGE   = "page";

		// calculations
		static public string CALCULATION_NONE    = "none";
		static public string CALCULATION_COUNT   = "count";
		static public string CALCULATION_SUM     = "sum";
		static public string CALCULATION_AVERAGE = "average";
		static public string CALCULATION_LOWEST  = "lowest";
		static public string CALCULATION_HIGHEST = "highest";
		
		/* */
		private string name ;
		private string type ;	
		private string expression = null;
		private object m_value = null;
		private string level = RESET_TYPE_NONE ;
		private string calculation = CALCULATION_NONE ;
		private string group = null ;

		/* */

		public double counter = 0.0 ;
		public double sum = 0.0;

		/* */
		public Variable(string name)
		{
			this.name = name;
		}
		/* */
		public string Name 
		{
			set
			{
				this.name = value ;
			}
			get 
			{
				return this.name;
			}
		}
		/* */
		public string Type 
		{
			set 
			{
				this.type = value ;
			}
			get
			{
				return this.type;
			}
		}
		/* */
		public string Expression
		{
			get
			{
				return this.expression;
			}
			set
			{
				this.expression = value ;
			}
		}
		/* */
		public object Value 
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
		/* */
		public string ResetingGroup 
		{
			get 
			{
				return this.group ;
			}
			set 
			{
				this.group = value ;
			}
		}
		/* */
		public string Level
		{
			get
			{
				return this.level;
			}
			set 
			{
				this.level = value ;
			}
		}
		/* */
		public string Formula
		{
			get 
			{
				return this.calculation ;
			}
			set 
			{
				this.calculation = value ;
			}
		}

		/* *
		 * this guy is called whenever his group changes
		 * */
		public void UpdateMe(object sender,GroupChangeEventArgs e)
		{
			//reset this variable
			m_value = null ;
		}
	}
}
