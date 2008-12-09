using System;
using System.Xml;
using Melon.Commons;
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
			var report = new Report();

			Variable v;

			while (reader.Read()) // forward parsing
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						
						switch (reader.Name)
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
								var field = new Field(reader.GetAttribute("name")) {Type = reader.GetAttribute("type")};
								report.AddField(field);
								break;
							case "Variable":
								v = new Variable(reader.GetAttribute("name"))
								    	{
								    		Type = reader.GetAttribute("type"),
								    		Level = reader.GetAttribute("level")
								    	};
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
								var f = new Font(reader.GetAttribute("name"), reader.GetAttribute("fontName"));
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
								var g = new Group(reader.GetAttribute("name"))
								        	{
								        		Invariant = reader.GetAttribute("invariant"),
								        		GroupHeader = ParseBands(reader, "groupHeader", report),
								        		GroupFooter = ParseBands(reader, "groupFooter", report)
								        	};

								// the variable has to exist
								if (report.VariableCollection[g.Invariant] == null)
								{
									throw new Exception("Unknown variable : " + g.Invariant);
								}
								
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

		private static BandCollection ParseBands(XmlReader reader, string endTag, Report report)
		{
			var bands = new BandCollection();

			while (reader.Read()) //make sure this loop ends
			{
				if (reader.Name.Equals(endTag) && (reader.NodeType == XmlNodeType.EndElement))
					break;
				if (reader.Name == "Band" && reader.NodeType == XmlNodeType.Element)
				{
					var band = new Band(XmlConvert.ToInt16(reader.GetAttribute("height"))) {parent = report};
					bands.AddBand(band);
					//parse Band content
					ParseBand(reader, band, report);
				}
			}

			return bands;
		}

		private static void ParseBand(XmlReader reader, Band band, Report report)
		{
			while (reader.Read())
			{
				if (reader.Name.Equals("Band") && (reader.NodeType == XmlNodeType.EndElement)) break;
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						int x;
						int y;
						int fontSize;
						int height;
						int width;
						switch (reader.Name)
						{
							case "StaticText":
								x = XmlConvert.ToInt16(reader.GetAttribute("x"));
								y = XmlConvert.ToInt16(reader.GetAttribute("y"));
								fontSize = XmlConvert.ToInt16(reader.GetAttribute("font-size"));
								var color = reader.GetAttribute("color");
								var content = reader.ReadString();
								var t = new Text(content, Text.Alignment.Left, x, y) {FontSize = fontSize, color = new Color(color)};
								band.Elements.Add(t);
								break;
							case "Expression":
								x = XmlConvert.ToInt16(reader.GetAttribute("x"));
								y = XmlConvert.ToInt16(reader.GetAttribute("y"));
								fontSize = XmlConvert.ToInt16(reader.GetAttribute("font-size"));
								var strtype = reader.GetAttribute("type");
								var e = new Expression(reader.ReadString()) {X = x, Y = y, FontSize = fontSize, Type = strtype};
								band.Elements.Add(e);
								band.parent.ExpressionCollection.Add(e.GetHashCode(), e);
								break;
							case "Image":
								x = XmlConvert.ToInt16(reader.GetAttribute("x"));
								y = XmlConvert.ToInt16(reader.GetAttribute("y"));
								height = XmlConvert.ToInt16(reader.GetAttribute("height"));
								width = XmlConvert.ToInt16(reader.GetAttribute("width"));
								var url = reader.GetAttribute("href");

								//fix the path to image
								/*if (!Path.IsPathRooted(url))
								{
									url = Path.Combine(this.workDirectory,url);
								}*/

								//check if it is already in
								var i = (Image) report.ImageCollection[url];
								if (i == null) // NOTE : i don't like this way 
								{
									i = new Image(url, x, y) {width = width, height = height};
									report.ImageCollection.Add(url, i); //Add to global image array
									i.IName = new ImageName();
									band.Elements.Add(i);
								}
								else
								{
									var i2 = new Image(url, x, y) {width = width, height = height, IName = i.IName};
									//pon el nombre apuntando a la misma referencia
									// la idea es que estas imagenes comparten el mismo nombre, capice?
									band.Elements.Add(i2);
								}

								break;
							case "Rectangle":
								x = XmlConvert.ToInt16(reader.GetAttribute("x"));
								y = XmlConvert.ToInt16(reader.GetAttribute("y"));
								height = XmlConvert.ToInt16(reader.GetAttribute("height"));
								width = XmlConvert.ToInt16(reader.GetAttribute("width"));

								var bordercolor = reader.GetAttribute("bordercolor");
								var fillcolor = reader.GetAttribute("fillcolor");

								var r = new Rectangle
								              	{
								              		x = x,
								              		y = y,
								              		width = width,
								              		height = height,
								              		bordercolor = new Color(bordercolor),
								              		fillcolor = new Color(fillcolor)
								              	};
								band.Elements.Add(r);
								break;
							case "Bookmark":
								var var = reader.GetAttribute("var");
								var id = reader.GetAttribute("id"); //TODO :  termina esto
								var b = new Bookmark(var);
								band.Elements.Add(b);
								break;
						}
						break;
				}
			}
		}
	}
}