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
		public void	Load()
		{
			ReportReader reader = new ReportReader();

			report = reader.Load("WorldPopulation.xml");
		}

		[Test]
		public void Generate()
		{
			IDbConnection cn = new MySqlConnection("Server=localhost;Database=world;User ID=user;Password=password;");

			Generator generator = new Generator(report);
			
			generator.Connection = cn;
			generator.FillReport();

			document = generator.Doc;

		}

		[Test]
		public void Print()
		{
			PrintManager printer = new PrintManager();

			FileStream f = new FileStream("report.pdf", FileMode.Create, FileAccess.Write);

			PDFDriver driver = new PDFDriver(f);

			printer.Print(document, driver);

			f.Close();
		}
	}
}
