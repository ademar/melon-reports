using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Melon.Reports.Objects
{
	public class Report
	{
		public Report()
		{
			ImageCollection = new Hashtable();
			Expressions = new Collection<Expression>();
			Variables = new Dictionary<string,Variable>();
			Groups = new ArrayList();
			Fields = new Collection<Field>();
            Parameters = new Collection<Parameter>();
            Summary = new BandCollection();
			PageFooter = new BandCollection();
			Detail = new BandCollection();
			PageHeader = new BandCollection();
			ReportHeader = new BandCollection();
			PageTitle = new BandCollection();
			Fonts = new List<Font>();
		}

		public Report(string title, int width, int height):this()
		{
			Title = title;
			Height = height;
			Width = width;
		}

		public string Title { get; set; }

		public int Width { get; set; }
        public int Height { get; set; }
        public int LeftMargin { get; set; }
        public int RightMargin { get; set; }
        public int TopMargin { get; set; }
        public int BottonMargin { get; set; }

		public string ConnectionString { get; set; }
        public string QueryString { get; set; }

		public BandCollection PageTitle { get; set; }
        public BandCollection ReportHeader { get; set; }
        public BandCollection PageHeader { get; set; }
        public BandCollection Detail { get; set; }
        public BandCollection PageFooter { get; set; }
        public BandCollection Summary { get; set; }

		public ArrayList Groups { get; private set; }

		public IList<Font> Fonts { get; set; }
		public Collection<Field> Fields { get; private set; }

        public Collection<Parameter> Parameters { get; private set; }
        public Collection<Expression> Expressions { get; private set; }
		public IDictionary<string, Variable> Variables { get; private set; }

		public Hashtable ImageCollection { get; private set; }
	}
}