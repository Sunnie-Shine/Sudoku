using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="DigitsMask">All digits mask.</param>
	public abstract record XrStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Cells Cells, short DigitsMask
	) : UniquenessStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.5M + (Cells.Count >> 1 - 2) * .1M;

		/// <inheritdoc/>
		public sealed override string? Acronym => "XR";

		/// <inheritdoc/>
		public abstract override Technique TechniqueCode { get; }


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask).ToString();
			string cellsStr = Cells.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string? additional = GetAdditional();
			return
				$"{Name}: {digitsStr} in {cellsStr}" +
				$"{(additional is null ? string.Empty : $" with {additional}")} => {elimStr}";
		}

		/// <summary>
		/// Get additional string.
		/// </summary>
		/// <returns>The additional string.</returns>
		protected abstract string? GetAdditional();
	}
}
