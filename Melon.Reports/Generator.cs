using System.Collections;
using System.Data;
using Melon.Reports.Objects;

namespace Melon.Reports
{
	public class Generator
	{
		private readonly Report report;
		private int h;
		private readonly ExpressionBuilder expressionBuilder;
		private readonly Calculator calculator;

		public Generator(Report report)
		{
			Connection = null;
			this.report = report;

			expressionBuilder = new ExpressionBuilder(report.Fields, report.VariableCollection, report.ExpressionCollection);

			calculator = new Calculator(expressionBuilder);
		}

		public void FillReport()
		{
			expressionBuilder.BuildExpressions(report);

			Document = new Document {Fonts = report.Fonts, Images = new Image[report.ImageCollection.Count]};

			report.ImageCollection.Values.CopyTo(Document.Images, 0);

			var com = Connection.CreateCommand();
			com.CommandText = report.QueryString;

			Connection.Open();

			var dataReader = com.ExecuteReader();

			h = report.Height - report.TopMargin;

			var page = Document.AddPage();
			page.Height = report.Height;
			page.Width = report.Width;

			// DOUBT : cuando se evaluan el header y el footer ;
			// dos opciones : antes de evaluar la pagina o despues
			// TODO : tengo que resolver el problema de las expresiones en el primer header

			page.PutBands(report.PageHeader, ref h);

			int RECORD_COUNT = 0;
			int PAGE_NUMBER = 1;

			expressionBuilder.SetField("PageNumber", PAGE_NUMBER);

			//a reversed copy of the group array
			var reversedGroups = (ArrayList) report.Groups.Clone();
			reversedGroups.Reverse();

			while (dataReader.Read())
			{
				RECORD_COUNT++;

				calculator.UpdateFields(report.Fields, dataReader, report);
				//Evaluate Variables

				expressionBuilder.SetField("GlobalRecordCount", RECORD_COUNT);

				calculator.EvaluateExpressions(report);

				//maybe it can be improved
				//check groups -- close in reversed order
				foreach (Group g in reversedGroups)
				{
					var o = expressionBuilder.EvaluateVariable(g.Invariant);
					if (!Equals(o, g.Value) && g.IsOpen)
					{
						page.PutBands(g.GroupFooter, ref h);
						g.IsOpen = false;
					}
				}

				foreach (Group g in report.Groups)
				{
					//get actual value of asociated variable
					var o = expressionBuilder.EvaluateVariable(g.Invariant);
					if (!Equals(o, g.Value))
					{
						g.OnGroupChange(new GroupChangeEventArgs(report));
						g.Value = o;
						g.Counter = 0;
						page.PutBands(g.GroupHeader, ref h);
						g.IsOpen = true;
					}

					g.Counter++;
				}

				// details

				IEnumerator it = report.Detail.Bands.GetEnumerator(); //las bandas del detalle
				while (it.MoveNext())
				{
					var band = (Band) it.Current;


					// HACK : page break??
					if (h < (report.BottonMargin + report.PageFooter.Height))
					{
						page.PutBands(report.PageFooter, ref h);
						page = Document.AddPage();
						page.Height = report.Height;
						page.Width = report.Width;
						h = report.Height - report.TopMargin; //reset h
						page.PutBands(report.PageHeader, ref h);
						PAGE_NUMBER ++;
						expressionBuilder.SetField("PageNumber", PAGE_NUMBER);
					}

					page.PutBand(band, h);
					h -= band.Height;
				}
			}
			page.PutBands(report.PageFooter, ref h); //one last footer
		}

		public Document Document { get; private set; }

		public IDbConnection Connection { get; set; }
	}
}