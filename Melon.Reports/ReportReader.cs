using System;
using System.Xml;
using System.Xml.Schema;
using Melon.Reports.Objects;

namespace Melon.Reports
{
	public class ReportReader
	{
		public Report Load(string filename)
		{
			var xmlColl = new XmlSchemaSet();

			var sTest = typeof (Report).Assembly.GetManifestResourceStream("Melon.Reports.Schemas.melon-0.5.xsd");

			if (sTest != null)
			{
				var schemaReader = new XmlTextReader(sTest);

				xmlColl.Add("melon-0.5.xsd", schemaReader);
			}

			var settings = new XmlReaderSettings {ValidationType = ValidationType.Schema, Schemas = xmlColl};

			settings.ValidationEventHandler += ValidationEventHandle;

			/*settings.WhitespaceHandling = WhitespaceHandling.Significant;
			settings.Normalization = true;*/

			var reader = XmlReader.Create(filename, settings);

			var parser = new Parser(reader);

			var report = parser.Parse();

			reader.Close();

			return report;
		}

		public void ValidationEventHandle(object sender, ValidationEventArgs args)
		{
			throw new Exception(args.Message);
		}
	}
}