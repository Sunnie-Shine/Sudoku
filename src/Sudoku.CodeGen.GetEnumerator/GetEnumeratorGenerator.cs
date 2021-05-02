﻿#pragma warning disable IDE0057

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using GenericsOptions = Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions;
using GlobalNamespaceStyle = Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle;
using MiscellaneousOptions = Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions;
using TypeQualificationStyle = Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Sudoku.CodeGen.GetEnumerator
{
	/// <summary>
	/// Indicates a source generator that generates the code for the method <c>GetEnumerator</c>.
	/// </summary>
	[Generator]
	public sealed partial class GetEnumeratorGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates the type format, and the property type format.
		/// </summary>
		private static readonly SymbolDisplayFormat TypeFormat = new(
			GlobalNamespaceStyle.OmittedAsContaining,
			TypeQualificationStyle.NameAndContainingTypesAndNamespaces,
			GenericsOptions.IncludeTypeParameters | GenericsOptions.IncludeTypeConstraints,
			miscellaneousOptions:
				MiscellaneousOptions.UseSpecialTypes
				| MiscellaneousOptions.EscapeKeywordIdentifiers
				| MiscellaneousOptions.IncludeNullableReferenceTypeModifier
		);


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			{
				return;
			}

			var compilation = context.Compilation;
			var nameDic = new Dictionary<string, int>();
			foreach (var (symbol, attribute) in g(context, receiver))
			{
				_ = nameDic.TryGetValue(symbol.Name, out int i);
				string name = i == 0 ? symbol.Name : $"{symbol.Name}{(i + 1).ToString()}";
				nameDic[symbol.Name] = i + 1;

				var syntaxTree = attribute.SyntaxTree;
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				if (getGetEnumeratorCode(symbol, attribute, semanticModel) is { } c)
				{
					context.AddSource($"{name}.GetEnumerator.g.cs", c);
				}
			}

			static IEnumerable<(INamedTypeSymbol, AttributeSyntax)> g(
				in GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return
					from candidateInfo in receiver.Candidates
					let model = compilation.GetSemanticModel(candidateInfo.Node.SyntaxTree)
					select (
						(INamedTypeSymbol)model.GetDeclaredSymbol(candidateInfo.Node)!,
						candidateInfo.Attribute
					);
			}

			static ITypeSymbol? getReturnType(
				AttributeArgumentListSyntax attributeArgumentList, SemanticModel semanticModel)
			{
				foreach (var attributeArg in attributeArgumentList.Arguments)
				{
					if (
						attributeArg is
						{
							NameEquals:
							{
								Name:
								{
									Identifier: { ValueText: nameof(AutoGetEnumeratorAttribute.ReturnType) }
								}
							},
							Expression: var expr
						}
						&& semanticModel.GetOperation(expr) is ITypeOfOperation { TypeOperand: var operand }
					)
					{
						return operand;
					}
				}

				return null;
			}

			static string? getGetEnumeratorCode(
				INamedTypeSymbol symbol, AttributeSyntax attribute, SemanticModel semanticModel)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				string typeName = symbol.Name;
				string fullTypeName = symbol.ToDisplayString(TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParameterList = i == -1 ? string.Empty : fullTypeName.Substring(i);
				string readonlyKeyword = symbol.TypeKind == TypeKind.Struct ? "readonly " : string.Empty;
				string? typeArguments = symbol
					.AllInterfaces
					.FirstOrDefault(static i => i.Name.StartsWith("IEnumerable"))?
					.TypeArguments[0]
					.ToDisplayString(TypeFormat);
				var typeSymbol = getReturnType(attribute.ArgumentList, semanticModel);
				string returnType = typeSymbol is null
					? $"System.Collections.Generic.IEnumerator<{typeArguments}>"
					: typeSymbol.ToDisplayString(TypeFormat);

				if (attribute.ArgumentList is null || typeSymbol is null && typeArguments is null)
				{
					return null;
				}

				string typeKind = symbol switch
				{
					{ TypeKind: TypeKind.Class } => "class",
					{ TypeKind: TypeKind.Struct } => "struct",
					{ IsRecord: true } => "record"
				};
				string memberNameStr = attribute.ArgumentList.Arguments[0].Expression.ToString();
				string memberName = memberNameStr.Substring(7, memberNameStr.Length - 8);
				string exprStr = attribute.ArgumentList.Arguments[1].Expression.ToString();
				string memberConversion = exprStr.Substring(1, exprStr.Length - 2).Replace("@", memberName);
				string interfaceExplicitlyImpl = symbol.IsRefLikeType ? string.Empty : $@"

		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		{readonlyKeyword}System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();";

				return $@"#pragma warning disable 1591

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind} {typeName}{genericParameterList}
	{{
		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public {readonlyKeyword}{returnType} GetEnumerator() => {memberConversion};{interfaceExplicitlyImpl}
	}}
}}";
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
	}
}