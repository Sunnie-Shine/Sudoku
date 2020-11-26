﻿using System.Collections.Generic;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Provides a usage of <b>death blossom</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pivot">The pivot cell.</param>
	/// <param name="Alses">All ALSes used.</param>
	public sealed record DbTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Pivot,
		IReadOnlyDictionary<int, Als> Alses) : AlsTechniqueInfo(Conclusions, Views)
	{
		/// <summary>
		/// Indicates how many petals used.
		/// </summary>
		public int PetalsCount => Alses.Count;

		/// <inheritdoc/>
		public override decimal Difficulty => 8.0M + PetalsCount * .1M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.DeathBlossom;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			const string separator = ", ";
			string pivotStr = new GridMap { Pivot }.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: Cell {pivotStr} - {g()} => {elimStr}";

			unsafe string g() =>
				new StringBuilder()
					.AppendRange<KeyValuePair<int, Als>, string>(Alses, &converter)
					.RemoveFromEnd(separator.Length)
					.ToString();

			static string converter(in KeyValuePair<int, Als> pair) =>
				$"{pair.Key + 1} - {pair.Value}{separator}";
		}
	}
}