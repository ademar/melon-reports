using System.Collections;
using System.Data;
using Melon.Reports.Objects;

namespace Melon.Reports
{
	public class Generator
	{
		private readonly Report report;
		private readonly Calculator calculator;

		private int h;

		public Generator(Report report)
		{
			this.report = report; 
			calculator = new Calculator(new ExpressionBuilder(report.Fields, report.Variables, report.Expressions));
		}

		public Document FillReport()
		{
			return BuildDocument();
		}

		private Document BuildDocument()
		{
			calculator.Init();

			var document = new Document
			               	{
			               		Fonts = report.Fonts, 
								Images = new Image[report.ImageCollection.Count],
								Height = report.Height,
								Width = report.Width
			               	};

			report.ImageCollection.Values.CopyTo(document.Images, 0);

			var page = openPage(document);

			var RECORD_COUNT = 0;
			var PAGE_NUMBER = 1;

			calculator.SetField("PageNumber", PAGE_NUMBER);

			var dataReader = GetDataReader();

			while (dataReader.Read())
			{
				RECORD_COUNT++;

				calculator.UpdateFields(report.Fields, dataReader);
				calculator.EvaluateExpressions(report.Expressions);
				calculator.SetField("GlobalRecordCount", RECORD_COUNT);

				appendGroupFooters(page, report.Groups);
                appendGroupHeaders(page);
                
				foreach (var band in report.Detail.Bands)
				{
					if (pageBreak())
					{
						appendPageFooter(page);

						page = openPage(document);

						calculator.SetField("PageNumber", PAGE_NUMBER++);

					}

                    appendBand(page, band);
				}
			}

			appendPageFooter(page);

			return document;
		}

		private bool pageBreak()
		{
			return h < (report.BottonMargin + report.PageFooter.Height);
		}

		private Page openPage(Document document)
		{
			var page = document.CreatePage();

			resetH(); 
			appendPageHeader(page);

			return page;
		}

		private void resetH()
		{
			h = report.Height - report.TopMargin;
		}

		private void appendBand(Page page, Band band)
		{
			page.PutBand(band, h);
			h -= band.Height;
		}

		private void PutBands(Page page, BandCollection bands)
		{
			foreach (var band in bands.Bands)
			{
				appendBand(page, band);
			}
		}

		private void appendPageHeader(Page page)
		{
			PutBands(page,report.PageHeader);
		}

		private void appendPageFooter(Page page)
		{
			PutBands(page,report.PageFooter);
		}

		private void appendGroupHeaders(Page page)
		{
			foreach (Group g in report.Groups)
			{
				var o = calculator.EvaluateVariable(g.Invariant);

				if (Equals(o, g.Value)) continue;

				g.OnGroupChange(new GroupChangeEventArgs(report));
				g.Value = o;
                g.IsOpen = true;

				PutBands(page, g.GroupHeader);
			}
		}

		private void appendGroupFooters(Page page, ArrayList groups)
		{
			var reversedGroups = (ArrayList)groups.Clone();
			reversedGroups.Reverse();

			foreach (Group g in reversedGroups)
			{
				var o = calculator.EvaluateVariable(g.Invariant);

				if (Equals(o, g.Value) || !g.IsOpen) continue;

				g.IsOpen = false;
				PutBands(page,g.GroupFooter);
			}
		}

		private IDataReader GetDataReader()
		{
			var com = Connection.CreateCommand();
			com.CommandText = report.QueryString;

			Connection.Open();

			return com.ExecuteReader();
		}

		public IDbConnection Connection { get; set; }
	}
}