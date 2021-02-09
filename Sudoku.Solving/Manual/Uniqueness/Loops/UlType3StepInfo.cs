using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of <b>unique loop type 3</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Loop">The loop.</param>
	/// <param name="SubsetDigitsMask">The subset digits mask.</param>
	/// <param name="SubsetCells">The subset cells.</param>
	public sealed record UlType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int Digit1, int Digit2, in Cells Loop, short SubsetDigitsMask, IReadOnlyList<int> SubsetCells
	) : UlStepInfo(Conclusions, Views, Digit1, Digit2, Loop)
	{
		/// <inheritdoc/>
		public override int Type => 3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = Loop.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string subsetName = SubsetNames[SubsetCells.Count + 1];
			string digitsStr = new DigitCollection(SubsetDigitsMask).ToString();
			string subsetCellsStr = new Cells(SubsetCells).ToString();
			return
				$"{Name}: Digits {(Digit1 + 1).ToString()}, {(Digit2 + 1).ToString()} in cells {cellsStr} " +
				$"with the naked {subsetName} with extra digits {digitsStr} " +
				$"in cells {subsetCellsStr} => {elimStr}";
		}

		/// <inheritdoc/>
		public bool Equals(UlType3StepInfo? other) =>
			base.Equals(other) && SubsetDigitsMask == other.SubsetDigitsMask;

		/// <inheritdoc/>
		public override int GetHashCode() => base.GetHashCode();
	}
}
