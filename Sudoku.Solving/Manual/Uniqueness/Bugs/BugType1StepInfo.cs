using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 1 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public sealed record BugType1StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views
	) : BugStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugType1;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {elimStr}";
		}
	}
}
