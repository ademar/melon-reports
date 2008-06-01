using System;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Group.
	/// </summary>
	public class Group
	{
		string name  = null ;
		string invariant = null ;
		object actualvalue = null ;
		int counter = 0 ;
		bool open = false ;

		BandCollection groupHeader = new BandCollection();
		BandCollection groupFooter = new BandCollection();

		//event declaration
		public event GroupChangeEventHandler GroupChange;

		public void OnGroupChange(GroupChangeEventArgs e)
		{
			if (GroupChange != null)
			{
				GroupChange(this,e);
			}
		}

		public Group()
		{}

		public Group(string name)
		{
			this.name = name ;
		}

		public string Name 
		{
			get 
			{
				return this.name ;
			}
			set 
			{
				this.name = value;
			}
		}
		public string Invariant 
		{
			get 
			{
				return this.invariant ;
			}
			set 
			{
				this.invariant = value ;
			}
		}
		public BandCollection GroupHeader
		{
			get
			{
				return groupHeader ;
			}
			set 
			{
				this.groupHeader = value ;
			}
		}
		public BandCollection GroupFooter 
		{
			get
			{
				return groupFooter ;
			}
			set 
			{
				this.groupFooter = value ;
			}
		}

		public object Value 
		{
			set 
			{
				this.actualvalue = value ;
			}
			get
			{
				return this.actualvalue;
			}
		}

		public int Counter 
		{
			get
			{
				return counter ;
			}
			set
			{
				counter = value ;
			}
		}

		public bool IsOpen 
		{
			get
			{
				return open ;
			}
			set 
			{
				open = value ;
			}
		}

	}
}
