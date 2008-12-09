using System.Collections;
using System.Text;

namespace Melon.Pdf.Objects
{
	/// <summary>
	/// Represents an Outline Item Dictionary.
	/// </summary>
	public class PDFOutline : PDFObject
	{
		PDFOutline parent; 

		PDFOutline prev;
		PDFOutline next;

		PDFOutline first;
		PDFOutline last;

		int count;

		readonly string dest ;

		readonly ArrayList childs  = new ArrayList();

		public PDFOutline(int number, string title, string dest):base(number)
		{
			Title = title ;
			this.dest = dest ;
		}

		public string Title { get; set; }

		public void AddItem(PDFOutline outline)
		{
			if(childs.Count > 0)
			{
				outline.prev = (PDFOutline)childs[childs.Count - 1];
				outline.prev.next = outline ;
			}
			else
			{
				first = outline ;
			}

			childs.Add(outline);
			outline.parent = this ;

			last = outline;
		}

		public override string ToPDF()
		{
			 var str =  new StringBuilder(string.Format("{0} {1} obj\n<<\n", Number, Generation));

			 if (parent==null)
			 {
				 if(first!=null && last!=null)
				 {
					 str.Append(" /First " + first.Reference + "\n");
					 str.Append(" /Last " + last.Reference + "\n");
				 }
			 }
			 else
			 {
				 str.Append(" /Title (" + Title + ")\n");
				 str.Append(" /Parent " + parent.Reference + "\n");

				 if (first!=null && last!=null)
				 {
					 str.Append(" /First " + first.Reference + "\n");
					 str.Append(" /Last " + last.Reference + "\n");
				 }
				 if (prev!=null)
				 {
					 str.Append(" /Prev " + prev.Reference + "\n");
				 }
				 if (next!=null)
				 {
					 str.Append(" /Next " + next.Reference + "\n");
				 }
				 if (count>0)
				 {
					 str.Append(" /Count -" + count + "\n");
				 }
				 if (dest!=null)
				 {
					str.Append(" /Dest " + dest + "\n");
				 }
			 }
			 str.Append(">> endobj\n");

			 return str.ToString();
		}
	}
}
