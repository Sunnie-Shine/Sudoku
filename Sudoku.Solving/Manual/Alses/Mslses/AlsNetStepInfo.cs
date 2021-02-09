using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;
using static System.Math;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets net</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">Indicates the cells used.</param>
	public sealed record AlsNetStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, in Cells Cells
	) : MslsStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 9.4M + (decimal)Floor((Sqrt(1 + 8 * Cells.Count) - 1) / 2) * .1M;

		/// <inheritdoc/>
		public override string? Acronym => "MSLS";

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.Msls;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.Msls;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = Cells.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {Cells.Count.ToString()} cells {cellsStr} => {elimStr}";
		}
	}
}
