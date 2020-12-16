﻿using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Extensions;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>Bowman's bingo</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="ContradictionSeries">Indicates all conclusions that occurs a contradict.</param>
	public sealed record BowmanBingoStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		IReadOnlyList<Conclusion> ContradictionSeries) : LastResortStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 8.0M + ContradictionSeries.Count.GetExtraDifficultyByLength();

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BowmanBingo;


		/// <inheritdoc/>
		public override string ToString()
		{
			string contradictionSeriesStr = new ConclusionCollection(ContradictionSeries).ToString(false, " -> ");
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: Try {contradictionSeriesStr} => {elimStr}";
		}
	}
}