﻿using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Exocets.Eliminations;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Provides a usage of <b>senior exocet</b> (SE) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Exocet">The exocet.</param>
	/// <param name="Digits">All digits.</param>
	/// <param name="EndoTargetCell">The endo target cell.</param>
	/// <param name="ExtraRegionsMask">The extra regions mask.</param>
	/// <param name="TargetEliminations">The target eliminations.</param>
	/// <param name="TrueBaseEliminations">The true base eliminations.</param>
	/// <param name="MirrorEliminations">The mirror eliminations.</param>
	/// <param name="CompatibilityEliminations">The compatibility eliminations.</param>
	public sealed record SeStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Pattern Exocet,
		IEnumerable<int> Digits, int EndoTargetCell, int[]? ExtraRegionsMask,
		Target? TargetEliminations, TrueBase? TrueBaseEliminations,
		Mirror? MirrorEliminations, CompatibilityTest? CompatibilityEliminations)
		: ExocetStepInfo(
			Conclusions, Views, Exocet, Digits, null, null,
			TargetEliminations, MirrorEliminations, default, default, default,
			TrueBaseEliminations, CompatibilityEliminations)
	{
		/// <summary>
		/// Indicates whether the specified instance contains any extra regions.
		/// </summary>
		public bool ContainsExtraRegions => ExtraRegionsMask?.Any(static m => m != 0) ?? false;

		/// <inheritdoc/>
		public override decimal Difficulty => 9.6M + (ContainsExtraRegions ? 0 : .2M);

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			ContainsExtraRegions ? TechniqueCode.ComplexSe : TechniqueCode.Se;


		/// <inheritdoc/>
		public override string ToString() => ToStringInternal();

		/// <inheritdoc/>
		protected override string? GetAdditional()
		{
			const string separator = ", ";
			string endoTargetStr = $"endo-target: {new Cells { EndoTargetCell }}";
			if (ExtraRegionsMask is not null)
			{
				var sb = new StringBuilder();
				int count = 0;
				for (int digit = 0; digit < 9; digit++)
				{
					if (ExtraRegionsMask[digit] is var mask and not 0)
					{
						sb
							.Append(digit + 1)
							.Append(new RegionCollection(mask.GetAllSets()).ToString())
							.Append(separator);
						count++;
					}
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