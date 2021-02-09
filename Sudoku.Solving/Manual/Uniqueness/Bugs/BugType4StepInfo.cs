using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 4 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digits">All digits.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record BugType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		IReadOnlyList<int> Digits, IReadOnlyList<int> Cells, in ConjugatePair ConjugatePair
	) : BugStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .1M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugType4;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits).ToString();
			string cellsStr = new Cells(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: {digitsStr} in cells {cellsStr} with conjugate pair " +
				$"{ConjugatePair.ToString()} => {elimStr}";
		}
	}
}
