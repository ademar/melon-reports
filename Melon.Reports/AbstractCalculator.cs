using System.Collections.Generic;
using Melon.Reports.Objects;

namespace Melon.Reports
{
	public abstract class AbstractCalculator
	{
		protected IDictionary<string, Variable> variables;

		protected AbstractCalculator(IDictionary<string, Variable> variables)
		{
			this.variables = variables;
		}

		public object EvaluateVariable(Variable var)
		{
			var expressionValue = EvaluateVariableExpression(var.GetHashCode());
			var actualValue = var.Value;

			if (var.Formula == Variable.CALCULATION_NONE)
			{
				var.Value = expressionValue;
				return expressionValue;
			}

			if (actualValue == null) actualValue = 0.0;

			var val = (double) actualValue;
			if (var.Formula == Variable.CALCULATION_COUNT)
			{
				val ++;
				var.Value = val;
			}
			else if (var.Formula == Variable.CALCULATION_SUM)
			{
				val = var.sum + (double) expressionValue;
				var.sum = val;
				var.Value = val;
			}
			else if (var.Formula == Variable.CALCULATION_AVERAGE)
			{
			}
			else if (var.Formula == Variable.CALCULATION_HIGHEST)
			{
			}
			else if (var.Formula == Variable.CALCULATION_LOWEST)
			{
			}
			return val;
		}

		public abstract object EvaluateVariableExpression(int hashcode);
	}
}