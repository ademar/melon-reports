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
			CSharpCodeProvider csharp = new CSharpCodeProvider();

			/**/
			ICodeGenerator codeGenerator = csharp.CreateGenerator();

			StreamWriter w = new StreamWriter(new FileStream("compiled.cs", FileMode.Create));

			CodeCommentStatement c = new CodeCommentStatement("This file is dynamically generated");
			codeGenerator.GenerateCodeFromStatement(c, w, null);

			CodeNamespace namespc = new CodeNamespace("Melon.Reports");
			namespc.Imports.Add(new CodeNamespaceImport("System"));
			namespc.Imports.Add(new CodeNamespaceImport("Melon.Reports.Objects"));

			//namespc.Imports.Add(new CodeNamespaceImport("System.Data.SqlTypes"));

			CodeTypeDeclaration ExpressionCalculatorClass = new CodeTypeDeclaration("ExpressionCalculator");
			ExpressionCalculatorClass.IsClass = true;
			ExpressionCalculatorClass.TypeAttributes = TypeAttributes.Public;

			namespc.Types.Add(ExpressionCalculatorClass);
			//set the base class
			ExpressionCalculatorClass.BaseTypes.Add("Melon.Reports.AbstractCalculator");

			//build constructor
			CodeConstructor theConstructor = new CodeConstructor();
			theConstructor.Attributes = MemberAttributes.Public;
			theConstructor.Parameters.Add(new CodeParameterDeclarationExpression("Report", "report"));
			theConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("report"));
			ExpressionCalculatorClass.Members.Add(theConstructor);

			//add the fields
			IDictionaryEnumerator it = FieldCollection.GetEnumerator();

			while (it.MoveNext())
			{
				CodeMemberField f = new CodeMemberField(((Field)it.Value).Type, ((Field)it.Value).Name);
				f.Attributes = MemberAttributes.Public;
				ExpressionCalculatorClass.Members.Add(f);
			}

			//add especial variables
			/*	GlobalRecordCount	*/
			CodeMemberField GlobalRecordCountField = new CodeMemberField(typeof(int), "GlobalRecordCount");
			GlobalRecordCountField.Attributes = MemberAttributes.Public;
			ExpressionCalculatorClass.Members.Add(GlobalRecordCountField);
			/*	PageNumber	*/
			CodeMemberField PageNumberField = new CodeMemberField(typeof(int), "PageNumber");
			PageNumberField.Attributes = MemberAttributes.Public;
			ExpressionCalculatorClass.Members.Add(PageNumberField);

			/*
			 * public object EvaluateVariableExpression(int hashcode){
			 *  .....
			 * }
			 */

			StringBuilder dynaCode = new StringBuilder("Object o = null; \n		switch(i){\n");

			it = VariableCollection.GetEnumerator();

			while (it.MoveNext())
			{
				Variable v = (Variable)it.Value;
				dynaCode.Append("		case " + v.GetHashCode() + ":\n" +
					"		o = (" + v.Type + ")" + v.Expression.Trim() + ";\n" +
					"		break;\n");

				// the property
				CodeMemberProperty p = new CodeMemberProperty();
				p.GetStatements.Add(new CodeSnippetStatement("return (" + v.Type + ")EvaluateVariable((Variable)variables[\"" + v.Name + "\"]);"));
				//p.SetStatements.Add(new CodeSnippetStatement("_" + ((Variable)it.Value).Name+ " = value ;"));
				p.Type = new CodeTypeReference(v.Type);
				p.Name = v.Name;
				p.Attributes = MemberAttributes.Public;
				ExpressionCalculatorClass.Members.Add(p);
			}
			dynaCode.Append("		}\nreturn o");

			CodeMemberMethod EvaluateVariableExpressionMethod = new CodeMemberMethod();
			EvaluateVariableExpressionMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "i"));
			EvaluateVariableExpressionMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression(dynaCode.ToString())));
			EvaluateVariableExpressionMethod.ReturnType = new CodeTypeReference(typeof(object));


			ExpressionCalculatorClass.Members.Add(EvaluateVariableExpressionMethod);

			EvaluateVariableExpressionMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override; ;
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
				Expression ex = (Expression)it.Value;
				dynaCode.Append("		case " + ex.GetHashCode() + ":\n" +
								"			o = (" + ((Expression)it.Value).Type + ")" + ((Expression)it.Value).Content + ";\n" +
								"			break;\n");
			}

			dynaCode.Append("		}\n		return o");

			CodeMemberMethod EvaluateExpressionMethod = new CodeMemberMethod();
			EvaluateExpressionMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "i"));
			EvaluateExpressionMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression(dynaCode.ToString())));
			EvaluateExpressionMethod.ReturnType = new CodeTypeReference(typeof(object));


			ExpressionCalculatorClass.Members.Add(EvaluateExpressionMethod);

			EvaluateExpressionMethod.Attributes = MemberAttributes.Public;
			EvaluateExpressionMethod.Name = "EvaluateExpression";

			CodeCompileUnit cu = new CodeCompileUnit();
			cu.Namespaces.Add(namespc);

			//code generation
			
			codeGenerator.GenerateCodeFromNamespace(namespc, w, null);
			w.Close();

			string thisAsembly = Assembly.GetAssembly(GetType()).Location;
			
			//compilation
			CompilerParameters compparams = new CompilerParameters(new string[] { "mscorlib.dll", thisAsembly });
			compparams.GenerateInMemory = true;
			
			string ErrorMsg = "";

			ICodeCompiler cscompiler = csharp.CreateCompiler();
			CompilerResults compresult = cscompiler.CompileAssemblyFromDom(compparams, cu);



			if (compresult == null || compresult.Errors.Count > 0)
			{
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
			true, BindingFlags.CreateInstance, null, new object[] { report }, null, null);

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
