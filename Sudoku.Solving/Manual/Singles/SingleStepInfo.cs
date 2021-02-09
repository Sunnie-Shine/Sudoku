using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Provides a usage of <b>single</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Digit">The digit.</param>
	public abstract record SingleStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, int Cell, int Digit
	) : StepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => base.ShowDifficulty;

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Easy;

		/// <inheritdoc/>
		public sealed override TechniqueTags TechniqueTags => TechniqueTags.Singles;
	}
}
