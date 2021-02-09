using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern type 4</b> (BDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Map">The cells used.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="ConjugateRegion">
	/// The so-called conjugate region. If you don't know what is a "conjugate region",
	/// please read the comments in the method
	/// <see cref="BdpStepSearcher.CheckType4(IList{StepInfo}, in SudokuGrid, in Pattern, short, short, short, in Cells)"/>
	/// for more details.
	/// </param>
	/// <param name="ExtraMask">Indicates the mask of digits that is the combination.</param>
	/// <seealso cref="BdpStepSearcher.CheckType4(IList{StepInfo}, in SudokuGrid, in Pattern, short, short, short, in Cells)"/>
	public sealed record BdpType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Cells Map, short DigitsMask, in Cells ConjugateRegion, short ExtraMask
	) : BdpStepInfo(Conclusions, Views, Map, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BdpType4;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask).ToString();
			string combStr = new DigitCollection(ExtraMask).ToString();
			string cellsStr = Map.ToString();
			string conjRegion = ConjugateRegion.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: {digitsStr} in cells {cellsStr} with the conjugate region {conjRegion} " +
				$"of the extra digits {combStr} => {elimStr}";
		}
	}
}
