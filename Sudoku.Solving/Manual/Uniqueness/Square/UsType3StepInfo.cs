using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Provides a usage of <b>unique square type 3</b> (US) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">The cells.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="ExtraDigitsMask">The extra digits mask.</param>
	/// <param name="ExtraCells">The extra cells.</param>
	public sealed record UsType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Cells Cells, short DigitsMask, short ExtraDigitsMask, IReadOnlyList<int> ExtraCells
	) : UsStepInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + ExtraDifficulty;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.UsType3;

		/// <summary>
		/// Indicates the extra difficulty.
		/// </summary>
		private decimal ExtraDifficulty => PopCount((uint)ExtraDigitsMask) * .1M;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask).ToString();
			string cellsStr = Cells.ToString();
			string subsetDigitsStr = new DigitCollection(ExtraDigitsMask).ToString();
			string subsetCellsStr = new Cells(ExtraCells).ToString();
			string subsetName = SubsetNames[ExtraCells.Count + 1];
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Digits {digitsStr} in cells {cellsStr} can be avoid to form a deadly pattern " +
				$"if and only if the digits {subsetDigitsStr} in cells {subsetCellsStr} " +
				$"form a naked {subsetName} => {elimStr}";
		}
	}
}
