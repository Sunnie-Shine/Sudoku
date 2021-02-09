using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Extensions;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>Bowman's bingo</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="ContradictionSeries">Indicates all conclusions that occurs a contradict.</param>
	public sealed record BowmanBingoStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		IReadOnlyList<Conclusion> ContradictionSeries
	) : LastResortStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 8.0M + ContradictionSeries.Count.GetExtraDifficultyByLength();

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.LongChaining;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BowmanBingo;


		/// <inheritdoc/>
		public override string ToString()
		{
			string contradictionSeriesStr = new ConclusionCollection(ContradictionSeries).ToString(false, " -> ");
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: Try {contradictionSeriesStr} => {elimStr}";
		}
	}
}
