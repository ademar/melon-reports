using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Melon.Reports.Objects;

namespace Melon.Reports
{
	public class ReportReader
	{
		public Report Load(string filename)
		{
			XmlSchemaSet xmlColl = new XmlSchemaSet();

			Stream sTest = typeof(Report).Assembly.GetManifestResourceStream("Melon.Reports.Schemas.melon-0.5.xsd");

			XmlTextReader schemaReader = new XmlTextReader(sTest);
			
			xmlColl.Add("melon-0.5.xsd",schemaReader);

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.Schema;
			settings.Schemas = xmlColl;
			
			settings.ValidationEventHandler += this.ValidationEventHandle;

			/*settings.WhitespaceHandling = WhitespaceHandling.Significant;
			settings.Normalization = true;*/

			XmlReader reader = XmlReader.Create(filename,settings);

			Parser parser = new Parser(reader);

			Report report = parser.Parse();

			reader.Close();
			
			return report;
		}
		
		public void ValidationEventHandle(object sender,ValidationEventArgs args)
		{
			throw new Exception(args.Message);
		}

				
	}
}
