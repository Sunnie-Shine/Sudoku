using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 3 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TrueCandidates">All true candidates.</param>
	/// <param name="Digits">All digits.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsNaked">Indicates whether the subset is naked.</param>
	public sealed record BugType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		IReadOnlyList<int> TrueCandidates, IReadOnlyList<int> Digits,
		IReadOnlyList<int> Cells, bool IsNaked
	) : BugStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + Digits.Count * .1M + (IsNaked ? 0 : .1M);

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugType3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits).ToString();
			string cellsStr = new Cells(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string sizeStr = SubsetNames[Digits.Count].ToLower();
			string trueCandidatesStr = new Candidates(TrueCandidates).ToString();
			string subsetTypeStr = IsNaked ? "naked" : "hidden";
			return
				$"{Name}: True candidates {trueCandidatesStr} with {subsetTypeStr} {sizeStr} {digitsStr} " +
				$"in cells {cellsStr} => {elimStr}";
		}
	}
}
