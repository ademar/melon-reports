namespace Melon.Reports.Objects
{
	public class Group
	{
		
		public event GroupChangeEventHandler GroupChange;

		public void OnGroupChange(GroupChangeEventArgs e)
		{
			if (GroupChange != null)
			{
				GroupChange(this, e);
			}
		}

		public Group()
		{
			GroupFooter = new BandCollection();
			GroupHeader = new BandCollection();
		}

		public Group(string name)
		{
			GroupFooter = new BandCollection();
			GroupHeader = new BandCollection();
			Name = name;
		}

		public string Name { get; set; }
        public string Invariant { get; set; }
        public object Value { set; get; }
        public bool IsOpen { get; set; }

		public BandCollection GroupHeader { get; set; }
		public BandCollection GroupFooter { get; set; }
	}
}