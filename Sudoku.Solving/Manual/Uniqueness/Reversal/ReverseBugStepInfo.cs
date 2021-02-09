using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Reversal
{
	/// <summary>
	/// Provides a usage of <b>reverse bi-value universal grave</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">All cells used.</param>
	/// <param name="Digit1">Indicates the digit 1.</param>
	/// <param name="Digit2">Indicates the digit 2.</param>
	public abstract record ReverseBugStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Cells Loop, int Digit1, int Digit2
	) : UniquenessStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public virtual bool Equals(ReverseBugStepInfo? obj) =>
			obj is not null && Loop == obj.Loop
			&& Digit1 == obj.Digit1 && Digit2 == obj.Digit2 && TechniqueCode == obj.TechniqueCode;

		/// <inheritdoc/>
		public override int GetHashCode() => HashCode.Combine(Loop, Digit1, Digit2, TechniqueCode);
	}
}
