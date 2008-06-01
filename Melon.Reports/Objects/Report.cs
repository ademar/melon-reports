using System.Collections;

namespace Melon.Reports.Objects
{
	public class Report
	{
		private string m_title;
		private int m_width;
		private int m_height;
		private int m_left_margin,m_right_margin,m_top_margin,m_botton_margin ;

		private string m_queryString;
		private string m_connectionString;


		readonly Hashtable variableCollection = new Hashtable();
		readonly Hashtable fieldCollection = new Hashtable();
		readonly Hashtable parameterCollection = new Hashtable();

		readonly Hashtable expressionCollection = new Hashtable();
		readonly Hashtable imageCollection = new Hashtable();

		ArrayList fontCollection = new ArrayList();

		BandCollection pageTitle = new BandCollection();
		BandCollection reportHeader = new BandCollection();
		BandCollection pageHeader = new BandCollection();
		BandCollection detail = new BandCollection();
		BandCollection pageFooter = new BandCollection();
		BandCollection summary = new BandCollection();


		readonly ArrayList groupCollection = new ArrayList();

		// compiled CIL
		/*object compiledExpressions = null;
		Type compiledType =  null ;*/

		public Report(){}

		public Report(string title, int width, int height)
		{
			m_title = title;
			m_height = height;
			m_width = width;
		}
		
		public void AddField(Field f)
		{
			fieldCollection.Add(f.Name,f);
		}
		
		public void AddVariable(Variable v)
		{
			variableCollection.Add(v.Name,v);
		}
		
		public void AddParameter(Parameter p)
		{
			parameterCollection.Add(p.Name,p);
		}

		public void AddFont(Font f)
		{
			fontCollection.Add(f);
		}
		
		public string Title 
		{
			get
			{
				return m_title;
			}
			set
			{
				m_title = value;
			}
		}
		
		public int Width
		{
			get 
			{
				return m_width;
			}
			set 
			{
				m_width = value;
			}
		}
		
		public int Height
		{
			get 
			{
				return m_height;
			}
			set
			{
				m_height = value ;
			}
		}
		
		public int LeftMargin
		{
			get
			{
				return m_left_margin;
			}
			set
			{
				m_left_margin = value ;
			}
		}
		
		public int RightMargin
		{
			get
			{
				return m_right_margin;
			}
			set
			{
				m_right_margin = value ;
			}
		}
		
		public int TopMargin
		{
			get
			{
				return m_top_margin;
			}
			set
			{
				m_top_margin = value ;
			}
		}
		
		public int BottonMargin
		{
			get
			{
				return m_botton_margin;
			}
			set
			{
				m_botton_margin = value ;
			}
		}
		
		public ArrayList Fonts 
		{
			get
			{
				return fontCollection;
			}
			set 
			{
				fontCollection = value ;
			}
		}
		
		public string ConnectionString
		{
			get 
			{
				return m_connectionString;
			}
			set 
			{
				m_connectionString = value ;
			}
		}

		public string QueryString
		{
			get 
			{
				return m_queryString;
			}
			set 
			{
				m_queryString = value ;
			}
		}
		
		public BandCollection PageTitle 
		{
			get 
			{
				return pageTitle;
			}
			set 
			{
				pageTitle = value;
			}
		}
		
		public BandCollection ReportHeader 
		{
			get 
			{
				return reportHeader;
			}
			set 
			{
				reportHeader = value;
			}
		}
		
		public BandCollection PageHeader
		{
			get
			{
				return pageHeader ;
			}
			set 
			{
				pageHeader = value;
			}
		}
		
		public BandCollection Detail 
		{
			get 
			{
				return detail;
			}
			set 
			{
				detail = value ;
			}
		}
		
		public BandCollection PageFooter 
		{
			get
			{
				return pageFooter ;
			}
			set 
			{
				pageFooter = value ;
			}
		}
		
		public BandCollection Summary 
		{
			get
			{
				return summary;
			}
			set
			{
				summary = value ;
			}
		}
		
		public Hashtable Fields
		{
			get
			{
				return fieldCollection ;
			}
		}
		
		public ArrayList Groups
		{
			get 
			{
				return groupCollection ;
			}
		}


		public Hashtable VariableCollection
		{
			get { return variableCollection; }
		}

		public Hashtable FieldCollection
		{
			get { return fieldCollection; }
		}

		public Hashtable ParameterCollection
		{
			get { return parameterCollection; }
		}

		public Hashtable ExpressionCollection
		{
			get { return expressionCollection; }
		}

		public ArrayList FontCollection
		{
			get { return fontCollection; }
		}

		public ArrayList GroupCollection
		{
			get { return groupCollection; }
		}

		public Hashtable ImageCollection
		{
			get { return imageCollection; }
		}
	}
}

