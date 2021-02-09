using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Provides a usage of <b>senior exocet</b> (SE) technique.
	/// </summary>
	/// <param name="Views">All views.</param>
	/// <param name="Exocet">The exocet.</param>
	/// <param name="Digits">All digits.</param>
	/// <param name="EndoTargetCell">The endo target cell.</param>
	/// <param name="ExtraRegionsMask">The extra regions mask.</param>
	/// <param name="Eliminations">All eliminations.</param>
	public sealed record SeStepInfo(
		IReadOnlyList<PresentationData> Views, in Pattern Exocet, IEnumerable<int> Digits,
		int EndoTargetCell, int[]? ExtraRegionsMask, IReadOnlyList<Elimination> Eliminations
	) : ExocetStepInfo(Views, Exocet, Digits, null, null, Eliminations)
	{
		/// <summary>
		/// Indicates whether the specified instance contains any extra regions.
		/// </summary>
		public bool ContainsExtraRegions => ExtraRegionsMask?.Any(static m => m != 0) ?? false;

		/// <inheritdoc/>
		public override decimal Difficulty => 9.6M + (ContainsExtraRegions ? 0 : .2M);

		/// <inheritdoc/>
		public override string? Acronym => "SE";

		/// <inheritdoc/>
		public override Technique TechniqueCode => ContainsExtraRegions ? Technique.ComplexSe : Technique.Se;


		/// <inheritdoc/>
		public override string ToString() => ToStringInternal();

		/// <inheritdoc/>
		protected override string? GetAdditional()
		{
			const string separator = ", ";
			string endoTargetStr = $"endo-target: {new Cells { EndoTargetCell }.ToString()}";
			if (ExtraRegionsMask is not null)
			{
				var sb = new StringBuilder();
				int count = 0;
				for (int digit = 0; digit < 9; digit++)
				{
					if (ExtraRegionsMask[digit] is not (var mask and not 0))
					{
						continue;
					}

					sb
						.Append(digit + 1)
						.Append(new RegionCollection(mask.GetAllSets()).ToString())
						.Append(separator);
					count++;
				}

				if (count != 0)
				{
					return
						$"{endoTargetStr}. Extra regions will be included: {sb.RemoveFromEnd(separator.Length)}";
				}
			}

			return endoTargetStr;
		}
	}
}
