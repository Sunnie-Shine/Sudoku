﻿using System.Text.Markdown;
using Microsoft.CodeAnalysis;
using Sudoku.XmlDocs.Extensions;

namespace Sudoku.XmlDocs.SyntaxInfo
{
	/// <summary>
	/// Defines a field syntax information.
	/// </summary>
	/// <param name="SyntaxNode">The current syntax node.</param>
	/// <param name="IdentifierName">The identifier name of this member.</param>
	/// <param name="CustomAccessibility">The accessibility.</param>
	/// <param name="CustomModifier">The custom modifier.</param>
	/// <param name="Summary">The documentation comment for the section "summary".</param>
	/// <param name="Remarks">The documentation comment for the section "remarks".</param>
	/// <param name="Example">The documentation comment for the section "example".</param>
	/// <param name="ExceptionList">The <c>exception</c> list.</param>
	/// <param name="SeeAlsoList">The <c>seealso</c> list.</param>
	public sealed record FieldSyntaxInfo(
		SyntaxNode SyntaxNode, string IdentifierName,
		CustomAccessibility CustomAccessibility, CustomModifier CustomModifier,
		string? Summary, string? Remarks, string? Example,
		(string Exception, string? Description)[]? ExceptionList, string[]? SeeAlsoList
	) : MemberSyntaxInfo(
		SyntaxNode, IdentifierName, CustomAccessibility, CustomModifier,
		Summary, Remarks, Example, ExceptionList, SeeAlsoList
	)
	{
		/// <inheritdoc/>
		public override string ToString() => Document.Create()
			.AppendTitle(MemberKind.Field, IdentifierName ?? string.Empty)
			.AppendSummary(Summary)
			.AppendRemarks(Remarks)
			.AppendException(ExceptionList)
			.ToString();
	}
}