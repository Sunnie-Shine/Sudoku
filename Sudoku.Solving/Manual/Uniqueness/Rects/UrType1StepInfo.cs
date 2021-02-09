using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) type 1 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	/// <param name="AbsoluteOffset">The absolute offset that used in sorting.</param>
	public sealed record UrType1StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int Digit1, int Digit2, int[] Cells, bool IsAvoidable, int AbsoluteOffset
	) : UrStepInfo(
		Conclusions, Views, IsAvoidable ? Technique.ArType1 : Technique.UrType1,
		Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset
	)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.5M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		public override string ToString() => base.ToString();

		/// <inheritdoc/>
		protected override string? GetAdditional() => null;
	}
}
