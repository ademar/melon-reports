using System;
using System.Xml;
using Melon.Reports.Objects;

namespace Melon.Reports
{
	public class Parser
	{
		private readonly XmlReader reader;

		public Parser(XmlReader reader)
		{
			this.reader = reader;
		}

		public Report Parse()
		{
			Report report = new Report();

			Variable v;

			while (reader.Read()) // forward parsing
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						//do the walk
						string rr = reader.Name;
						switch (rr)
						{
							case "MelonReport":
								report.Title = reader.GetAttribute("title");
								report.Height = XmlConvert.ToInt16(reader.GetAttribute("height"));
								report.Width = XmlConvert.ToInt16(reader.GetAttribute("width"));
								report.LeftMargin = XmlConvert.ToInt16(reader.GetAttribute("left-margin"));
								report.RightMargin = XmlConvert.ToInt16(reader.GetAttribute("right-margin"));
								report.TopMargin = XmlConvert.ToInt16(reader.GetAttribute("top-margin"));
								report.BottonMargin = XmlConvert.ToInt16(reader.GetAttribute("botton-margin"));
								break;
							case "Connection":
								report.ConnectionString = reader.GetAttribute("String");
								break;
							case "QueryString":
								report.QueryString = reader.ReadString();
								break;
							case "Field":
								Field field = new Field(reader.GetAttribute("name"));
								field.Type = reader.GetAttribute("type");
								report.AddField(field);
								break;
							case "Variable":
								v = new Variable(reader.GetAttribute("name"));
								v.Type = reader.GetAttribute("type");
								v.Level = reader.GetAttribute("level");
								if (v.Level.Equals(Variable.RESET_TYPE_GROUP))
									v.ResetingGroup = reader.GetAttribute("group");
								v.Formula = reader.GetAttribute("formula");
								v.Expression = reader.ReadString();
								report.AddVariable(v);
								break;
							case "Parameter":
								report.AddParameter(new Parameter(reader.GetAttribute("name")));
								break;
							case "ReportFont":
								Font f = new Font(reader.GetAttribute("name"), reader.GetAttribute("fontName"));
								if (reader.MoveToAttribute("default"))
									f.IsDefault = XmlConvert.ToBoolean(reader.GetAttribute("default"));
								report.AddFont(f);
								break;
							case "Title":
								report.PageTitle = ParseBands(reader, "Title", report);
								break;
							case "ReportHeader":
								report.ReportHeader = ParseBands(reader, "ReportHeader", report);
								break;
							case "PageHeader":
								report.PageHeader = ParseBands(reader, "PageHeader", report);
								break;
							case "Detail":
								report.Detail = ParseBands(reader, "Detail", report);
								break;
							case "PageFooter":
								report.PageFooter = ParseBands(reader, "PageFooter", report);
								break;
							case "Summary":
								report.Summary = ParseBands(reader, "Summary", report);
								break;
							case "Group": // the groups stuff
								Group g = new Group(reader.GetAttribute("name"));
								g.Invariant = reader.GetAttribute("invariant");
								g.GroupHeader = ParseBands(reader, "groupHeader", report);
								g.GroupFooter = ParseBands(reader, "groupFooter", report);

								// the variable has to exist
								if (report.VariableCollection[g.Invariant] == null)
								{
									throw new Exception("Unknown variable : " + g.Invariant);
								};

								// has to do the wiring to all variables with this group as reseter
								foreach (Variable var in report.VariableCollection.Values)
								{

									if (var.Level.Equals(Variable.RESET_TYPE_GROUP) && var.ResetingGroup.Equals(g.Name))
									{
										g.GroupChange += var.UpdateMe;
									}
								}


								report.Groups.Add(g);
								break;
						}
						break;
				}
			}
			return report;
		}

		private BandCollection ParseBands(XmlReader reader, string endTag, Report report)
		{
			BandCollection bands = new BandCollection();
			while (reader.Read()) //make sure this loop ends
			{
				if (reader.Name.Equals(endTag) && (reader.NodeType == XmlNodeType.EndElement))
					break;
				if (reader.Name == "Band" && reader.NodeType == XmlNodeType.Element)
				{
					Band band = new Band(XmlConvert.ToInt16(reader.GetAttribute("height")));
					band.parent = report;
					bands.AddBand(band);
					//parse Band content
					ParseBand(reader, band,report);
				}

			}
			return bands;

		}

		private void ParseBand(XmlReader reader, Band band, Report report)
		{
			int x, y, fontSize, height, width;
			while (reader.Read())
			{
				if (reader.Name.Equals("Band") && (reader.NodeType == XmlNodeType.EndElement)) break;
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						switch (reader.Name)
						{
							case "StaticText":
								x = XmlConvert.ToInt16(reader.GetAttribute("x"));
								y = XmlConvert.ToInt16(reader.GetAttribute("y"));
								fontSize = XmlConvert.ToInt16(reader.GetAttribute("font-size"));
								string color = reader.GetAttribute("color");
								string content = reader.ReadString();
								Text t = new Text(content, Text.Alignment.Left, x, y);
								t.FontSize = fontSize;
								t.color = new Color(color);
								band.AddElement(t);
								break;
							case "Expression":
								x = XmlConvert.ToInt16(reader.GetAttribute("x"));
								y = XmlConvert.ToInt16(reader.GetAttribute("y"));
								fontSize = XmlConvert.ToInt16(reader.GetAttribute("font-size"));
								string strtype = reader.GetAttribute("type");
								Expression e = new Expression(reader.ReadString());
								e.X = x;
								e.Y = y;
								e.FontSize = fontSize;
								e.Type = strtype;
								band.AddElement(e);
								band.parent.ExpressionCollection.Add(e.GetHashCode(), e);
								break;
							case "Image":
								x = XmlConvert.ToInt16(reader.GetAttribute("x"));
								y = XmlConvert.ToInt16(reader.GetAttribute("y"));
								height = XmlConvert.ToInt16(reader.GetAttribute("height"));
								width = XmlConvert.ToInt16(reader.GetAttribute("width"));
								string url = reader.GetAttribute("href");

								//fix the path to image
								/*if (!Path.IsPathRooted(url))
								{
									url = Path.Combine(this.workDirectory,url);
								}*/

								//check if it is already in
								Image i = (Image)report.ImageCollection[url];
								if (i == null)// NOTE : i don't like this way 
								{
									i = new Image(url, x, y);
									i.width = width;
									i.height = height;
									report.ImageCollection.Add(url, i); //Add to global image array
									i.IName = new ImageName();
									band.AddElement(i);
								}
								else
								{
									Image i2 = new Image(url, x, y);
									i2.width = width;
									i2.height = height;
									//pon el nombre apuntando a la misma referencia
									// la idea es que estas imagenes comparten el mismo nombre, capice?
									i2.IName = i.IName;
									band.AddElement(i2);
								}

								break;
							case "Rectangle":
								x = XmlConvert.ToInt16(reader.GetAttribute("x"));
								y = XmlConvert.ToInt16(reader.GetAttribute("y"));
								height = XmlConvert.ToInt16(reader.GetAttribute("height"));
								width = XmlConvert.ToInt16(reader.GetAttribute("width"));
								string bordercolor = reader.GetAttribute("bordercolor");
								string fillcolor = reader.GetAttribute("fillcolor");
								Rectangle r = new Rectangle();
								r.x = x;
								r.y = y;
								r.width = width;
								r.height = height;
								r.bordercolor = new Color(bordercolor);
								r.fillcolor = new Color(fillcolor);
								band.AddElement(r);
								break;
							case "Bookmark":
								string var = reader.GetAttribute("var");
								string id = reader.GetAttribute("id");//TODO :  termina esto
								Bookmark b = new Bookmark(var);
								band.AddElement(b);
								break;
						}
						break;
				}
			}
		}
	}
}
