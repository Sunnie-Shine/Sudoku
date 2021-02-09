using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Provides a usage of <b>empty rectangle</b> (<b>ER</b>) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="Block">The block that the empty rectangle lies in.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record ErStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, int Digit, int Block,
		in ConjugatePair ConjugatePair
	) : SdpStepInfo(Conclusions, Views, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.6M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.EmptyRectangle;


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string regionStr = new RegionCollection(Block).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: {digit.ToString()} in {regionStr} with conjugate pair " +
				$"{ConjugatePair.ToString()} => {elimStr}";
		}
	}
}
