using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Wings
{
	/// <summary>
	/// Provides a usage of <b>wing</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public abstract record WingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views
	) : StepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => base.ShowDifficulty;

		/// <inheritdoc/>
		public sealed override TechniqueTags TechniqueTags => TechniqueTags.Wings | TechniqueTags.ShortChaining;
	}
}
