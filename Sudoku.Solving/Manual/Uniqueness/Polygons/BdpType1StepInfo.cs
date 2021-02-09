using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern type 1</b> (BDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Map">The cells used.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	public sealed record BdpType1StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Cells Map, short DigitsMask
	) : BdpStepInfo(Conclusions, Views, Map, DigitsMask)
	{
		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BdpType1;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask).ToString();
			string cellsStr = Map.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitsStr} in cells {cellsStr} => {elimStr}";
		}
	}
}
