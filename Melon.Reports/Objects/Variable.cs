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
		
		public double counter;
		public double sum;

		public Variable(string name)
		{
			Formula = CALCULATION_NONE;
			Level = RESET_TYPE_NONE;
			Name = name;
		}

		
		public string Name { set; get; }
		public string Type { set; get; }
		public string Expression { get; set; }
		public object Value { get; set; }
		public string ResetingGroup { get; set; }
		public string Level { get; set; }
		public string Formula { get; set; }

		public void UpdateMe(object sender,GroupChangeEventArgs e)
		{
			Value = null ;
		}
	}
}
