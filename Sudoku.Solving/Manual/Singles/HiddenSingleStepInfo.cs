using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a using of <b>hidden single</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Digit">The digit.</param>
	/// <param name="Region">The region.</param>
	/// <param name="EnableAndIsLastDigit">Indicates whether the current technique is a last digit.</param>
	public sealed record HiddenSingleStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, int Cell, int Digit,
		int Region, bool EnableAndIsLastDigit
	) : SingleStepInfo(Conclusions, Views, Cell, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			EnableAndIsLastDigit switch { true => 1.1M, _ => Region switch { < 9 => 1.2M, _ => 1.5M } };

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			EnableAndIsLastDigit
			? Technique.LastDigit
			: (Technique)((int)Technique.HiddenSingleBlock + (int)Region.ToLabel());


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellStr = new Cells { Cell }.ToString();
			int v = Digit + 1;
			return EnableAndIsLastDigit
				? $"{Name}: {cellStr} = {v.ToString()}"
				: $"{Name}: {cellStr} = {v.ToString()} in {new RegionCollection(Region).ToString()}";
		}
	}
}
