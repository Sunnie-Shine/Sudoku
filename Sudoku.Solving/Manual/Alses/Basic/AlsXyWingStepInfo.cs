using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets XY-Wing</b> (ALS-XY-Wing) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Als1">The ALS 1.</param>
	/// <param name="Als2">The ALS 2.</param>
	/// <param name="Bridge">The bridge ALS.</param>
	/// <param name="XDigitsMask">The X digits mask.</param>
	/// <param name="YDigitsMask">The Y digits mask.</param>
	/// <param name="ZDigitsMask">The Z digits mask.</param>
	public sealed record AlsXyWingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, in Als Als1, in Als Als2,
		in Als Bridge, short XDigitsMask, short YDigitsMask, short ZDigitsMask
	) : AlsStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 6.0M;

		/// <inheritdoc/>
		public override string? Acronym => "ALS-XY-Wing";

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.AlsXyWing;


		/// <inheritdoc/>
		public override string ToString()
		{
			string xStr = new DigitCollection(XDigitsMask).ToString();
			string yStr = new DigitCollection(YDigitsMask).ToString();
			string zStr = new DigitCollection(ZDigitsMask).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: {Als1.ToString()} -> {Bridge.ToString()} -> {Als2.ToString()}, " +
				$"x = {xStr}, y = {yStr}, z = {zStr} => {elimStr}";
		}
	}
}
