using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using Melon.Reports.Objects;
using Microsoft.CSharp;

namespace Melon.Reports
{
	public class ExpressionBuilder
	{
		private readonly Hashtable FieldCollection;
		private readonly Hashtable VariableCollection;
		private readonly Hashtable ExpressionCollection;

		private object compiledExpressions;
		private Type compiledType;

		public ExpressionBuilder(Hashtable fieldCollection, Hashtable variableCollection, Hashtable expressionCollection)
		{
			FieldCollection = fieldCollection;
			ExpressionCollection = expressionCollection;
			VariableCollection = variableCollection;
		}

		public void BuildExpressions(Report report)
		{
			var csharp = new CSharpCodeProvider();

			var w = new StreamWriter(new FileStream("compiled.cs", FileMode.Create));

			var c = new CodeCommentStatement("This file is dynamically generated");
			csharp.GenerateCodeFromStatement(c, w, null);

			var namespc = new CodeNamespace("Melon.Reports");
			namespc.Imports.Add(new CodeNamespaceImport("System"));
			namespc.Imports.Add(new CodeNamespaceImport("Melon.Reports.Objects"));


			var ExpressionCalculatorClass = new CodeTypeDeclaration("ExpressionCalculator")
			                                	{
			                                		IsClass = true,
			                                		TypeAttributes = TypeAttributes.Public
			                                	};

			namespc.Types.Add(ExpressionCalculatorClass);
			//set the base class
			ExpressionCalculatorClass.BaseTypes.Add("Melon.Reports.AbstractCalculator");

			//build constructor
			var theConstructor = new CodeConstructor {Attributes = MemberAttributes.Public};
			theConstructor.Parameters.Add(new CodeParameterDeclarationExpression("Report", "report"));
			theConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("report"));
			ExpressionCalculatorClass.Members.Add(theConstructor);

			//add the fields
			var it = FieldCollection.GetEnumerator();

			while (it.MoveNext())
			{
				var f = new CodeMemberField(((Field) it.Value).Type, ((Field) it.Value).Name) {Attributes = MemberAttributes.Public};
				ExpressionCalculatorClass.Members.Add(f);
			}

			//add especial variables
			/*	GlobalRecordCount	*/
			var GlobalRecordCountField = new CodeMemberField(typeof (int), "GlobalRecordCount")
			                             	{Attributes = MemberAttributes.Public};
			ExpressionCalculatorClass.Members.Add(GlobalRecordCountField);
			/*	PageNumber	*/
			var PageNumberField = new CodeMemberField(typeof (int), "PageNumber") {Attributes = MemberAttributes.Public};
			ExpressionCalculatorClass.Members.Add(PageNumberField);

			/*
			 * public object EvaluateVariableExpression(int hashcode){
			 *  .....
			 * }
			 */

			var dynaCode = new StringBuilder("Object o = null; \n		switch(i){\n");

			it = VariableCollection.GetEnumerator();

			while (it.MoveNext())
			{
				var v = (Variable) it.Value;
				dynaCode.Append("		case " + v.GetHashCode() + ":\n" +
				                "		o = (" + v.Type + ")" + v.Expression.Trim() + ";\n" +
				                "		break;\n");

				// the property
				var p = new CodeMemberProperty();
				p.GetStatements.Add(
					new CodeSnippetStatement("return (" + v.Type + ")EvaluateVariable((Variable)variables[\"" + v.Name + "\"]);"));
				//p.SetStatements.Add(new CodeSnippetStatement("_" + ((Variable)it.Value).Name+ " = value ;"));
				p.Type = new CodeTypeReference(v.Type);
				p.Name = v.Name;
				p.Attributes = MemberAttributes.Public;
				ExpressionCalculatorClass.Members.Add(p);
			}
			dynaCode.Append("		}\nreturn o");

			var EvaluateVariableExpressionMethod = new CodeMemberMethod();
			EvaluateVariableExpressionMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof (int), "i"));
			EvaluateVariableExpressionMethod.Statements.Add(
				new CodeExpressionStatement(new CodeSnippetExpression(dynaCode.ToString())));
			EvaluateVariableExpressionMethod.ReturnType = new CodeTypeReference(typeof (object));


			ExpressionCalculatorClass.Members.Add(EvaluateVariableExpressionMethod);

			EvaluateVariableExpressionMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			;
			EvaluateVariableExpressionMethod.Name = "EvaluateVariableExpression";


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

			it = ExpressionCollection.GetEnumerator();

			while (it.MoveNext())
			{
				var ex = (Expression) it.Value;
				dynaCode.Append("		case " + ex.GetHashCode() + ":\n" +
				                "			o = (" + ((Expression) it.Value).Type + ")" + ((Expression) it.Value).Content + ";\n" +
				                "			break;\n");
			}

			dynaCode.Append("		}\n		return o");

			var EvaluateExpressionMethod = new CodeMemberMethod();
			EvaluateExpressionMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof (int), "i"));
			EvaluateExpressionMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression(dynaCode.ToString())));
			EvaluateExpressionMethod.ReturnType = new CodeTypeReference(typeof (object));


			ExpressionCalculatorClass.Members.Add(EvaluateExpressionMethod);

			EvaluateExpressionMethod.Attributes = MemberAttributes.Public;
			EvaluateExpressionMethod.Name = "EvaluateExpression";

			var cu = new CodeCompileUnit();
			cu.Namespaces.Add(namespc);

			//code generation

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
			                                                      true, BindingFlags.CreateInstance, null, new object[] {report},
			                                                      null, null);

			Console.WriteLine(compresult.CompiledAssembly.Location);

			if (o == null)
			{
				throw new ApplicationException(".NET implementation is broken.");
			}

			Type test = compresult.CompiledAssembly.GetType("Melon.Parser.ExpressionCalculator");

			this.compiledExpressions = o;
			this.compiledType = test;
		}

		public object EvaluateVariable(string variableName)
		{
			PropertyInfo p = compiledType.GetProperty(variableName);
			object o = p.GetValue(compiledExpressions, null);
			return o;
		}

		public void SetVariable(string variableName, object val)
		{
			PropertyInfo p = compiledType.GetProperty(variableName);
			p.SetValue(compiledExpressions, val, null);
		}

		public object EvaluateExpression(int i)
		{
			MethodInfo m = compiledType.GetMethod("EvaluateExpression");
			object[] arg = new object[1];
			arg[0] = i;
			return m.Invoke(compiledExpressions, arg);
		}

		public void SetField(string fieldName, object val)
		{
			Console.WriteLine("fieldName: " + fieldName);

			FieldInfo f = compiledType.GetField(fieldName);

			f.SetValue(compiledExpressions, val);
		}
	}
}