﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Provides a generator that generates the deconstruction methods that are extension methods.
	/// </summary>
	[Generator]
	public sealed partial class ExtensionDeconstructMethodGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			var compilation = context.Compilation;
			var nameDic = new Dictionary<string, int>();
			foreach (var groupResult in
				from attribute in receiver.Attributes
				select attribute.ArgumentList into argList
				where argList is { Arguments: { Count: >= 2 } }
				let firstArg = argList.Arguments[0].Expression as TypeOfExpressionSyntax
				where firstArg is not null
				let semanticModel = compilation.GetSemanticModel(firstArg.SyntaxTree)
				let operation = semanticModel.GetOperation(firstArg) as ITypeOfOperation
				where operation is not null
				let type = operation.TypeOperand
				group (argList, type) by type.ToDisplayString(FormatOptions.TypeFormat))
			{
				string key = groupResult.Key;
				foreach (var (p, typeSymbol) in groupResult)
				{
					_ = nameDic.TryGetValue(typeSymbol.Name, out int i);
					string name = i == 0 ? typeSymbol.Name : $"{typeSymbol.Name}{i + 1}";
					nameDic[typeSymbol.Name] = i + 1;
					context.AddSource($"{name}Ex", "DeconstructionMethods", getDeconstructionCode(typeSymbol, p));
				}
			}


			static string getDeconstructionCode(ITypeSymbol symbol, AttributeArgumentListSyntax argList)
			{
				string? tempNamespace = argList.Arguments.FirstOrDefault(
					/*extended-property-pattern*/
					static arg => arg is { NameEquals: { Name: { Identifier: { ValueText: "Namespace" } } } }
				)?.Expression.ToString();
				string namespaceName = tempNamespace?.Substring(1, tempNamespace.Length - 2)
					?? $"{symbol.ContainingNamespace}.Extensions";
				string typeName = symbol.Name;
				symbol.DeconstructInfo(
					out string fullTypeName, out _, out string genericParametersList,
					out string genericParameterListWithoutConstraint, out string fullTypeNameWithoutConstraint,
					out string constraint, out _
				);
				var members =
					from arg in argList.Arguments
					let expr = arg.Expression as InvocationExpressionSyntax
					where expr.IsNameOfExpression() && expr!.ArgumentList.Arguments.Count == 1
					let p = expr.ArgumentList.Arguments[0].ToString()
					let lastDotTokenPos = p.LastIndexOf('.')
					where lastDotTokenPos != -1
					select p.Substring(lastDotTokenPos + 1);
				string inModifier = symbol.TypeKind == TypeKind.Struct ? "in " : string.Empty;
				string parameterList = string.Join(
					", ",
					from member in members
					let memberFound = symbol.GetAllMembers().FirstOrDefault(m => m.Name == member)
					where memberFound is not null
					let memberType = memberFound.GetMemberType()
					where memberType is not null
					select $@"out {memberType} {member.ToCamelCase()}"
				);
				string assignments = string.Join(
					"\r\n\t\t\t",
					from member in members select $"{member.ToCamelCase()} = @this.{member};"
				);
				string deconstructMethods = $@"/// <summary>
		/// Deconstruct the instance to multiple values, which allows you use the value tuple syntax
		/// to get the properties from an instance like:
		/// <code>
		/// var (variable1, variable2, variable3) = instance;
		/// </code>
		/// or like
		/// <code>
		/// (int variable1, double variable2, string? variable3) = instance;
		/// </code>
		/// </summary>
		/// <remarks>
		/// <para>
		/// The method should be declared manually when the type is a normal <see langword=""struct""/>
		/// or <see langword=""class""/>. If the method is in a <see langword=""record""/>
		/// (or a <see langword=""record struct""/>), the deconstruct method will be generated
		/// by the compiler automatically and returns <b>all properties</b>
		/// to those <see langword=""out""/> parameters.
		/// </para>
		/// <para>
		/// Please note: If the deconstruct method is automatically generated by the compiler,
		/// you can't create a deconstruct method manually with the same number of the parameters
		/// than that auto method; otherwise, the method can't be called normally.
		/// </para>
		/// <para>
		/// In addition, the deconstruct methods can take <b>more than</b> 8 <see langword=""out""/> parameters,
		/// although a normal <see cref=""ValueTuple""/> can only contain at most 8 parameters.
		/// </para>
		/// </remarks>
		/// <seealso cref=""ValueTuple""/>
		[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct{genericParameterListWithoutConstraint}(this {inModifier}{fullTypeNameWithoutConstraint} @this, {parameterList}){constraint}
		{{
			{assignments}
		}}";

				return $@"#pragma warning disable 1574, 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	public static partial class {typeName}Ex
	{{
		{deconstructMethods}
	}}
}}";
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
	}
}
