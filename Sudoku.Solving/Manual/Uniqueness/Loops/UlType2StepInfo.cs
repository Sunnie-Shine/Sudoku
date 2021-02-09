using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of <b>unique loop type 2</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Loop">The loop.</param>
	/// <param name="ExtraDigit">The extra digit.</param>
	public sealed record UlType2StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int Digit1, int Digit2, in Cells Loop, int ExtraDigit
	) : UlStepInfo(Conclusions, Views, Digit1, Digit2, Loop)
	{
		/// <inheritdoc/>
		public override int Type => 2;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = Loop.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Digits {(Digit1 + 1).ToString()}, {(Digit2 + 1).ToString()} in cells {cellsStr} " +
				$"with the extra digit {(ExtraDigit + 1).ToString()} => {elimStr}";
		}

		/// <inheritdoc/>
		public bool Equals(UlType2StepInfo? other) => base.Equals(other);

		/// <inheritdoc/>
		public override int GetHashCode() => base.GetHashCode();
	}
}
