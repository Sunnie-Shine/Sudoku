﻿using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0101", "SS0102F")]
	public sealed partial class InterpolatedStringAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				static context =>
				{
					CheckSS0101(context);
					CheckSS0102(context);
				},
				new[] { SyntaxKind.Interpolation, SyntaxKind.InterpolatedStringExpression }
			);
		}


		private static void CheckSS0101(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, orginalNode) = context;
			if (orginalNode is not InterpolatedStringExpressionSyntax node)
			{
				return;
			}

			/*length-pattern*/
			if (
				node.DescendantNodes().OfType<InterpolationSyntax>().ToArray() is not
				{
					Length: not 0
				} interpolationParts
			)
			{
				return;
			}

			foreach (var interpolation in interpolationParts)
			{
				if (
					semanticModel.GetOperation(interpolation) is not IInterpolationOperation
					{
						Kind: OperationKind.Interpolation,
						Expression: { Type: { IsValueType: true } }
					}
				)
				{
					return;
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0101,
						location: interpolation.Expression.GetLocation(),
						messageArgs: null
					)
				);
			}
		}

		private static void CheckSS0102(SyntaxNodeAnalysisContext context)
		{
			if (context.Node is not InterpolatedStringExpressionSyntax node)
			{
				return;
			}

			if (node.DescendantNodes().OfType<InterpolationSyntax>().Any())
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0102,
					location: node.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
