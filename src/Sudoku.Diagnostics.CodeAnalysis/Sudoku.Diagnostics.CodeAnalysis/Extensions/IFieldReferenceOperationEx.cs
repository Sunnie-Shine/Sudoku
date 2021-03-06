﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IFieldReferenceOperation"/>.
	/// </summary>
	/// <seealso cref="IFieldReferenceOperation"/>
	public static class IFieldReferenceOperationEx
	{
		/// <summary>
		/// Checks whether the current instance has the same reference with the specified one.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="other">Another instance.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool SameReferenceWith(this IFieldReferenceOperation @this, IFieldReferenceOperation other)
		{
			IFieldSymbol field1 = @this.Field, field2 = other.Field;
			return field1.ToDisplayString() == field2.ToDisplayString();
		}
	}
}
