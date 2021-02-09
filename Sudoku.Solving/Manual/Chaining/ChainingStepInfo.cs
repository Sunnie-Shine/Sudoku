using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;
#if DOUBLE_LAYERED_ASSUMPTION
using Sudoku.Data.Extensions;
using static Sudoku.Constants.Processings;
#endif

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>chain</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="XEnabled">Indicates whether the chain is enabled X strong relations.</param>
	/// <param name="YEnabled">Indicates whether the chain is enabled Y strong relations.</param>
	/// <param name="IsNishio">Indicates whether the chain is a nishio forcing chains (X-Forcing chains).</param>
	/// <param name="IsMultiple">
	/// Indicates whether the chain is a multiple forcing chains (Cell forcing chains and Region forcing chains).
	/// </param>
	/// <param name="IsDynamic">Indicates whether the chain is a dynamic forcing chains.</param>
	/// <param name="Level">The dynamic searching level.</param>
	public abstract record ChainingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		bool XEnabled, bool YEnabled, bool IsNishio, bool IsMultiple, bool IsDynamic, int Level)
		: StepInfo(Conclusions, Views)
#if DOUBLE_LAYERED_ASSUMPTION
		, IHasParentNodeInfo
#endif
	{
		/// <summary>
		/// The sort key.
		/// </summary>
		public abstract ChainingTypeCode SortKey { get; }

#if DOUBLE_LAYERED_ASSUMPTION
		/// <summary>
		/// Indicates the targets of this current chain type.
		/// </summary>
		public abstract Node[] ChainsTargets { get; }
#endif

		/// <summary>
		/// The flat complexity.
		/// </summary>
		public abstract int FlatComplexity { get; }

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => base.ShowDifficulty;

		/// <summary>
		/// The total complexity.
		/// </summary>
		public int Complexity => FlatComplexity;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			this switch
			{
				{ IsNishio: true } => Technique.NishioFc,
				{ IsDynamic: true } => SortKey switch
				{
					ChainingTypeCode.DynamicRegionFc => Technique.DynamicRegionFc,
					ChainingTypeCode.DynamicCellFc => Technique.DynamicCellFc,
					ChainingTypeCode.DynamicContradictionFc => Technique.DynamicContradictionFc,
					ChainingTypeCode.DynamicDoubleFc => Technique.DynamicDoubleFc
				},
				{ IsMultiple: true } => SortKey switch
				{
					ChainingTypeCode.RegionFc => Technique.RegionFc,
					ChainingTypeCode.CellFc => Technique.CellFc,
				},
				_ => Technique.Aic
			};

		/// <summary>
		/// The base difficulty.
		/// </summary>
		protected decimal BaseDifficulty =>
			this switch
			{
				{ IsNishio: true } => 7.5M,
				{ IsDynamic: true } => Level switch
				{
					0 => 8.5M,
					1 => 8.5M + .5M * Level,
					>= 2 => 9.5M + .5M * (Level - 2)
				},
				{ IsMultiple: true } => 8.0M
			};

		/// <summary>
		/// The length difficulty.
		/// </summary>
		protected decimal LengthDifficulty
		{
			get
			{
				decimal result = 0;
				int ceil = 4;
				int length = Complexity - 2;
				for (bool isOdd = false; length > ceil; isOdd = !isOdd)
				{
					result += .1M;
					ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
				}

				return result;
			}
		}

		/// <summary>
		/// Indicates whether the specified chain is an XY-Chain.
		/// </summary>
		protected bool IsXyChain
		{
			get
			{
				if (Views[0].Links is { } links)
				{
					for (int i = 0; i < links.Count; i += 2)
					{
						var (_, _, _, (startCell, _, endCell, _, _)) = links[i];
						if (startCell != endCell)
						{
							return false;
						}
					}

					return true;
				}

				return false;
			}
		}


#nullable disable warnings
		/// <inheritdoc/>
		public virtual bool Equals(ChainingStepInfo? other) =>
			(this, other) switch
			{
				(null, null) => true,
				(not null, not null) => GetHashCode() == other.GetHashCode(),
				_ => false
			};
#nullable restore warnings

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			// Same conclusions hold same hash code.
			int result = 0xC0DE;
			foreach (var conclusion in Conclusions)
			{
				result ^= 0xDECADE | conclusion.GetHashCode();
			}

			return result;
		}

#if DOUBLE_LAYERED_ASSUMPTION
		/// <summary>
		/// Collect rule parents. This method is also used in displaying removed candidates in a single view.
		/// </summary>
		/// <param name="initialGrid">(<see langword="in"/> parameter) The initial grid.</param>
		/// <param name="currentGrid">(<see langword="in"/> parameter) The current grid.</param>
		/// <param name="result">The result.</param>
		/// <param name="target">(<see langword="in"/> parameter) The target node.</param>
		protected void CollectRuleParents(
			in SudokuGrid initialGrid, in SudokuGrid currentGrid, IList<Node> result, in Node target)
		{
			var done = new Set<Node>();
			var todo = new List<Node> { target };
			while (todo.Count != 0)
			{
				var next = new List<Node>();
				foreach (var p in todo)
				{
					if (!done.Contains(p))
					{
						done.Add(p);
						var cause = p.Cause;
						if (cause == Cause.None)
						{
#if DEBUG
							System.Diagnostics.Contracts.Contract.Assert(
								this is LoopStepInfo || p.Parents is not { Count: not 0 });
#endif
							switch (this)
							{
								case CellChainingStepInfo _:
								{
									cause = Cause.NakedSingle;
									break;
								}
								case RegionChainingStepInfo regionChainingInfo:
								{
									cause = GetLabel(regionChainingInfo.Region).GetRegionCause();
									break;
								}
							}
						}

						if (p.IsOn && cause != Cause.None)
						{
#if DEBUG
							System.Diagnostics.Contracts.Contract.Assert(cause != Cause.Advanced);
#endif

							int currentCell = p.Cell;
							if (cause == Cause.NakedSingle)
							{
								short actMask = currentGrid.GetCandidateMask(currentCell);
								short initialMask = initialGrid.GetCandidateMask(currentCell);
								foreach (int digit in (short)(actMask | initialMask))
								{
									if ((initialMask >> digit & 1) != 0 && !(actMask >> digit & 1) != 0)
									{
										result.Add(new(currentCell, digit, false));
									}
								}
							}
							else
							{
								int region = GetRegion(currentCell, cause.GetRegionLabel());
								foreach (int cell in RegionCells[region])
								{
									if (initialGrid.Exists(cell, p.Digit) is true
										&& currentGrid.Exists(cell, p.Digit) is false)
									{
										result.Add(new(currentCell, p.Digit, false));
									}
								}
							}
						}

						if (p.Parents is { } parents)
						{
							next.AddRange(parents);
						}
					}

					todo = next;
				}
			}
		}

		/// <summary>
		/// Get the nested suffix with the specified level.
		/// </summary>
		/// <param name="level">The level.</param>
		/// <returns>The name suffix.</returns>
		private string GetNestedSuffix(int level) =>
			Level switch
			{
				0 => string.Empty,
				1 => " (+)",
				2 => " (+ Alternating Inference Chains)",
				3 => " (+ Multiple Forcing Chains)",
				4 => " (+ Dynamic Forcing Chains)",
				_ => $" (+ Dynamic Forcing Chains{GetNestedSuffix(level - 3)})"
			};

		/// <inheritdoc/>
		IEnumerable<Node> IHasParentNodeInfo.GetRuleParents(in SudokuGrid initialGrid, in SudokuGrid currentGrid)
		{
			// Here we should iterate on each chain target separately.
			// Because they may be equal, but they may have different parents.
			var result = new List<Node>();
			foreach (var target in ChainsTargets)
			{
				// Iterate on chain targets.
				CollectRuleParents(initialGrid, currentGrid, result, target);
			}

			return result;
		}
#endif
	}
}
