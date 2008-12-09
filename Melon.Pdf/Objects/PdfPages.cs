// created on 3/13/2002 at 5:33 PM
using System.Collections.Generic;
using System.Globalization;

namespace Melon.Pdf.Objects
{
	public class PdfPages : PdfObject
	{
		protected IList<string> kids = new List<string>();

		public PdfPages(int number) : base(number)
		{
		}

		public void addPage(PdfPage page)
		{
			kids.Add(page.Reference);
			Count++;

			page.Parent  = Reference;
		}

		public int Count { get; protected set; }

		public override string ToPdf()
		{
			var s = string.Format(CultureInfo.InvariantCulture, "{0} {1} obj\n<< /Type /Pages\n/Count {2}\n/Kids [", Number,Generation, Count);

			var it = kids.GetEnumerator();

			while (it.MoveNext())
			{
				s = string.Format("{0} {1}", s, it.Current);//BUG: 
			}

			s = s + " ] >>\nendobj\n";

			return s;
		}
	}
}