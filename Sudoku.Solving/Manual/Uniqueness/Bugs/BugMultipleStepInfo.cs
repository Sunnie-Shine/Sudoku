using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Resources;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>BUG + n</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Candidates">All candidates used.</param>
	public sealed record BugMultipleStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		IReadOnlyList<int> Candidates
	) : BugStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override string Name => $"{TextResources.Current.Bug} + {Candidates.Count.ToString()}";

		/// <inheritdoc/>
		public override string? Acronym => $"BUG + {Candidates.Count.ToString()}";

		/// <inheritdoc/>
		public override decimal Difficulty =>
			base.Difficulty + .1M + (int)(Math.Sqrt(2 * (Candidates.Count - 1)) + .5) / 10M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugMultiple;


		/// <inheritdoc/>
		public override string ToString()
		{
			string candsStr = new Cells(Candidates).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: True candidates: {candsStr} => {elimStr}";
		}
	}
}
