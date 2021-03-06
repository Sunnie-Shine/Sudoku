﻿using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Indicates the generator that generates the code that overrides <see cref="object.GetHashCode"/>.
	/// </summary>
	/// <seealso cref="object.GetHashCode"/>
	[Generator]
	public sealed partial class AutoGetHashCodeGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;

			var compilation = context.Compilation;
			var attributeSymbol = compilation.GetTypeByMetadataName<AutoHashCodeAttribute>();
			foreach (var type in
				from type in receiver.Candidates
				let model = compilation.GetSemanticModel(type.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(type)! into symbol
				where (
					from attribute in symbol.GetAttributes()
					where SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)
					select attribute
				).Any()
				select symbol)
			{
				if (type.GetAttributeString(attributeSymbol) is not { } attributeStr)
				{
					continue;
				}

				int tokenStartIndex = attributeStr.IndexOf("({");
				if (tokenStartIndex == -1)
				{
					continue;
				}

				if (attributeStr.GetMemberValues(tokenStartIndex) is not { Length: not 0 } members)
				{
					continue;
				}

				type.DeconstructInfo(
					true, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out _, out string typeKind, out string readonlyKeyword, out _
				);
				string hashCodeStr = string.Join(" ^ ", from member in members select $"{member}.GetHashCode()");

				context.AddSource(
					type.ToFileName(),
					"GetHashCode",
					$@"using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind}{type.Name}{genericParametersList}
	{{
		/// <inheritdoc cref=""object.GetHashCode""/>
		[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override {readonlyKeyword}int GetHashCode() => {hashCodeStr};
	}}
}}"
				);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
	}
}
