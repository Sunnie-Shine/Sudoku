using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Provides a usage of <b>almost locked candidates</b> (ALC) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="DigitsMask">The mask to represent all digits used.</param>
	/// <param name="BaseCells">All base cells.</param>
	/// <param name="TargetCells">All target cells.</param>
	/// <param name="HasValueCell">Indicates whether the current ALC contains value cells.</param>
	public sealed record AlcStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, short DigitsMask,
		in Cells BaseCells, in Cells TargetCells, bool HasValueCell
	) : IntersectionStepInfo(Conclusions, Views)
	{
		/// <summary>
		/// Indicates the size.
		/// </summary>
		public int Size => PopCount((uint)DigitsMask);

		/// <inheritdoc/>
		public override decimal Difficulty => BaseDifficulty + (HasValueCell ? ExtraDifficulty : 0);

		/// <inheritdoc/>
		public override string? Acronym => Size switch { 2 => "ALP", 3 => "ALT", 4 => "ALQ" };

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			Size switch
			{
				2 => Technique.AlmostLockedPair,
				3 => Technique.AlmostLockedTriple,
				4 => Technique.AlmostLockedQuadruple
			};

		/// <summary>
		/// Indicates the base difficulty.
		/// </summary>
		private decimal BaseDifficulty => Size switch { 2 => 4.5M, 3 => 5.2M, 4 => 5.7M };

		/// <summary>
		/// Indicates the extra difficulty.
		/// </summary>
		private decimal ExtraDifficulty => Size switch { 2 => .1M, 3 => .1M, 4 => .2M };


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask).ToString();
			string baseCellsStr = BaseCells.ToString();
			string targetCellsStr = TargetCells.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitsStr} in {baseCellsStr} to {targetCellsStr} => {elimStr}";
		}
	}
}
