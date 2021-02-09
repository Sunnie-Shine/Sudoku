using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>pattern overlay method</b> (POM) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public sealed record PomStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views
	) : LastResortStepInfo(Conclusions, Views)
	{
		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit => Conclusions[0].Digit;

		/// <inheritdoc/>
		public override decimal Difficulty => 8.5M;

		/// <inheritdoc/>
		public override string? Acronym => "POM";

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.Pom;


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: Digit {digit.ToString()} => {elimStr}";
		}
	}
}
