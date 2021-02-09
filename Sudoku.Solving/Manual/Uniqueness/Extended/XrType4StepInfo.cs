using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) type 4 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="DigitsMask">All digits mask.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record XrType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, in Cells Cells,
		short DigitsMask, in ConjugatePair ConjugatePair
	) : XrStepInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .1M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.XrType4;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		public override string ToString() => base.ToString();

		/// <inheritdoc/>
		protected override string? GetAdditional() => $"the conjugate pair {ConjugatePair.ToString()}";
	}
}
