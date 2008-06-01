using System;
using System.Collections;
using System.IO;
using Melon.Pdf.Imaging;
using Melon.Pdf.Objects;
using Melon.Reports.Objects;

namespace Melon.Printer
{
	public class PDFDriver : AbstractDriver
	{
		private readonly PDFDocument pdf = null ;

		private PDFPage currentPage = null ;
		private PDFStream currentPDFStream = null ;
		private string defaultFont = null ;

		
		public PDFDriver(Stream stream) : base(stream)
		{
			pdf = new PDFDocument();
		}

		
		public override void Print(Document document){

			// set fonts
			IEnumerator it = document.Fonts.GetEnumerator();
			while(it.MoveNext())
			{
				Font f = (Font)it.Current ;
				pdf.MakeFont(f.Name,PDFFont.TYPE1,f.FontName);
				if(f.IsDefault) defaultFont = f.Name ;

			}

			//add images
			foreach(Image i in document.Images)
			{
				WinImage wi = new WinImage(i.url);
				i.IName.Name = "Im" + pdf.AddImage(wi);
			}

			// start printing
			it = document.Pages.GetEnumerator();
			while(it.MoveNext())
			{
				PrintPage((Page)it.Current);
			}
			pdf.outputHeader(printStream);
			pdf.outputTrailer(printStream);
			
			
			printStream.Flush();
		}

		public void PrintPage(Page page)
		{
			IEnumerator it = page.Elements.GetEnumerator();
			currentPDFStream = pdf.MakeStream();
			currentPage = pdf.MakePage(currentPDFStream,page.Width,page.Height); 
			while(it.MoveNext())
			{
				PrintElement((BasicElement)it.Current);
			}
			
		}
		
		public void PrintElement(BasicElement element)
		{
			Type type = element.GetType();
			if(type == typeof(Text)) 
			{
				PrintText((Text)element);
			}
			else if(type == typeof(Expression)) 
			{
				PrintExpression((Expression)element);
			}
			else if(type == typeof(Image)) 
			{
				PrintImage((Image)element);
			}
			else if(type == typeof(Rectangle)) 
			{
				PrintRectangle((Rectangle)element);
			}
			else if(type == typeof(Bookmark)) 
			{
				PrintBookmark((Bookmark)element);
			}
		}
		
		public void PrintImage(Image image) 
		{
			currentPDFStream.SaveState();
			currentPDFStream.ShowImage(image.IName.Name,image.x,(image.H - image.y - image.height),image.width,image.height);
			currentPDFStream.RestoreState();
		}
		
		public void PrintText(Text text)
		{
			currentPDFStream.SaveState();
			currentPDFStream.SetRGBColorFill(text.color.Red,text.color.Green,text.color.Blue);
			currentPDFStream.BeginText();
			currentPDFStream.SetFont(defaultFont,text.FontSize);
			currentPDFStream.SetTextPos(text.X,(text.H - text.Y));
			currentPDFStream.ShowText(text.Label);
			currentPDFStream.EndText();
			currentPDFStream.RestoreState();
		}
		
		public void PrintExpression(Expression e)
		{
			currentPDFStream.BeginText();
			currentPDFStream.SetFont(defaultFont,e.FontSize);
			currentPDFStream.SetTextPos(e.X,(e.H - e.Y));
			currentPDFStream.ShowText(e.Value.ToString());
			currentPDFStream.EndText();
		}
		
		public void PrintRectangle(Rectangle r)
		{
			currentPDFStream.SaveState();
			currentPDFStream.SetRGBColorFill(r.fillcolor.Red,r.fillcolor.Green,r.fillcolor.Blue);
			currentPDFStream.SetRGBColorStroke(r.bordercolor.Red,r.bordercolor.Green,r.bordercolor.Blue);
			currentPDFStream.ShowRectangle(r.x,r.H - r.y - r.height,r.width,r.height);
			currentPDFStream.RestoreState();
		}
		
		public void PrintBookmark(Bookmark b)
		{
			pdf.MakeOutline(pdf.OutlineRoot,b.VarName,currentPage);
		}
		
	}
}
