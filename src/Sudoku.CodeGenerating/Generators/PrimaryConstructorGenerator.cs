﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Indicates a generator that generates primary constructors for <see langword="class"/>es
	/// when they're marked <see cref="AutoGeneratePrimaryConstructorAttribute"/>.
	/// </summary>
	/// <remarks>
	/// This generator can <b>only</b> support non-nested <see langword="class"/>es.
	/// </remarks>
	/// <seealso cref="AutoGeneratePrimaryConstructorAttribute"/>
	[Generator]
	public sealed partial class PrimaryConstructorGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			var compilation = context.Compilation;
			var attributeSymbol = compilation.GetTypeByMetadataName<AutoGeneratePrimaryConstructorAttribute>();
			var addAttributeSymbol = compilation.GetTypeByMetadataName<PrimaryConstructorIncludedMemberAttribute>();
			var removeAttributeSymbol = compilation.GetTypeByMetadataName<PrimaryConstructorIgnoredMemberAttribute>();
			foreach (var type in
				from candidate in receiver.CandidateClasses
				let model = compilation.GetSemanticModel(candidate.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(candidate)! into type
				where (
					from a in type.GetAttributes()
					where SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)
					select a
				).Any()
				select type)
			{
				type.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out _, out _, out _, out _
				);

				var baseClassCtorArgs =
					type.BaseType is { } baseType
					&& baseType.GetAttributes().Any(
						a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)
					) ? GetMembers(baseType, true, attributeSymbol, addAttributeSymbol, removeAttributeSymbol) : null;
				/*length-pattern*/
				string? baseCtorInheritance = baseClassCtorArgs is not { Count: not 0 }
					? null
					: $" : base({string.Join(", ", from x in baseClassCtorArgs select x.ParameterName)})";

				var members = GetMembers(type, false, attributeSymbol, addAttributeSymbol, removeAttributeSymbol);
				string parameterList = string.Join(
					", ",
					from x in baseClassCtorArgs is null ? members : members.Concat(baseClassCtorArgs)
					select $"{x.Type} {x.ParameterName}"
				);
				string memberAssignments = string.Join(
					"\r\n\t\t\t",
					from member in members select $"{member.Name} = {member.ParameterName};"
				);

				context.AddSource(
					type.ToFileName(),
					"PrimaryConstructor",
					$@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial class {type.Name}{genericParametersList}
	{{
		[CompilerGenerated]
		public {type.Name}({parameterList}){baseCtorInheritance}
		{{
			{memberAssignments}
		}}
	}}
}}"
				);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();


		/// <summary>
		/// Try to get all possible fields or properties in the specified <see langword="class"/> type.
		/// </summary>
		/// <param name="type">The specified class symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.</param>
		/// <param name="attributeSymbol">
		/// Indicates the attribute symbol of attribute <see cref="AutoGeneratePrimaryConstructorAttribute"/>.
		/// </param>
		/// <param name="addAttributeSymbol">
		/// Indicates the attribute symbol of attribute <see cref="PrimaryConstructorIncludedMemberAttribute"/>.
		/// </param>
		/// <param name="removeAttributeSymbol">
		/// Indicates the attribute symbol of attribute <see cref="PrimaryConstructorIgnoredMemberAttribute"/>.
		/// </param>
		/// <returns>The result list that contains all member symbols.</returns>
		/// <seealso cref="AutoGeneratePrimaryConstructorAttribute"/>
		/// <seealso cref="PrimaryConstructorIncludedMemberAttribute"/>
		/// <seealso cref="PrimaryConstructorIgnoredMemberAttribute"/>
		private static IReadOnlyList<(string Type, string ParameterName, string Name, IEnumerable<AttributeData> Attributes)> GetMembers(
			INamedTypeSymbol type, bool handleRecursively, INamedTypeSymbol? attributeSymbol,
			INamedTypeSymbol? addAttributeSymbol, INamedTypeSymbol? removeAttributeSymbol)
		{
			var result = new List<(string, string, string, IEnumerable<AttributeData>)>(
				(
					from x in type.GetMembers().OfType<IFieldSymbol>()
					where x is { CanBeReferencedByName: true, IsStatic: false }
						&& (x.IsReadOnly && !x.HasInitializer() || x.GetAttributes().Any(p_include))
						&& !x.GetAttributes().Any(p_ignore)
					select (
						x.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
						x.Name.ToCamelCase(),
						x.Name,
						x.GetAttributes().AsEnumerable()
					)
				).Concat(
					from x in type.GetMembers().OfType<IPropertySymbol>()
					where x is { CanBeReferencedByName: true, IsStatic: false }
						&& (x.IsReadOnly && !x.HasInitializer() || x.GetAttributes().Any(p_include))
						&& !x.GetAttributes().Any(p_ignore)
					select (
						x.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
						x.Name.ToCamelCase(),
						x.Name,
						x.GetAttributes().AsEnumerable()
					)
				)
			);

			if (handleRecursively && type.BaseType is { } baseType && baseType.GetAttributes().Any(p_attribute))
			{
				result.AddRange(
					GetMembers(baseType, true, attributeSymbol, addAttributeSymbol, removeAttributeSymbol)
				);
			}

			return result;


			bool p_include(AttributeData a) => SymbolEqualityComparer.Default.Equals(a.AttributeClass, addAttributeSymbol);
			bool p_ignore(AttributeData a) => SymbolEqualityComparer.Default.Equals(a.AttributeClass, removeAttributeSymbol);
			bool p_attribute(AttributeData a) => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol);
		}
	}
}
