using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern type 3</b> (QDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pattern">The pattern.</param>
	/// <param name="ExtraDigitsMask">The extra digits mask.</param>
	/// <param name="ExtraCells">The extra cells.</param>
	public sealed record QdpType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Pattern Pattern, short ExtraDigitsMask, IReadOnlyList<int> ExtraCells
	) : QdpStepInfo(Conclusions, Views, Pattern)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + PopCount((uint)ExtraDigitsMask) * .1M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.QdpType3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string patternStr = Pattern.FullMap.ToString();
			string digitsStr = new DigitCollection(ExtraDigitsMask).ToString();
			string cellsStr = new Cells(ExtraCells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string subsetName = SubsetNames[ExtraCells.Count + 1].ToLower();
			return
				$"{Name}: Cells {patternStr} won't be a deadly pattern " +
				$"if and only if digits {digitsStr} in cells {cellsStr} is a naked {subsetName} => {elimStr}";
		}
	}
}
