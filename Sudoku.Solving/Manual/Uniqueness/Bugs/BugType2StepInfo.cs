using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 2 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit.</param>
	/// <param name="Cells">All cell offsets.</param>
	public sealed record BugType2StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int Digit, IReadOnlyList<int> Cells
	) : BugStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			base.Difficulty + (int)(Math.Sqrt(2 * (Cells.Count - 1)) + .5) / 10M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugType2;


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string cellsStr = new Cells(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digit.ToString()} with cells {cellsStr} => {elimStr}";
		}
	}
}
