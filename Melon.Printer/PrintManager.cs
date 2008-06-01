using Melon.Reports.Objects;

namespace Melon.Printer
{
	public class PrintManager
	{
		public void Print(Document document,AbstractDriver printerDriver)
		{
			printerDriver.Print(document);
		}
	}
}
