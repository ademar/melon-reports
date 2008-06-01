using System;
using System.Collections;

namespace Melon.Reports.Objects
{
	/// <summary>
	/// Summary description for Page.
	/// </summary>
	public class Page
	{
		ArrayList elements = new ArrayList();
		int m_height ;
		int m_width ;

		public Page()
		{}

		public Page(int height, int width)
		{
			this.m_height =  height ;
			this.m_width = width ;
		}

		public void AddElement(BasicElement e,int h)
		{
			e.H = h ;
			this.elements.Add(e);
		}

		public ArrayList Elements 
		{
			get 
			{
				return this.elements ;
			}
		}
		public int Height
		{
			get 
			{
				return this.m_height ;
			}
			set
			{
				this.m_height = value ;
			}
		}
		public int Width
		{
			get
			{
				return this.m_width ;
			}
			set 
			{
				this.m_width = value ;
			}
		}

		public void PutBands(BandCollection bands,ref int h)
		{
			
			IEnumerator it = bands.Bands.GetEnumerator();
			while(it.MoveNext())
			{
				
				this.PutBand((Band)it.Current,h);// esto esta mal y no hace falta
				h -= ((Band)it.Current).Height;
			}
			
		}

		public void PutBand(Band band,int h)
		{
			IEnumerator it = band.Elements.GetEnumerator();
			while(it.MoveNext())
			{
				//pingaaaaaaaaaaaaaaaa
				// la taya es que las expresiones se convierten en texto
				// Otra cosa es que quizas no todo haya que convertirlo a texto aqui

				// parece que la operacion typeof() es mas rapida que el is
				Object current = it.Current ;
				if (current is Text)
				{
					Text t1 = (Text)it.Current ;
					Text t2 = new Text(t1.Label,t1.Align,t1.X,t1.Y);
					t2.FontSize = t1.FontSize ;
					t2.color = t1.color ;
					this.AddElement(t2,h);

				}
				else if (current is Expression)// aqui es la cosa !!
				{
					Expression e1 = (Expression)it.Current;
					//evalua la expression aqui -- cojones que lejos esta el report
					Text t2 = new Text(e1.Value.ToString(),Text.Alignment.Left,e1.X,e1.Y);
					t2.FontSize = e1.FontSize;
					t2.color = new Color("#000000"); //FIX: fix this shit
					this.AddElement(t2,h);
				}
				else if (current is Image)
				{
					Image i1 = (Image)it.Current;
					this.AddElement(i1,h);
				}
				else if (current is Rectangle)
				{
					Rectangle r1 = (Rectangle)it.Current;
					Rectangle r2 = new Rectangle();
					r2.x=r1.x;
					r2.y=r1.y;
					r2.height=r1.height;
					r2.width=r1.width;
					r2.bordercolor=r1.bordercolor;
					r2.fillcolor=r1.fillcolor;
					this.AddElement(r2,h);
				}
				else if (current is Bookmark)// solo lo estoy moviendo
				{
					this.AddElement((Bookmark)current,h);
                    //aqui debia evaluar el valor de la variable
					
				}
				
			}
		}
	}
}
