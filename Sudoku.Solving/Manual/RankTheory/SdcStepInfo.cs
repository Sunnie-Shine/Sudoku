using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.RankTheory
{
	/// <summary>
	/// Provides a usage of <b>sue de coq</b> (SdC) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Block">The block.</param>
	/// <param name="Line">The line.</param>
	/// <param name="BlockMask">The block mask.</param>
	/// <param name="LineMask">The line mask.</param>
	/// <param name="IntersectionMask">The intersection mask.</param>
	/// <param name="IsCannibalistic">Indicates whether the SdC is cannibalistic.</param>
	/// <param name="IsolatedDigitsMask">The isolated digits mask.</param>
	/// <param name="BlockCells">The map of block cells.</param>
	/// <param name="LineCells">The map of line cells.</param>
	/// <param name="IntersectionCells">The map of intersection cells.</param>
	public sealed record SdcStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int Block, int Line, short BlockMask, short LineMask, short IntersectionMask,
		bool IsCannibalistic, short IsolatedDigitsMask, in Cells BlockCells, in Cells LineCells,
		in Cells IntersectionCells
	) : RankTheoryStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.0M + IsolatedExtraDifficulty + CannibalismExtraDifficulty;

		/// <inheritdoc/>
		public override string? Acronym => "SDC";

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.Als;

		/// <inheritdoc/>
		public override Technique TechniqueCode => IsCannibalistic ? Technique.CannibalizedSdc : Technique.Sdc;

		/// <summary>
		/// Indicates the extra difficulty on cannibalism.
		/// </summary>
		private decimal CannibalismExtraDifficulty => IsCannibalistic ? .2M : 0;

		/// <summary>
		/// Indicates the extra difficulty on isolated digits.
		/// </summary>
		private decimal IsolatedExtraDifficulty => IsolatedDigitsMask != 0 ? .1M : 0;


		/// <inheritdoc/>
		public override string ToString()
		{
			string blockCellsStr = BlockCells.ToString();
			string blockDigitsStr = new DigitCollection(BlockMask).ToString(null);
			string lineCellsStr = LineCells.ToString();
			string lineDigitsStr = new DigitCollection(LineMask).ToString(null);
			string interCellsStr = IntersectionCells.ToString();
			string interDigitsStr = new DigitCollection(IntersectionMask).ToString(null);
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: {interCellsStr}({interDigitsStr}) - " +
				$"{blockCellsStr}({blockDigitsStr}) & {lineCellsStr}({lineDigitsStr}) => {elimStr}";
		}
	}
}
