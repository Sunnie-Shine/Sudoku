using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Provides a usage of <b>subset</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Region">The region that structure lies in.</param>
	/// <param name="Cells">All cells used.</param>
	/// <param name="Digits">All digits used.</param>
	public abstract record SubsetStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int Region, in Cells Cells, IReadOnlyList<int> Digits
	) : StepInfo(Conclusions, Views)
	{
		/// <summary>
		/// Indicates the size.
		/// </summary>
		public int Size => Digits.Count;

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => base.ShowDifficulty;

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;

		/// <inheritdoc/>
		public sealed override TechniqueTags TechniqueTags => TechniqueTags.Subsets;
	}
}
