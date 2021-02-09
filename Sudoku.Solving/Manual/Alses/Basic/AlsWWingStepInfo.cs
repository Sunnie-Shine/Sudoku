using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets W-Wing</b> (ALS-W-Wing) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Als1">The ALS 1.</param>
	/// <param name="Als2">The ALS 2.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	/// <param name="WDigitsMask">The W digit mask.</param>
	/// <param name="X">The digit X.</param>
	public sealed record AlsWWingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, in Als Als1, in Als Als2,
		in ConjugatePair ConjugatePair, short WDigitsMask, int X
	) : AlsStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 6.2M;

		/// <inheritdoc/>
		public override string? Acronym => "ALS-W-Wing";

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.AlsWWing;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string wStr = new DigitCollection(WDigitsMask).ToString();
			return
				$"{Name}: Two ALSes {Als1.ToString()}, {Als2.ToString()} connected by " +
				$"{ConjugatePair.ToString()}, W = {wStr}, X = {(X + 1).ToString()} => {elimStr}";
		}
	}
}
