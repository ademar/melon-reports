using System.Collections;
using System.Data;
using Melon.Reports.Objects;

namespace Melon.Reports
{
	public class Generator
	{
		private readonly Report report;
		private int h;
		private readonly Calculator calculator;

		public Generator(Report report)
		{
			this.report = report;

			calculator = new Calculator(report);
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

			h = report.Height - report.TopMargin;

			var page = document.CreatePage();
			page.PutBands(report.PageHeader, ref h);

			var RECORD_COUNT = 0;
			var PAGE_NUMBER = 1;

			calculator.SetField("PageNumber", PAGE_NUMBER);

			var reversedGroups = (ArrayList) report.Groups.Clone();
			reversedGroups.Reverse();

			var dataReader = GetDataReader();

			while (dataReader.Read())
			{
				RECORD_COUNT++;

				calculator.UpdateFields(report.Fields, dataReader);
				calculator.EvaluateExpressions(report.Expressions);
				calculator.SetField("GlobalRecordCount", RECORD_COUNT);

				appendGroupFooters(page, reversedGroups);
                appendGroupHeaders(page);
                
				foreach (var band in report.Detail.Bands)
				{
					// page break
					if (h < (report.BottonMargin + report.PageFooter.Height))
					{
						appendPageFooter(page);

						page = document.CreatePage();

						//reset h
						h = report.Height - report.TopMargin; 

						appendPageHeader(page);

						PAGE_NUMBER ++;

						calculator.SetField("PageNumber", PAGE_NUMBER);

					}

					appendDetailBand(page, band);

					h -= band.Height;
				}
			}

			appendPageFooter(page);

			return document;
		}

		private void appendDetailBand(Page page, Band band)
		{
			page.PutBand(band, h);
		}

		private void appendPageHeader(Page page)
		{
			page.PutBands(report.PageHeader, ref h);
		}

		private void appendPageFooter(Page page)
		{
			page.PutBands(report.PageFooter, ref h);
		}

		private void appendGroupHeaders(Page page)
		{
			foreach (Group g in report.Groups)
			{
				var o = calculator.EvaluateVariable(g.Invariant);

				if (Equals(o, g.Value)) continue;

				g.OnGroupChange(new GroupChangeEventArgs(report));
				g.Value = o;
				page.PutBands(g.GroupHeader, ref h);
				g.IsOpen = true;
			}
		}

		private void appendGroupFooters(Page page, ArrayList reversedGroups)
		{
			foreach (Group g in reversedGroups)
			{
				var o = calculator.EvaluateVariable(g.Invariant);

				if (Equals(o, g.Value) || !g.IsOpen) continue;

				page.PutBands(g.GroupFooter, ref h);
				g.IsOpen = false;
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