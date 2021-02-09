using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave XZ rule</b> (BUG-XZ) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="ExtraCell">The extra cell.</param>
	public sealed record BugXzStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		short DigitsMask, IReadOnlyList<int> Cells, int ExtraCell
	) : BugStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .2M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugXz;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digit = new DigitCollection(DigitsMask).ToString();
			string extraCellStr = new Cells { ExtraCell }.ToString();
			string cellsStr = new Cells(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digit} with cells {cellsStr}, with extra cell {extraCellStr} => {elimStr}";
		}
	}
}
