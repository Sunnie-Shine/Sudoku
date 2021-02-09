using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.RankTheory
{
	/// <summary>
	/// Provides a usage of <b>bi-value oddagon</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">The loop used.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	public abstract record BivalueOddagonStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, in Cells Loop,
		int Digit1, int Digit2
	) : RankTheoryStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public virtual bool Equals(BivalueOddagonStepInfo? other) =>
			other is not null && Loop == other.Loop && Digit1 == other.Digit1 && Digit2 == other.Digit2;

		/// <inheritdoc/>
		public override int GetHashCode() =>
			((int)TechniqueCode << 17) + 0x123456 ^ (1 << Digit1 | 1 << Digit2) ^ Loop.GetHashCode();
	}
}
