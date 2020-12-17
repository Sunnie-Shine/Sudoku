﻿using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Reversal
{
	/// <summary>
	/// Provides a usage of <b>reverse bi-value universal grave type 4</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">All cells used.</param>
	/// <param name="ExtraCells">All extra cells.</param>
	/// <param name="Digit1">The digit 1 used.</param>
	/// <param name="Digit2">The digit 2 used.</param>
	/// <param name="ConjugatePair">Indicates the conjugate pair.</param>
	public sealed record ReverseBugType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Loop,
		in Cells ExtraCells, int Digit1, int Digit2, in ConjugatePair ConjugatePair)
		: ReverseBugStepInfo(Conclusions, Views, Loop, Digit1, Digit2)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.7M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			Loop.Count == 4 ? TechniqueCode.ReverseUrType4 : TechniqueCode.ReverseUlType4;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = Loop.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: We can't fill with the digit {(ConjugatePair.Digit == Digit1 ? Digit2 : Digit1) + 1} " +
				$"bacause of the conjugate pair {ConjugatePair}, " +
				$"otherwise the deadly pattern in cells {cellsStr} will be formed => {elimStr}";
		}
	}
}