using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for GroupChangeEventHandler.
	/// </summary>
	public delegate void GroupChangeEventHandler(object sender, GroupChangeEventArgs e);

	public class GroupChangeEventArgs : EventArgs
	{
		public GroupChangeEventArgs(Report report)
		{
			Report = report;
		}

		public Report Report { get; set; }
	}
}