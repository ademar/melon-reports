using System;
using System.Collections;
using System.Data;
using Melon.Reports.Objects;

namespace Melon.Reports
{
	public class Calculator
	{
		private readonly ExpressionBuilder expressionBuilder;
		
		public Calculator(ExpressionBuilder expressionBuilder)
		{
			this.expressionBuilder = expressionBuilder;
		}

		public void EvaluateBand(Band band,Hashtable fields,Report report)
		{
			var it = band.Elements.GetEnumerator();
			while(it.MoveNext())
			{
				var o = it.Current;
				var type = o.GetType();
				if(type == typeof(Expression))
				{
					var e = (Expression)o ;
					e.Value = expressionBuilder.EvaluateExpression(e.GetHashCode());

					//Field f = (Field)fields[e.fieldname];
					//e.Value = (string)f.Value ;
				}
			}

		}

		public void EvaluateExpressions(Report report)
		{
			var it = report.ExpressionCollection.GetEnumerator();
			while(it.MoveNext())
			{
				var e =  (Expression)it.Value ;
				e.Value = expressionBuilder.EvaluateExpression(e.GetHashCode());

			}

		}

		public void UpdateFields(Hashtable fields,IDataReader reader,Report report)
		{
			IDictionaryEnumerator  it = fields.GetEnumerator();
			while(it.MoveNext())
			{
				Field f = (Field)it.Value ;
				
				
				Console.WriteLine(f.Name);
				expressionBuilder.SetField(f.Name, reader[f.Name]);
				

			}
		}
	}
}
