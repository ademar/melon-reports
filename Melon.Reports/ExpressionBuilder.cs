using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Melon.Reports.Objects;
using Microsoft.CSharp;

namespace Melon.Reports
{
	public class ExpressionBuilder
	{
		
		private object compiledExpressions;
		private Type compiledType;

		private readonly IList<Field> fields;
        private readonly IList<Parameter> parameters;
        private readonly IDictionary<string,Variable> variables;
		private readonly IList<Expression> expressions;

		public ExpressionBuilder(IList<Field> fields, IDictionary<string, Variable> variables, IList<Expression> expressions, IList<Parameter> parameters)
		{
			this.fields = fields;
			this.expressions = expressions;
			this.variables = variables;
            this.parameters = parameters;

        }

		public void CompileExpressions()
		{
			var csharp = new CSharpCodeProvider();

			var w = new StreamWriter(new FileStream("compiled.cs", FileMode.Create));

			var c = new CodeCommentStatement("This file is dynamically generated");
			csharp.GenerateCodeFromStatement(c, w, null);

			var namespc = new CodeNamespace("Melon.Reports");
			namespc.Imports.Add(new CodeNamespaceImport("System"));
			namespc.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
			namespc.Imports.Add(new CodeNamespaceImport("Melon.Reports.Objects"));


			var ExpressionCalculatorClass = new CodeTypeDeclaration("ExpressionCalculator")
			                                	{
			                                		IsClass = true,
			                                		TypeAttributes = TypeAttributes.Public
			                                	};

			namespc.Types.Add(ExpressionCalculatorClass);
			
			ExpressionCalculatorClass.BaseTypes.Add("Melon.Reports.AbstractCalculator");
            ExpressionCalculatorClass.Members.Add(createConstructor());

			foreach (var field in fields)
			{
				var f = new CodeMemberField(field.Type, field.Name) { Attributes = MemberAttributes.Public };
				ExpressionCalculatorClass.Members.Add(f);
			}

            foreach (var field in parameters)
            {
                var f = new CodeMemberField(field.Type, field.Name) { Attributes = MemberAttributes.Public };
                ExpressionCalculatorClass.Members.Add(f);
            }

            //especial variables

            ExpressionCalculatorClass.Members.Add(new CodeMemberField(typeof (int), "GlobalRecordCount") {Attributes = MemberAttributes.Public});
			ExpressionCalculatorClass.Members.Add(new CodeMemberField(typeof (int), "PageNumber") {Attributes = MemberAttributes.Public});

			/*
			 * public object EvaluateVariableExpression(int hashcode){
			 *  .....
			 * }
			 */

			var dynaCode = new StringBuilder("Object o = null; \n		switch(i){\n");

			foreach (var variable in variables.Values)
			{
				dynaCode.Append(Case(variable));
                ExpressionCalculatorClass.Members.Add(createProperty(variable));
			}
			dynaCode.Append("		}\nreturn o");

			ExpressionCalculatorClass.Members.Add(createMethod(dynaCode, "EvaluateVariableExpression"));

			
			/* add expressions using the hash trick
			 * 
			 * public virtual object EvaluateExpression(int i) {
			 *	Object o = null; 
			 *	switch(i){
			 *		case 13:
			 *			o = (System.String)System.DateTime.Today.ToShortDateString();
			 *			break;
			 *		case ........
			 *			..
			 *	}
			 * }
			 */
            
			dynaCode = new StringBuilder("Object o = null; \n		switch(i){\n");
            
			foreach (var ex in expressions)
			{
				dynaCode.Append("		case " + ex.GetHashCode() + ":\n" +
								"			o = " + ex.Content + ";\n" +
								"			break;\n");
			}
			dynaCode.Append("		}\n		return o");

			
			ExpressionCalculatorClass.Members.Add(createMethod2(dynaCode, "EvaluateExpression"));

			var cu = new CodeCompileUnit();
			cu.Namespaces.Add(namespc);

			csharp.GenerateCodeFromNamespace(namespc, w, null);
			w.Close();

			var thisAsembly = Assembly.GetAssembly(GetType()).Location;

			//compilation
			var compparams = new CompilerParameters(new[] {"mscorlib.dll", thisAsembly}) {GenerateInMemory = true};

			var ErrorMsg = "";

			var cscompiler = csharp.CreateCompiler();
			var compresult = cscompiler.CompileAssemblyFromDom(compparams, cu);


			if (compresult == null || compresult.Errors.Count > 0)
			{
				if (compresult != null)
					if (compresult.Errors.Count > 0)
					{
						foreach (CompilerError CompErr in compresult.Errors)
						{
							ErrorMsg = ErrorMsg +
							           "Line number " + CompErr.Line +
							           ", Error Number: " + CompErr.ErrorNumber +
							           ", '" + CompErr.ErrorText + ";" +
							           Environment.NewLine + Environment.NewLine;
						}
					}

				throw new Exception(ErrorMsg);
			}

			object o = compresult.CompiledAssembly.CreateInstance("Melon.Reports.ExpressionCalculator",
			                                                      true, BindingFlags.CreateInstance, null, new object[] {variables},
			                                                      null, null);

			Console.WriteLine(compresult.CompiledAssembly.Location);

			if (o == null)
			{
				throw new ApplicationException(".NET implementation is broken.");
			}

			var test = compresult.CompiledAssembly.GetType("Melon.Reports.ExpressionCalculator");

			compiledExpressions = o;
			compiledType = test;
		}

		private static CodeConstructor createConstructor()
		{
			var theConstructor = new CodeConstructor {Attributes = MemberAttributes.Public};
			theConstructor.Parameters.Add(new CodeParameterDeclarationExpression("IDictionary<string,Variable>", "variables"));
			theConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("variables"));
			return theConstructor;
		}

		private static CodeMemberMethod createMethod2(StringBuilder dynaCode, string methodName)
		{
			var EvaluateExpressionMethod = new CodeMemberMethod();
			EvaluateExpressionMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof (int), "i"));
			EvaluateExpressionMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression(dynaCode.ToString())));
			EvaluateExpressionMethod.ReturnType = new CodeTypeReference(typeof (object));
			EvaluateExpressionMethod.Attributes = MemberAttributes.Public;
			EvaluateExpressionMethod.Name = methodName;
			return EvaluateExpressionMethod;
		}

		private static CodeMemberMethod createMethod(StringBuilder dynaCode, string methodName)
		{
			var EvaluateVariableExpressionMethod = new CodeMemberMethod();

			EvaluateVariableExpressionMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof (int), "i"));
			EvaluateVariableExpressionMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression(dynaCode.ToString())));
			EvaluateVariableExpressionMethod.ReturnType = new CodeTypeReference(typeof (object));
			EvaluateVariableExpressionMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			EvaluateVariableExpressionMethod.Name = methodName;
			return EvaluateVariableExpressionMethod;
		}

		private static CodeMemberProperty createProperty(Variable variable)
		{
			var p = new CodeMemberProperty();
			p.GetStatements.Add(new CodeSnippetStatement("return (" + variable.Type + ")EvaluateVariable(variables[\"" + variable.Name + "\"]);"));
			//p.SetStatements.Add(new CodeSnippetStatement("_" + ((Variable)it.Value).Name+ " = value ;"));
			p.Type = new CodeTypeReference(variable.Type);
			p.Name = variable.Name;
			p.Attributes = MemberAttributes.Public;
			return p;
		}

		private static string Case(Variable variable)
		{
			return "		case " + variable.GetHashCode() + ":\n" +
			       "		o = " + variable.Expression.Trim() + ";\n" +
			       "		break;\n";
		}

		public object EvaluateVariable(string variableName)
		{
			var p = compiledType.GetProperty(variableName);
			var o = p.GetValue(compiledExpressions, null);
			return o;
		}

		public void SetVariable(string variableName, object val)
		{
			var p = compiledType.GetProperty(variableName);
			p.SetValue(compiledExpressions, val, null);
		}

		public object EvaluateExpression(int i)
		{
			var m = compiledType.GetMethod("EvaluateExpression");
			var arg = new object[1];
			arg[0] = i;
			return m.Invoke(compiledExpressions, arg);
		}

		public void SetField(string fieldName, object val)
		{
			var f = compiledType.GetField(fieldName);

			f.SetValue(compiledExpressions, val);
		}
	}
}