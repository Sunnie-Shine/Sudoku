using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;
#if DOUBLE_LAYERED_ASSUMPTION
using System.Linq;
#endif

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>region forcing chains</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Region">The region.</param>
	/// <param name="Digit">The digit.</param>
	/// <param name="Chains">All branches.</param>
	/// <param name="IsDynamic">Indicates whether the chain is dynamic.</param>
	/// <param name="Level">Indicates the depth level of the dynamic chains.</param>
	public sealed record RegionChainingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int Region, int Digit, IReadOnlyDictionary<int, Node> Chains, bool IsDynamic, int Level
	) : ChainingStepInfo(Conclusions, Views, default, default, default, true, IsDynamic, Level)
	{
		/// <inheritdoc/>
		public override ChainingTypeCode SortKey =>
			IsDynamic ? ChainingTypeCode.DynamicRegionFc : ChainingTypeCode.RegionFc;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => TechniqueTags.LongChaining;

#if DOUBLE_LAYERED_ASSUMPTION
		/// <inheritdoc/>
		public override Node[] ChainsTargets => Chains.Values.ToArray();
#endif

		/// <inheritdoc/>
		public override int FlatComplexity
		{
			get
			{
				int result = 0;
				foreach (var node in Chains.Values)
				{
					result += node.AncestorsCount;
				}

				return result;
			}
		}

		/// <inheritdoc/>
		public override decimal Difficulty => BaseDifficulty + LengthDifficulty;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			string regionStr = new RegionCollection(Region).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: It can be proved using chains that digit {(Digit + 1).ToString()} from {regionStr} " +
				$"are false => {elimStr}";
		}
	}
}
