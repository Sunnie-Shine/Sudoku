﻿using System;
using static Sudoku.Data.SudokuGrid;

namespace Sudoku.Data
{
	/// <summary>
	/// Extends the iterators that allows all custom iterators to iterate on LINQ clauses.
	/// </summary>
	public static class Iterators
	{
		/// <summary>
		/// Checks all elements, and selects the values that satisfy the condition
		/// specified as a delegate method.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The enumerator.</param>
		/// <param name="condition">The condition method.</param>
		/// <returns>The iterator that iterates on each target element satisfying the condition.</returns>
		public static LetWhereIterator<T> Where<T>(
			this in SelectIterator<T> @this, Predicate<T?> condition
		) => new(@this, condition);

		/// <summary>
		/// Converts all elements to the target instances using the specified converter.
		/// </summary>
		/// <typeparam name="TConverted">The type of the target elements.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The enumerator.</param>
		/// <param name="converter">The convert method.</param>
		/// <returns>The iterator that iterates on each target element.</returns>
		public static WhereSelectIterator<TConverted> Select<TConverted>(
			this in WhereIterator @this, Func<int, TConverted> converter
		) => new(@this, converter);

		/// <summary>
		/// Converts all elements to the target instances using the specified converter.
		/// </summary>
		/// <typeparam name="TElement">The type of the base elements.</typeparam>
		/// <typeparam name="TConverted">The type of the target elements.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The enumerator.</param>
		/// <param name="converter">The convert method.</param>
		/// <returns>The iterator that iterates on each target element.</returns>
		public static LetWhereSelectIterator<TElement, TConverted> Select<TElement, TConverted>(
			this in LetWhereIterator<TElement?> @this, Func<TElement?, TConverted?> converter
		) => new(@this, converter);
	}
}
