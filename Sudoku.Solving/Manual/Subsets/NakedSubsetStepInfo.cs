using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Provides a usage of <b>naked subset</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Region">The region that structure lies in.</param>
	/// <param name="Cells">All cells used.</param>
	/// <param name="Digits">All digits used.</param>
	/// <param name="IsLocked">Indicates whether the subset is locked.</param>
	public sealed record NakedSubsetStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int Region, in Cells Cells, IReadOnlyList<int> Digits, bool? IsLocked
	) : SubsetStepInfo(Conclusions, Views, Region, Cells, Digits)
#if DOUBLE_LAYERED_ASSUMPTION
	, IHasParentNodeInfo
#endif
	{
		/// <inheritdoc/>
		public override decimal Difficulty => BaseDifficulty + ExtraDifficulty;

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			(IsLocked, Digits.Count) switch
			{
				(true, 2) => Technique.LockedPair,
				(false, 2) => Technique.NakedPairPlus,
				(null, 2) => Technique.NakedPair,
				(true, 3) => Technique.LockedTriple,
				(false, 3) => Technique.NakedTriplePlus,
				(null, 3) => Technique.NakedTriple,
				(false, 4) => Technique.NakedQuadruplePlus,
				(null, 4) => Technique.NakedQuadruple
			};

		/// <summary>
		/// Indicates the base difficulty.
		/// </summary>
		private decimal BaseDifficulty => Size switch { 2 => 3.0M, 3 => 3.6M, 4 => 5.0M };

		/// <summary>
		/// Indicates the extra difficulty.
		/// </summary>
		private decimal ExtraDifficulty =>
			IsLocked switch { null => 0, true => Size switch { 2 => -1.0M, 3 => -1.1M }, false => .1M };


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitStr = new DigitCollection(Digits).ToString();
			string regionStr = new RegionCollection(Region).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitStr} in {regionStr} => {elimStr}";
		}

#if DOUBLE_LAYERED_ASSUMPTION
		/// <inheritdoc/>
		IEnumerable<Node> IHasParentNodeInfo.GetRuleParents(in SudokuGrid initialGrid, in SudokuGrid currentGrid)
		{
			short digitsMask = 0;
			foreach (int digit in Digits)
			{
				digitsMask |= (short)(1 << digit);
			}
			digitsMask = (short)(SudokuGrid.MaxCandidatesMask & ~digitsMask);

			var result = new List<Node>();
			foreach (int digit in digitsMask)
			{
				foreach (int cell in Cells)
				{
					var mask = initialGrid.GetCandidateMask(cell);
					if ((mask >> digit & 1) != 0)
					{
						result.Add(new(cell, digit, false));
					}
				}
			}

			return result;
		}
#endif
	}
}
