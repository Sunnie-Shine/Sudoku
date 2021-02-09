using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;
#if DOUBLE_LAYERED_ASSUMPTION
using static Sudoku.Solving.TechniqueSearcher;
#endif

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Provides a usage of <b>locked candidates</b> (LC) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit.</param>
	/// <param name="BaseSet">The base region.</param>
	/// <param name="CoverSet">The cover region.</param>
	public sealed record LcStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int Digit, int BaseSet, int CoverSet
	) : IntersectionStepInfo(Conclusions, Views)
#if DOUBLE_LAYERED_ASSUMPTION
	, IHasParentNodeInfo
#endif
	{
		/// <inheritdoc/>
		public override decimal Difficulty => BaseSet < 9 ? 2.6M : 2.8M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;

		/// <inheritdoc/>
		public override Technique TechniqueCode => BaseSet < 9 ? Technique.Pointing : Technique.Claiming;


		/// <inheritdoc/>
		public override string ToString()
		{
			string baseSetStr = new RegionCollection(BaseSet).ToString();
			string coverSetStr = new RegionCollection(CoverSet).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $@"{Name}: {Digit + 1} in {baseSetStr}\{coverSetStr} => {elimStr}";
		}

#if DOUBLE_LAYERED_ASSUMPTION
		/// <inheritdoc/>
		IEnumerable<Node> IHasParentNodeInfo.GetRuleParents(in SudokuGrid initialGrid, in SudokuGrid currentGrid)
		{
			// Add any node of first region that aren't in the second region.
			var result = new List<Node>();
			foreach (int cell in RegionMaps[BaseSet] & EmptyMap)
			{
				short mask = currentGrid.GetCandidateMask(cell);
				short initialMask = initialGrid.GetCandidateMask(cell);
				if ((initialMask >> Digit & 1) != 0 && !(mask >> Digit & 1) != 0)
				{
					bool isInCoverSet = false;
					foreach (int otherCell in RegionCells[CoverSet] & EmptyMap)
					{
						if (cell == otherCell)
						{
							isInCoverSet = true;
							break;
						}
					}

					if (!isInCoverSet)
					{
						result.Add(new(cell, Digit, false));
					}
				}
			}

			return result;
		}
#endif
	}
}
