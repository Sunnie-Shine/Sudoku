using System.Collections.Generic;
using System.Extensions;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Provides a usage of <b>Gurth's symmetrical placement</b> (GSP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="SymmetryType">The symmetry type used.</param>
	/// <param name="MappingTable">
	/// The mapping table. The value is always not <see langword="null"/> unless the current instance
	/// contains multiple different symmetry types.
	/// </param>
	public sealed record GspStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		SymmetryType SymmetryType, int?[]? MappingTable
	) : SymmetryStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 7.0M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.Gsp;


		/// <inheritdoc/>
		public override string ToString()
		{
			const string separator = ", ";
			string customName = SymmetryType.GetName().ToLower();
			string conclusions = new ConclusionCollection(Conclusions).ToString();
			if (MappingTable is not null)
			{
				var sb = new StringBuilder();
				for (int i = 0; i < 9; i++)
				{
					int? value = MappingTable[i];
					sb
						.Append(i + 1)
						.Append(
							value.HasValue && value != i
							? $" -> {(value.Value + 1).ToString()}"
							: string.Empty)
						.Append(separator);
				}

				string mapping = sb.RemoveFromEnd(separator.Length).ToString();
				return $"{Name}: Symmetry type: {customName}, mapping relations: {mapping} => {conclusions}";
			}
			else
			{
				return $"{Name}: Symmetry type: {customName} => {conclusions}";
			}
		}


#nullable disable warnings
		/// <summary>
		/// Merge two information, and reserve all conclusions from them two.
		/// </summary>
		/// <param name="left">The left information to merge.</param>
		/// <param name="right">The right information to merge.</param>
		/// <returns>The merge result.</returns>
		public static GspStepInfo? operator |(GspStepInfo? left, GspStepInfo? right)
		{
			switch ((left, right))
			{
				case (null, null):
				{
					return null;
				}
				case (not null, not null):
				{
					var results = new List<Conclusion>(left.Conclusions);
					results.AddRange(right.Conclusions);

					var candidateOffsets = new List<PaintingPair<int>>(left.Views[0].Candidates!);
					candidateOffsets.AddRange(right.Views[0].Candidates!);
					return new(
						results,
						new PresentationData[] { new() { Candidates = candidateOffsets } },
						left.SymmetryType | right.SymmetryType,
						null
					);
				}
				default:
				{
					return new(left ?? right);
				}
			}
		}
#nullable restore warnings
	}
}
