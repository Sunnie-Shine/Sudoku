using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Provides a usage of <b>two strong links</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="BaseRegion">The base region.</param>
	/// <param name="TargetRegion">The target region.</param>
	public sealed record TwoStrongLinksStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int Digit, int BaseRegion, int TargetRegion
	) : SdpStepInfo(Conclusions, Views, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			TechniqueCode switch
			{
				Technique.TurbotFish => 4.2M,
				Technique.Skyscraper => 4.0M,
				Technique.TwoStringKite => 4.1M
			};

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			(BaseRegion / 9, TargetRegion / 9) switch
			{
				(0, _) or (_, 0) => Technique.TurbotFish,
				(1, 1) or (2, 2) => Technique.Skyscraper,
				(1, 2) or (2, 1) => Technique.TwoStringKite
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string baseRegionStr = new RegionCollection(BaseRegion).ToString();
			string targetRegionStr = new RegionCollection(TargetRegion).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $@"{Name}: {digit} in {baseRegionStr}\{targetRegionStr} => {elimStr}";
		}
	}
}
