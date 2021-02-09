using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>hidden unique rectangle</b> (HUR) or
	/// <b>hidden avoidable rectangle</b> (HAR) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TypeCode">The type code.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	/// <param name="ConjugatePairs">All conjugate pairs.</param>
	/// <param name="AbsoluteOffset">The absolute offset used in sorting.</param>
	public sealed record HiddenUrStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int Digit1, int Digit2, int[] Cells, bool IsAvoidable, IReadOnlyList<ConjugatePair> ConjugatePairs,
		int AbsoluteOffset
	) : UrPlusStepInfo(
		Conclusions, Views, IsAvoidable ? Technique.HiddenAr : Technique.HiddenUr,
		Digit1, Digit2, Cells, IsAvoidable, ConjugatePairs, AbsoluteOffset
	)
	{
		/// <inheritdoc/>
		public override string? Acronym => IsAvoidable ? "HAR" : "HUR";


		/// <inheritdoc/>
		public override string ToString() => base.ToString();
	}
}
