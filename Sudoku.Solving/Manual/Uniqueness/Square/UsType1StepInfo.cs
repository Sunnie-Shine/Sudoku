using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Provides a usage of <b>unique square type 1</b> (US) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">The cells.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="Candidate">Indicates the true candidate.</param>
	public sealed record UsType1StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Cells Cells, short DigitsMask, int Candidate
	) : UsStepInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.UsType1;


		/// <inheritdoc/>
		public override string ToString()
		{
			string candStr = new Candidates { Candidate }.ToString();
			string digitsStr = new DigitCollection(DigitsMask).ToString();
			string cellsStr = Cells.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Digits {digitsStr} in cells {cellsStr} will form a deadly pattern if " +
				$"the candidate {candStr} is false => {elimStr}";
		}
	}
}
