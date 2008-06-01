using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for GroupChangeEventHandler.
	/// </summary>
	public delegate void  GroupChangeEventHandler(object sender,GroupChangeEventArgs e);

	public class GroupChangeEventArgs : EventArgs 
	{
		Report report;

		public GroupChangeEventArgs(Report report)
		{
			this.report = report ;
		}

		public Report Report 
		{
			get
			{
				return this.report;
			}
			set 
			{
				this.report = value ;
			}
		}

	}
	
}
