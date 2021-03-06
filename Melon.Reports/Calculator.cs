using System.Collections.Generic;
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

		public void Init()
		{
			expressionBuilder.CompileExpressions();
		}

		public void EvaluateExpressions(ICollection<Expression> expressions)
		{
			foreach (var expression in expressions)
			{
				expression.Value = expressionBuilder.EvaluateExpression(expression.GetHashCode());
			}
		}

		public void UpdateFields(ICollection<Field> fields, IDataReaderAdapter reader)
		{
			foreach (var f in fields)
			{
				expressionBuilder.SetField(f.Name, reader.GetValue(f.Name));
			}
		}

		public void SetField(string fieldName, object value)
		{
			expressionBuilder.SetField(fieldName, value);
		}

		public object EvaluateVariable(string variableName)
		{
			return expressionBuilder.EvaluateVariable(variableName);
		}

		
	}
}