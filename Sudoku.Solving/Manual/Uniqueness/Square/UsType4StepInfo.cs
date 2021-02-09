using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Provides a usage of <b>unique square type 4</b> (US) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">The cells.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="ConjugateRegion">The so-called conjugate region.</param>
	public sealed record UsType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Cells Cells, short DigitsMask, int Digit1, int Digit2, in Cells ConjugateRegion
	) : UsStepInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.UsType4;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask).ToString();
			string cellsStr = Cells.ToString();
			string conjStr = ConjugateRegion.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Digits {digitsStr} in cells {cellsStr} can avoid to form a deadly pattern " +
				$"if and only if the conjugate region {conjStr} can't set the digit " +
				$"neither {(Digit1 + 1).ToString()} nor {(Digit2 + 1).ToString()} => {elimStr}";
		}
	}
}
