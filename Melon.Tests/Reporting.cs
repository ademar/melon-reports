using System.Data;
using System.IO;
using Melon.Printer;
using Melon.Reports;
using Melon.Reports.Objects;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Melon.Tests
{
	[TestFixture]
	public class Reporting
	{
		private Report report;
		private Document document;

		[Test]
		public void Load()
		{
			var reader = new ReportReader();

			report = reader.Load("WorldPopulation.xml");
		}

		[Test]
		public void Generate()
		{
			IDbConnection cn = new MySqlConnection("Server=localhost;Database=world;User ID=testuser;Password=password;");

			var generator = new Generator(report) {Connection = cn};

			document = generator.FillReport();
		}

		[Test]
		public void Print()
		{
			var printer = new PrintManager();

			var f = new FileStream(@"C:\Temp\report.pdf", FileMode.Create, FileAccess.Write);

			var driver = new PdfDriver();

			printer.Print(document, driver, f);

			f.Close();
		}
	}
}