using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) type 3 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="DigitsMask">All digits mask.</param>
	/// <param name="ExtraCells">All extra cells.</param>
	/// <param name="ExtraDigitsMask">All extra digits mask.</param>
	/// <param name="Region">The region.</param>
	public sealed record XrType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, in Cells Cells,
		short DigitsMask, IReadOnlyList<int> ExtraCells, short ExtraDigitsMask, int Region
	) : XrStepInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .1M * PopCount((uint)ExtraDigitsMask);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.XrType3;


		/// <inheritdoc/>
		public override string ToString() => base.ToString();

		/// <inheritdoc/>
		protected override string GetAdditional()
		{
			string digitsStr = new DigitCollection(ExtraDigitsMask).ToString();
			string cellsStr = new Cells(ExtraCells).ToString();
			string regionStr = new RegionCollection(Region).ToString();
			return $"{digitsStr} in cells {cellsStr} in {regionStr}";
		}
	}
}
