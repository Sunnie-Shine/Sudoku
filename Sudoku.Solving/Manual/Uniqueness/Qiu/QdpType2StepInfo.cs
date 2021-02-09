using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern type 2</b> (QDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pattern">The pattern.</param>
	/// <param name="ExtraDigit">The extra digit.</param>
	public sealed record QdpType2StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Pattern Pattern, int ExtraDigit
	) : QdpStepInfo(Conclusions, Views, Pattern)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .1M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.QdpType2;


		/// <inheritdoc/>
		public override string ToString()
		{
			string patternStr = Pattern.FullMap.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Cells {patternStr} will be a deadly pattern " +
				$"if the extra digits {(ExtraDigit + 1).ToString()} in pair cells is both false => {elimStr}";
		}
	}
}
