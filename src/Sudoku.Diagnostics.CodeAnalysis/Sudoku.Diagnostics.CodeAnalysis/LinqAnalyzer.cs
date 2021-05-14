﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that analyzes LINQ nodes.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class LinqAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the method name <c>Take</c>.
		/// </summary>
		private const string TakeMethodName = "Take";

		/// <summary>
		/// Indicates the method name <c>Count</c>.
		/// </summary>
		private const string CountMethodName = "Count";

		/// <summary>
		/// Indicates the full type name of <see cref="Enumerable"/>.
		/// </summary>
		/// <seealso cref="Enumerable"/>
		private const string EnumerableClassFullName = "System.Linq.Enumerable";

		/// <summary>
		/// Indicates the full type name of <see cref="IEnumerable{T}"/>.
		/// </summary>
		/// <seealso cref="IEnumerable{T}"/>
		private const string IEnumerableFullName = "System.Collections.Generic.IEnumerable`1";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				static context => CheckSudoku019(context),
				new[] { SyntaxKind.ExpressionStatement }
			);
		}


		private static void CheckSudoku019(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, node) = context;
			if (
				node is not BinaryExpressionSyntax
				{
					RawKind: (int)SyntaxKind.GreaterThanOrEqualExpression,
					Left: InvocationExpressionSyntax
					{
						Expression: MemberAccessExpressionSyntax
						{
							RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
							Expression:
								var potentialTakeMethodInvocationNode
								and not InvocationExpressionSyntax
								{
									Expression: MemberAccessExpressionSyntax
									{
										RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
										Expression: IdentifierNameSyntax
										{
											Identifier: { ValueText: TakeMethodName }
										}
									}
								},
							Name: IdentifierNameSyntax { Identifier: { ValueText: CountMethodName } },
						},
						ArgumentList: { Arguments: { Count: 0 } }
					} invocationNode,
					Right: var rightNode
				}
			)
			{
				return;
			}

			if (
				semanticModel.GetOperation(invocationNode) is not IInvocationOperation
				{
					Kind: OperationKind.Invocation,
					TargetMethod:
					{
						ContainingType: var containingTypeSymbol,
						Parameters: { Length: 1 } parameterSymbols,
						ReturnType: var returnTypeSymbol,
						IsExtensionMethod: true,
						IsGenericMethod: true
					}
				} operation
			)
			{
				return;
			}

			if (
				semanticModel.GetOperation(potentialTakeMethodInvocationNode) is IInvocationOperation
				{
					Kind: OperationKind.Invocation,
					TargetMethod:
					{
						ContainingType: var containingTypeSymbolTakeMethod,
						Parameters: { Length: 2 } parameterSymbolsTakeMethod,
						ReturnType: var returnTypeSymbolTakeMethod,
						IsExtensionMethod: true,
						IsGenericMethod: true
					}
				} possibleTakeMethodInvocationOperation
				&& SymbolEqualityComparer.Default.Equals(
					containingTypeSymbolTakeMethod,
					compilation.GetTypeByMetadataName(EnumerableClassFullName)
				)
				&& SymbolEqualityComparer.Default.Equals(
					returnTypeSymbolTakeMethod,
					compilation
					.GetTypeByMetadataName(IEnumerableFullName)!
					.WithTypeArguments(compilation, SpecialType.System_Int32)
				)
				&& SymbolEqualityComparer.Default.Equals(
					parameterSymbolsTakeMethod[0].Type,
					compilation
					.GetTypeByMetadataName(IEnumerableFullName)!
					.WithTypeArguments(compilation, SpecialType.System_Int32)
				)
				&& SymbolEqualityComparer.Default.Equals(
					parameterSymbolsTakeMethod[1].Type,
					compilation.GetSpecialType(SpecialType.System_Int32)
				)
			)
			{
				return;
			}

			if (
				!SymbolEqualityComparer.Default.Equals(
					containingTypeSymbol,
					compilation.GetTypeByMetadataName(EnumerableClassFullName)
				)
				|| !SymbolEqualityComparer.Default.Equals(
					returnTypeSymbol,
					compilation.GetSpecialType(SpecialType.System_Int32)
				)
				|| !SymbolEqualityComparer.Default.Equals(
					parameterSymbols[0].Type,
					compilation
					.GetTypeByMetadataName(IEnumerableFullName)!
					.WithTypeArguments(compilation, SpecialType.System_Int32)
				)
			)
			{
				return;
			}

			// No calling conversion.
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: new(
						id: DiagnosticIds.Sudoku019,
						title: Titles.Sudoku016,
						messageFormat: Messages.Sudoku019,
						category: Categories.Performance,
						defaultSeverity: DiagnosticSeverity.Warning,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.Sudoku019
					),
					location: node.GetLocation(),
					messageArgs: new[] { rightNode.ToString() }
				)
			);
		}
	}
}