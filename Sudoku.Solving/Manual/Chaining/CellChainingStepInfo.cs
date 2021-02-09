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
	/// Provides a usage of <b>cell forcing chains</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="SourceCell">The source cell.</param>
	/// <param name="Chains">All branches.</param>
	/// <param name="IsDynamic">Indicates whether the chain is dynamic.</param>
	/// <param name="Level">Indicates the depth level of the dynamic chains.</param>
	public sealed record CellChainingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int SourceCell, IReadOnlyDictionary<int, Node> Chains, bool IsDynamic, int Level
	) : ChainingStepInfo(Conclusions, Views, default, default, default, true, IsDynamic, Level)
	{
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

#if DOUBLE_LAYERED_ASSUMPTION
		/// <inheritdoc/>
		public override Node[] ChainsTargets => Chains.Values.ToArray();
#endif

		/// <inheritdoc/>
		public override ChainingTypeCode SortKey =>
			IsDynamic ? ChainingTypeCode.DynamicCellFc : ChainingTypeCode.CellFc;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => TechniqueTags.LongChaining;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			string srcCellStr = new Cells { SourceCell }.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: It can be proved using chains that all digits are false from {srcCellStr} => {elimStr}";
		}
	}
}
