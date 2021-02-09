using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.RankTheory
{
	/// <summary>
	/// Provides a usage of <b>rank theory</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public abstract record RankTheoryStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views
	) : StepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => base.ShowDifficulty;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => TechniqueTags.RankTheory;
	}
}
