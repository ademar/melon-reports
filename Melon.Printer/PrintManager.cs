using System.IO;
using Melon.Reports.Objects;

namespace Melon.Printer
{
	public class PrintManager
	{
		public void Print(Document document, AbstractDriver printerDriver, Stream stream)
		{
			printerDriver.Print(document, stream);
		}
	}
}