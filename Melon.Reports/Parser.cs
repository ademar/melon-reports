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

			while (reader.Read()) 
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
								report.Fields.Add(field);
								break;
							case "Variable":
								var variable = new Variable(reader.GetAttribute("name"))
								             	{
								             		Type = reader.GetAttribute("type"),
								             		Level = reader.GetAttribute("level")
								             	};
								if (variable.Level.Equals(Variable.RESET_TYPE_GROUP))
									variable.ResetingGroup = reader.GetAttribute("group");
								variable.Formula = reader.GetAttribute("formula");
								variable.Expression = reader.ReadString();
								report.Variables.Add(variable.Name,variable);
								break;
							case "ReportFont":
								var f = new Font(reader.GetAttribute("name"), reader.GetAttribute("fontName"));
								if (reader.MoveToAttribute("default"))
									f.IsDefault = XmlConvert.ToBoolean(reader.GetAttribute("default"));
								report.Fonts.Add(f);
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
							case "Group": 
								var g = new Group(reader.GetAttribute("name"))
								        	{
								        		Invariant = reader.GetAttribute("invariant"),
								        		GroupHeader = ParseBands(reader, "groupHeader", report),
								        		GroupFooter = ParseBands(reader, "groupFooter", report)
								        	};

								if (report.Variables[g.Invariant] == null)
								{
									throw new Exception("Unknown variable : " + g.Invariant);
								}
								
								foreach (var var in report.Variables.Values)
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

			while (reader.Read()) 
			{
				if (reader.Name.Equals(endTag) && (reader.NodeType == XmlNodeType.EndElement))
					break;
				if (reader.Name == "Band" && reader.NodeType == XmlNodeType.Element)
				{
					var band = new Band(XmlConvert.ToInt16(reader.GetAttribute("height"))) {parent = report};
					bands.Add(band);
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
								var t = new Text(content, TextAlignment.Left, x, y) {FontSize = fontSize, rgbColor = new RgbColor(color)};
								band.Elements.Add(t);
								break;
							case "Expression":
								x = XmlConvert.ToInt16(reader.GetAttribute("x"));
								y = XmlConvert.ToInt16(reader.GetAttribute("y"));
								fontSize = XmlConvert.ToInt16(reader.GetAttribute("font-size"));
								var strtype = reader.GetAttribute("type");
								var e = new Expression(reader.ReadString()) {X = x, Y = y, FontSize = fontSize, Type = strtype};
								band.Elements.Add(e);
								band.parent.Expressions.Add(e);
								break;
							case "Image":
								x = XmlConvert.ToInt16(reader.GetAttribute("x"));
								y = XmlConvert.ToInt16(reader.GetAttribute("y"));
								height = XmlConvert.ToInt16(reader.GetAttribute("height"));
								width = XmlConvert.ToInt16(reader.GetAttribute("width"));
								var url = reader.GetAttribute("href");
                                var i = (Image) report.ImageCollection[url];
								var i2 = new Image(url, x, y) {width = width, height = height, ImageName = i.ImageName};
								band.Elements.Add(i2);
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
								              		bordercolor = new RgbColor(bordercolor),
								              		fillcolor = new RgbColor(fillcolor)
								              	};
								band.Elements.Add(r);
								break;
							case "Bookmark":
								var var = reader.GetAttribute("var");
								var id = reader.GetAttribute("id"); 
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