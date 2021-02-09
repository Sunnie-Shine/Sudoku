using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Encapsulates a usage of <b>irregular wing</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public abstract record IrregularWingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views
	) : WingStepInfo(Conclusions, Views);
}
