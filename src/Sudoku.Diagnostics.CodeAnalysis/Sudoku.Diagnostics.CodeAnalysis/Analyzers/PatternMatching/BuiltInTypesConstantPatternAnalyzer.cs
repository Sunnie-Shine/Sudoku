﻿using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for the constant pattern matching of built-in types.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class BuiltInTypesConstantPatternAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.IsPatternExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			if (
				originalNode is not IsPatternExpressionSyntax
				{
					Expression: var expressionToCheck,
					IsKeyword: var token,
					Pattern: var pattern
				}
			)
			{
				return;
			}

			if (semanticModel.GetOperation(expressionToCheck) is not { Type: var typeSymbol })
			{
				return;
			}

			switch (pattern)
			{
				case ConstantPatternSyntax { Expression: var constantExpression }:
				{
					if (semanticModel.GetOperation(constantExpression) is not { Type: var constantTypeSymbol })
					{
						return;
					}

					if (!SymbolEqualityComparer.Default.Equals(typeSymbol, constantTypeSymbol))
					{
						return;
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0604,
							location: token.GetLocation(),
							messageArgs: new[]
							{
								expressionToCheck.ToString(),
								"==",
								constantExpression.ToString()
							}
						)
					);

					break;
				}
				case UnaryPatternSyntax
				{
					RawKind: (int)SyntaxKind.NotPattern,
					Pattern: ConstantPatternSyntax { Expression: var constantExpression }
				}:
				{
					if (semanticModel.GetOperation(constantExpression) is not { Type: var constantTypeSymbol })
					{
						return;
					}

					if (!SymbolEqualityComparer.Default.Equals(typeSymbol, constantTypeSymbol))
					{
						return;
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0604,
							location: token.GetLocation(),
							messageArgs: new[]
							{
								expressionToCheck.ToString(),
								"!=",
								constantExpression.ToString()
							}
						)
					);

					break;
				}
			}


		}
	}
}
