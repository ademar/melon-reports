using System.Collections;

namespace Melon.Reports.Objects
{
	public class Report
	{
		public Report()
		{
			ImageCollection = new Hashtable();
			ExpressionCollection = new Hashtable();
			ParameterCollection = new Hashtable();
			VariableCollection = new Hashtable();
			Groups = new ArrayList();
			Fields = new Hashtable();
			Summary = new BandCollection();
			PageFooter = new BandCollection();
			Detail = new BandCollection();
			PageHeader = new BandCollection();
			ReportHeader = new BandCollection();
			PageTitle = new BandCollection();
			Fonts = new ArrayList();
		}

		public Report(string title, int width, int height)
		{
			ImageCollection = new Hashtable();
			ExpressionCollection = new Hashtable();
			ParameterCollection = new Hashtable();
			VariableCollection = new Hashtable();
			Groups = new ArrayList();
			Fields = new Hashtable();
			Summary = new BandCollection();
			PageFooter = new BandCollection();
			Detail = new BandCollection();
			PageHeader = new BandCollection();
			ReportHeader = new BandCollection();
			PageTitle = new BandCollection();
			Fonts = new ArrayList();
			Title = title;
			Height = height;
			Width = width;
		}
		
		public void AddField(Field f)
		{
			Fields.Add(f.Name,f);
		}
		
		public void AddVariable(Variable v)
		{
			VariableCollection.Add(v.Name,v);
		}
		
		public void AddParameter(Parameter p)
		{
			ParameterCollection.Add(p.Name,p);
		}

		public void AddFont(Font f)
		{
			Fonts.Add(f);
		}

		public string Title { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public int LeftMargin { get; set; }

		public int RightMargin { get; set; }

		public int TopMargin { get; set; }

		public int BottonMargin { get; set; }

		public ArrayList Fonts { get; set; }

		public string ConnectionString { get; set; }

		public string QueryString { get; set; }

		public BandCollection PageTitle { get; set; }

		public BandCollection ReportHeader { get; set; }

		public BandCollection PageHeader { get; set; }

		public BandCollection Detail { get; set; }

		public BandCollection PageFooter { get; set; }

		public BandCollection Summary { get; set; }

		public Hashtable Fields { get; private set; }

		public ArrayList Groups { get; private set; }


		public Hashtable VariableCollection { get; private set; }

		public Hashtable ParameterCollection { get; private set; }

		public Hashtable ExpressionCollection { get; private set; }

		public Hashtable ImageCollection { get; private set; }
	}
}

