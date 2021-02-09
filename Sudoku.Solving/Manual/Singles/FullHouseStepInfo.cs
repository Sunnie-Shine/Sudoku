using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a usage of <b>full house</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Digit">The digit.</param>
	public sealed record FullHouseStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, int Cell, int Digit
	) : SingleStepInfo(Conclusions, Views, Cell, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 1.0M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.FullHouse;


		/// <inheritdoc/>
		public override string ToString() =>
			$"{Name}: {new Cells { Cell }.ToString()} = {(Digit + 1).ToString()}";
	}
}
