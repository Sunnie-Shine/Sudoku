using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;
#if DOUBLE_LAYERED_ASSUMPTION
using static Sudoku.Constants.Processings;
using static Sudoku.Solving.TechniqueSearcher;
#endif

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Provides a usage of <b>normal fish</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="BaseSets">The base sets.</param>
	/// <param name="CoverSets">The cover sets.</param>
	/// <param name="FinCells">All fin cells.</param>
	/// <param name="IsSashimi">
	/// Indicates whether the fish instance is sashimi.
	/// The value can be:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term><description>Sashimi finned fish.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term><description>Normal finned fish.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term><description>Normal fish.</description>
	/// </item>
	/// </list>
	/// </param>
	public sealed record NormalFishStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, int Digit,
		IReadOnlyList<int> BaseSets, IReadOnlyList<int> CoverSets, in Cells Fins, bool? IsSashimi
	) : FishStepInfo(Conclusions, Views, Digit, BaseSets, CoverSets)
#if DOUBLE_LAYERED_ASSUMPTION
	, IHasParentNodeInfo
#endif
	{
		/// <inheritdoc/>
		public override decimal Difficulty => BaseDifficulty + SashimiExtraDifficulty;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			Enum.Parse<Technique>(InternalName.Replace(" ", string.Empty).Replace("-", string.Empty));

		/// <summary>
		/// Indicates the base difficulty.
		/// </summary>
		private decimal BaseDifficulty => Size switch { 2 => 3.2M, 3 => 3.8M, 4 => 5.2M };

		/// <summary>
		/// Indicates the extra difficulty on sashimi judgement.
		/// </summary>
		private decimal SashimiExtraDifficulty =>
			IsSashimi switch
			{
				null => 0,
				true => Size switch { 2 => .3M, 3 => .3M, 4 => .4M },
				false => .2M
			};

		/// <summary>
		/// Indicates the internal name.
		/// </summary>
		private string InternalName =>
			$"{IsSashimi switch { true => "Sashimi ", false => "Finned ", _ => string.Empty }}{FishNames[Size]}";


		/// <inheritdoc/>
		public override string ToString()
		{
			string baseSetStr = new RegionCollection(BaseSets).ToString();
			string coverSetStr = new RegionCollection(CoverSets).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$@"{Name}: {(Digit + 1).ToString()} in {baseSetStr}\{coverSetStr}" +
				$"{(Fins.IsEmpty ? string.Empty : $" f{Fins.ToString()}")} => {elimStr}";
		}

#if DOUBLE_LAYERED_ASSUMPTION
		/// <inheritdoc/>
		IEnumerable<Node> IHasParentNodeInfo.GetRuleParents(in SudokuGrid initialGrid, in SudokuGrid currentGrid)
		{
			var result = new List<Node>();
			foreach (int baseSet in BaseSets)
			{
				foreach (int cell in RegionMaps[baseSet] & EmptyMap)
				{
					short mask = currentGrid.GetCandidateMask(cell);
					short initialMask = initialGrid.GetCandidateMask(cell);
					if ((initialMask >> Digit & 1) != 0 && !(mask >> Digit & 1) != 0)
					{
						bool isInCoverSet = false;
						foreach (int coverSet in CoverSets)
						{
							foreach (int otherCell in RegionMaps[coverSet] & EmptyMap)
							{
								if (otherCell == cell)
								{
									isInCoverSet = true;
									break;
								}
							}
						}
						if (!isInCoverSet)
						{
							result.Add(new(cell, Digit, false));
						}
					}
				}
			}

			return result;
		}
#endif
	}
}
