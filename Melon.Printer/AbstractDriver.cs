using System.IO;
using Melon.Reports.Objects;

namespace Melon.Printer
{
	public abstract class AbstractDriver
	{
		public Stream printStream = null ;

		public AbstractDriver(Stream stream)
		{
			printStream = stream ;
		}

		public abstract void Print(Document document);
	}
}
