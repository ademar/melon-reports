using System.IO;
using Melon.Reports.Objects;

namespace Melon.Printer
{
	public abstract class AbstractDriver
	{
		public abstract void Print(Document document, Stream printStream);
	}
}
