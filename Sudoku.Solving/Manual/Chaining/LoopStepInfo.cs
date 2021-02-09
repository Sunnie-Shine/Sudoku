using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Extensions;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Chaining
{
#if DOUBLE_LAYERED_ASSUMPTION
	/// <summary>
	/// Provides a usage of <b>(grouped) continuous nice loop</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="XEnabled">Indicates whether the chain is enabled X strong relations.</param>
	/// <param name="yEnabled">Indicates whether the chain is enabled Y strong relations.</param>
	/// <param name="DestOn">The destination node that is on.</param>
	/// <param name="DestOff">The destination node that is off.</param>
#else
	/// <summary>
	/// Provides a usage of <b>(grouped) continuous nice loop</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="XEnabled">Indicates whether the chain is enabled X strong relations.</param>
	/// <param name="yEnabled">Indicates whether the chain is enabled Y strong relations.</param>
	/// <param name="Target">The destination node that is off.</param>
#endif
	public sealed record LoopStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		bool XEnabled, bool YEnabled,
#if DOUBLE_LAYERED_ASSUMPTION
		in Node DestOn, in Node DestOff
#else
		in Node Target
#endif
	) : ChainingStepInfo(Conclusions, Views, XEnabled, YEnabled, default, default, default, default)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			(XEnabled && YEnabled ? 5.0M : 4.5M) + (FlatComplexity - 2).GetExtraDifficultyByLength();

		/// <inheritdoc/>
		public override string? Acronym => "CNL";

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			IsXCycle ? Technique.FishyCycle : IsXyChain ? Technique.XyCycle : Technique.ContinuousNiceLoop;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => TechniqueTags.LongChaining;

		/// <inheritdoc/>
		public override ChainingTypeCode SortKey => Enum.Parse<ChainingTypeCode>(TechniqueCode.ToString());

#if DOUBLE_LAYERED_ASSUMPTION
		/// <inheritdoc/>
		public override Node[] ChainsTargets => new[] { DestOn, DestOff };
#endif

		/// <inheritdoc/>
		public override int FlatComplexity =>
#if DOUBLE_LAYERED_ASSUMPTION
			DestOn.AncestorsCount;
#else
			Target.AncestorsCount;
#endif

		/// <summary>
		/// Indicates whether the specified cycle is an X-Cycle.
		/// </summary>
		private bool IsXCycle => XEnabled && !YEnabled;


		/// <inheritdoc/>
		public override string ToString()
		{
			string chainStr = new LinkCollection(from pair in Views[0].Links! select pair.Value).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {chainStr} => {elimStr}";
		}
	}
}
