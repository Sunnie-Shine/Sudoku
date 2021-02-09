using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Provides a usage of <b>W-Wing</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="StartCell">The start cell.</param>
	/// <param name="EndCell">The end cell.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record WWingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int StartCell, int EndCell, in ConjugatePair ConjugatePair
	) : IrregularWingStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.4M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.WWing;


		/// <inheritdoc/>
		public override string ToString()
		{
			string startCellStr = new Cells { StartCell }.ToString();
			string endCellStr = new Cells { EndCell }.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: {startCellStr} to {endCellStr} with conjugate pair " +
				$"{ConjugatePair.ToString()} => {elimStr}";
		}
	}
}
