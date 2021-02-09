using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern type 1</b> (QDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pattern">The pattern.</param>
	/// <param name="Candidate">The candidate.</param>
	public sealed record QdpType1StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Pattern Pattern, int Candidate
	) : QdpStepInfo(Conclusions, Views, Pattern)
	{
		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.QdpType1;


		/// <inheritdoc/>
		public override string ToString()
		{
			string patternStr = Pattern.FullMap.ToString();
			string candStr = new Candidates { Candidate }.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Cells {patternStr} will be a deadly pattern " +
				$"if {candStr} is false => {elimStr}";
		}
	}
}
