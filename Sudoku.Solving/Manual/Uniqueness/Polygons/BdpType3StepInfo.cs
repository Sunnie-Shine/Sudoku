using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern type 3</b> (BDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Map">The cells used.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="ExtraCells">The extra cells.</param>
	/// <param name="ExtraDigitsMask">The extra digits mask.</param>
	public sealed record BdpType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Cells Map, short DigitsMask, in Cells ExtraCells, short ExtraDigitsMask
	) : BdpStepInfo(Conclusions, Views, Map, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.2M + ExtraCells.Count * .1M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BdpType3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask).ToString();
			string cellsStr = Map.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string exDigitsStr = new DigitCollection(ExtraDigitsMask).ToString();
			string exCellsStr = ExtraCells.ToString();
			return
				$"{Name}: {digitsStr} in cells {cellsStr} with the digits {exDigitsStr} in cells {exCellsStr}" +
				$" => {elimStr}";
		}
	}
}
