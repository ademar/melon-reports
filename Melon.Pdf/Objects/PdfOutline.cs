using System.Collections;
using System.Text;

namespace Melon.Pdf.Objects
{
	/// <summary>
	/// Represents an Outline Item Dictionary.
	/// </summary>
	public class PdfOutline : PdfObject
	{
		private PdfOutline parent;

		private PdfOutline prev;
		private PdfOutline next;

		private PdfOutline first;
		private PdfOutline last;

		private int count;

		private readonly string dest;

		private readonly ArrayList childs = new ArrayList();

		public PdfOutline(int number, string title, string dest) : base(number)
		{
			Title = title;
			this.dest = dest;
		}

		public string Title { get; set; }

		public void AddItem(PdfOutline outline)
		{
			if (childs.Count > 0)
			{
				outline.prev = (PdfOutline) childs[childs.Count - 1];
				outline.prev.next = outline;
			}
			else
			{
				first = outline;
			}

			childs.Add(outline);
			outline.parent = this;

			last = outline;
		}

		public override string ToPdf()
		{
			var str = new StringBuilder(string.Format("{0} {1} obj\n<<\n", Number, Generation));

			if (parent == null)
			{
				if (first != null && last != null)
				{
					str.Append(" /First " + first.Reference + "\n");
					str.Append(" /Last " + last.Reference + "\n");
				}
			}
			else
			{
				str.Append(" /Title (" + Title + ")\n");
				str.Append(" /Parent " + parent.Reference + "\n");

				if (first != null && last != null)
				{
					str.Append(" /First " + first.Reference + "\n");
					str.Append(" /Last " + last.Reference + "\n");
				}
				if (prev != null)
				{
					str.Append(" /Prev " + prev.Reference + "\n");
				}
				if (next != null)
				{
					str.Append(" /Next " + next.Reference + "\n");
				}
				if (count > 0)
				{
					str.Append(" /Count -" + count + "\n");
				}
				if (dest != null)
				{
					str.Append(" /Dest " + dest + "\n");
				}
			}
			str.Append(">> endobj\n");

			return str.ToString();
		}
	}
}