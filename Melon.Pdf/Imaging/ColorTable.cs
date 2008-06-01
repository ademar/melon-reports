using System;
using System.Text;

namespace Melon.Pdf.Imaging
{
	public class ColorTable
	{
		readonly int m_size ;
		readonly byte[] m_table;

		int index = 0 ;

		public ColorTable(int size)
		{
			m_size = size ;
			m_table= new byte[m_size*3]; 
		}

		public void AddItem(byte b)
		{
			m_table[index++] = b ;	
		}

		public string GetRepresentation()
		{
			StringBuilder s = new StringBuilder((m_size-1).ToString());
			
			s.Append(" <");

			for(int i= 0; i<m_table.Length; i = i+3)
			{
				s.Append(Hex(m_table[i]));
				s.Append(Hex(m_table[i+1]));
				s.Append(Hex(m_table[i+2]));
		
			}
			
			s.Append(">");

			return s.ToString();

		}

		static string Hex(byte b)
		{
			string s = Convert.ToString(b,16);
			if (s.Length==1) s = "0" + s;
			return s; 
		}
	}
}
