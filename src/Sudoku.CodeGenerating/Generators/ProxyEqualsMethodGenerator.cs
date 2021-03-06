﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Indicates a generator that generates the code about the equality method.
	/// </summary>
	[Generator]
	public sealed partial class ProxyEqualsMethodGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			var processedList = new List<INamedTypeSymbol>();
			var compilation = context.Compilation;
			var attributeSymbol = compilation.GetTypeByMetadataName<ProxyEqualityAttribute>();
			foreach (var type in
				from candidate in receiver.Candidates
				let model = compilation.GetSemanticModel(candidate.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(candidate)! into type
				from member in type.GetMembers().OfType<IMethodSymbol>()
				where (
					from a in member.GetAttributes()
					where SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)
					select a
				).Any()
				let boolSymbol = compilation.GetSpecialType(SpecialType.System_Boolean)
				let returnTypeSymbol = member.ReturnType
				where SymbolEqualityComparer.Default.Equals(returnTypeSymbol, boolSymbol)
				let parameters = member.Parameters
				where parameters.Length == 2 && parameters.All(p => SymbolEqualityComparer.Default.Equals(p.Type, type))
				select type)
			{
				if (processedList.Contains(type, SymbolEqualityComparer.Default))
				{
					continue;
				}

				var methods = (
					from member in type.GetMembers().OfType<IMethodSymbol>()
					from attribute in member.GetAttributes()
					where SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)
					select member
				).First();

				/*slice-pattern*/
				if (
					type.IsReferenceType
					&& !methods.Parameters.NullableMatches(NullableAnnotation.Annotated, NullableAnnotation.Annotated)
				)
				{
					continue;
				}

				type.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out string genericParametersListWithoutConstraint, out string typeKind,
					out string readonlyKeyword, out _
				);
				string methodName = methods.Name;
				string inModifier = type.MemberShouldAppendIn() ? "in " : string.Empty;
				string nullableMark = type.TypeKind == TypeKind.Class || type.IsRecord ? "?" : string.Empty;
				string objectEqualityMethod = type.IsRefLikeType
					? "// This type is a ref struct, so 'bool Equals(object?) is useless."
					: $@"[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override {readonlyKeyword}bool Equals(object? obj) => obj is {type.Name}{genericParametersListWithoutConstraint} comparer && {methodName}(this, comparer);";

				context.AddSource(
					type.ToFileName(),
					"ProxyEquality",
					$@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind}{type.Name}{genericParametersList}
	{{
		{objectEqualityMethod}

		[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals({inModifier}{type.Name}{genericParametersListWithoutConstraint}{nullableMark} other) => {methodName}(this, other);


		[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==({inModifier}{type.Name}{genericParametersListWithoutConstraint} left, {inModifier}{type.Name}{genericParametersListWithoutConstraint} right) => {methodName}(left, right);

		[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=({inModifier}{type.Name}{genericParametersListWithoutConstraint} left, {inModifier}{type.Name}{genericParametersListWithoutConstraint} right) => !(left == right);
	}}
}}");

				processedList.Add(type);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
	}
}
