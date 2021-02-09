using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TechniqueCode2">The technique code.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	/// <param name="AbsoluteOffset">The absolute offset that used in sorting.</param>
	public abstract record UrStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		Technique TechniqueCode2, int Digit1, int Digit2, int[] Cells, bool IsAvoidable, int AbsoluteOffset
	) : UniquenessStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override string? Acronym => IsAvoidable ? "AR" : "UR";

		/// <inheritdoc/>
		public sealed override Technique TechniqueCode => TechniqueCode2;


		/// <inheritdoc/>
		public override string ToString()
		{
			int d1 = Digit1 + 1, d2 = Digit2 + 1;
			string cellsStr = new Cells(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string? additional = GetAdditional();
			return
				$"{Name}: Digits {d1.ToString()} and {d2.ToString()} in {cellsStr}" +
				$"{(additional is null ? string.Empty : $" with {additional}")} => {elimStr}";
		}

		/// <summary>
		/// Get additional string.
		/// </summary>
		/// <returns>The additional string.</returns>
		protected abstract string? GetAdditional();
	}
}
