﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for the repeated or unncessary
	/// discards <c><see langword="_"/></c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class DiscardAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the discard symbol.
		/// </summary>
		private const string Discard = "_";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.SimpleAssignmentExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			// Check those cases:
			// 1) _ = 10;
			// 2) _ = a;
			if (
				originalNode is not AssignmentExpressionSyntax
				{
					Left: IdentifierNameSyntax { Identifier: { ValueText: Discard } },
					Right: var rightExpr and (LiteralExpressionSyntax or IdentifierNameSyntax)
				} node
			)
			{
				return;
			}

			// Check whether the right-side expression is a member access expression,
			// and the expression isn't a method, property, indexer, event or user-defined operator.
			if (
				semanticModel.GetOperation(rightExpr) is
					IMethodReferenceOperation // User-defined method.
					or IPropertyReferenceOperation // User-defined property or indexer.
					or IEventReferenceOperation // User-defined event.
					or IConversionOperation { Conversion: { IsUserDefined: false } } // User-defined cast operator.
					or IUnaryOperation { OperatorMethod: not null } // User-defined unary operator.
					or IBinaryOperation { OperatorMethod: not null } // User-defined binary operator.
			)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS9003,
					location: originalNode.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}