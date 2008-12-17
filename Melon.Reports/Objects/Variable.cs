namespace Melon.Reports.Objects
{
	public class Variable
	{
		public static string RESET_TYPE_NONE = "none";
		public static string RESET_TYPE_GROUP = "group";
		public static string RESET_TYPE_REPORT = "report";
		public static string RESET_TYPE_PAGE = "page";

		public static string CALCULATION_NONE = "none";
		public static string CALCULATION_COUNT = "count";
		public static string CALCULATION_SUM = "sum";
		public static string CALCULATION_AVERAGE = "average";
		public static string CALCULATION_LOWEST = "lowest";
		public static string CALCULATION_HIGHEST = "highest";

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

		public void UpdateMe(object sender, GroupChangeEventArgs e)
		{
			Value = null;
		}
	}
}