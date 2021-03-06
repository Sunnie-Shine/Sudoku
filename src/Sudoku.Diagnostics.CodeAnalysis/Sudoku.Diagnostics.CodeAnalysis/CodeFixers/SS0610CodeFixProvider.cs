﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Sudoku.CodeGenerating;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SS0610")]
	public sealed partial class SS0610CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0610));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var (_, isPatternExprSpan) = diagnostic.AdditionalLocations[1];
			var isPatternExpr = root.FindNode(isPatternExprSpan, getInnermostNodeForTie: true);
			bool hasDesignation = diagnostic.AdditionalLocations.Count switch { 3 => true, 2 => false };
			bool isNullable = bool.Parse(diagnostic.Properties["IsNullable"]!);
			var (_, exprSpan) = diagnostic.AdditionalLocations[0];
			var expr = (ExpressionSyntax)root.FindNode(exprSpan, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0610,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							isPatternExpr,
							(isNullable, hasDesignation) switch
							{
								(_, true) => SyntaxFactoryEx.IsEmptyPropertyPatternExpression(
									expr,
									getVariableName(diagnostic, root).Name
								),
								(true, false) => SyntaxFactory.IsPatternExpression(
									expr,
									SyntaxFactory.UnaryPattern(
										SyntaxFactory.ConstantPattern(
											SyntaxFactory.LiteralExpression(
												SyntaxKind.NullLiteralExpression
											)
										)
									)
								),
								_ => SyntaxFactory.IsPatternExpression(
									expr,
									SyntaxFactory.VarPattern(
										SyntaxFactory.DiscardDesignation()
									)
								)
							}
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SS0610)
				),
				diagnostic
			);

			static (string Name, SingleVariableDesignationSyntax Node) getVariableName(
				Diagnostic diagnostic, SyntaxNode root)
			{
				var (_, designationSpan) = diagnostic.AdditionalLocations[2];
				var node = (SingleVariableDesignationSyntax)root.FindNode(
					designationSpan, getInnermostNodeForTie: true
				);

				return (node.Identifier.ValueText, node);
			}
		}
	}
}
