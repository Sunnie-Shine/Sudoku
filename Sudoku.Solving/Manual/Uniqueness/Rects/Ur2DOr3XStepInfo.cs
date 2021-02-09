using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle + 2D (or 3X)</b> or
	/// <b>avoidable rectangle + 2D (or 3X)</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TechniqueCode2">The technique code.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	/// <param name="ConjugatePairs">All conjugate pairs.</param>
	/// <param name="XDigit">The X digit.</param>
	/// <param name="YDigit">The Y digit.</param>
	/// <param name="XyCell">The cell that only contains X and Y digit.</param>
	/// <param name="AbsoluteOffset">The absolute offset that used in sorting.</param>
	public sealed record Ur2DOr3XStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		Technique TechniqueCode2, int Digit1, int Digit2, int[] Cells, bool IsAvoidable,
		int XDigit, int YDigit, int XyCell, int AbsoluteOffset
	) : UrStepInfo(Conclusions, Views, TechniqueCode2, Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.7M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		public override string ToString() => base.ToString();

		/// <inheritdoc/>
		protected override string GetAdditional()
		{
			string xyCellStr = new Cells { XyCell }.ToString();
			string xDigitStr = (XDigit + 1).ToString(), yDigitStr = (YDigit + 1).ToString();
			return $"X = {xDigitStr}, Y = {yDigitStr} and a bi-value cell {xyCellStr}";
		}
	}
}
