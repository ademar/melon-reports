using System.Collections;
using System.Text;

namespace Melon.Pdf.Objects
{
	/// <summary>
	/// Represents an Outline Item Dictionary.
	/// </summary>
	public class PDFOutline : PDFObject
	{
		string title ; //The text to be displayed on the screen for this item.

		PDFOutline parent = null; 

		PDFOutline prev  = null ;
		PDFOutline next  = null ;

		PDFOutline first = null ;
		PDFOutline last = null ;

		int count = 0 ;

		readonly string dest ;

		readonly ArrayList childs  = new ArrayList();

		public PDFOutline(int number, string title, string dest):base(number)
		{
			this.title = title ;
			this.dest = dest ;
		}

		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
			}

		}
		
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

		private void UpdateCount()
		{
			count ++ ;
			if (parent!=null) parent.UpdateCount();
		}
		
		 public override string ToPDF()
		{
			 StringBuilder str =  new StringBuilder(string.Format("{0} {1} obj\n<<\n", number, generation));

			 if (parent==null)
			 {
				 if(first!=null && last!=null)
				 {
					 str.Append(" /First " + first.getReference + "\n");
					 str.Append(" /Last " + last.getReference + "\n");
				 }
			 }
			 else
			 {
				 str.Append(" /Title (" + title + ")\n");
				 str.Append(" /Parent " + parent.getReference + "\n");
				 if (first!=null && last!=null)
				 {
					 str.Append(" /First " + first.getReference + "\n");
					 str.Append(" /Last " + last.getReference + "\n");
				 }
				 if (prev!=null)
				 {
					 str.Append(" /Prev " + prev.getReference + "\n");
				 }
				 if (next!=null)
				 {
					 str.Append(" /Next " + next.getReference + "\n");
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
