using System.Collections;
using Melon.Reports.Objects;

namespace Melon.Reports
{
	public abstract class AbstractCalculator
	{
		protected Hashtable variables = null ;

		public AbstractCalculator(Report report)
		{
			variables = report.VariableCollection;
		}

		public object EvaluateVariable(Variable var)
		{
			object expressionValue = EvaluateVariableExpression(var.GetHashCode());
			object actualValue = var.Value;

			if (var.Formula == Variable.CALCULATION_NONE)
			{
				var.Value = expressionValue;
				return expressionValue;
			}

			//from now on tha variable has to be numeric
			//TODO : check this in some way 
			if (actualValue == null) actualValue = 0.0 ;
			double val = (double)actualValue ;
			if (var.Formula == Variable.CALCULATION_COUNT)//what's this for ??
			{				
				val ++ ;
				var.Value = val ;
			}else
				if (var.Formula == Variable.CALCULATION_SUM)
				{
					val = var.sum + (double)expressionValue ;
					var.sum = val ;
					var.Value = val ;
				}else if (var.Formula == Variable.CALCULATION_AVERAGE)
                {
				}else if (var.Formula == Variable.CALCULATION_HIGHEST)
                {
                }else if (var.Formula == Variable.CALCULATION_LOWEST)
                {
                }
			return val;
		}

		public abstract object EvaluateVariableExpression(int hashcode);
		
	}
}
