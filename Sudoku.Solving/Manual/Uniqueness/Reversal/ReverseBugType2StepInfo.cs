using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Reversal
{
	/// <summary>
	/// Provides a usage of <b>reverse bi-value universal grave type 2</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">All cells used.</param>
	/// <param name="ExtraCells">All extra cells.</param>
	/// <param name="Digit1">The digit 1 used.</param>
	/// <param name="Digit2">The digit 2 used.</param>
	/// <param name="ExtraDigit">The extra digit.</param>
	public sealed record ReverseBugType2StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, in Cells Loop,
		in Cells ExtraCells, int Digit1, int Digit2, int ExtraDigit
	) : ReverseBugStepInfo(Conclusions, Views, Loop, Digit1, Digit2)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.7M;

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			Loop.Count == 4 ? Technique.ReverseUrType2 : Technique.ReverseUlType2;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string anchor = ExtraCells.ToString();
			string digitsStr = new DigitCollection(stackalloc[] { Digit1, Digit2 }).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Digits {digitsStr} in all empty cells may form a deadly pattern " +
				$"when {anchor} is only last the digit {(ExtraDigit + 1).ToString()} => {elimStr}";
		}
	}
}
