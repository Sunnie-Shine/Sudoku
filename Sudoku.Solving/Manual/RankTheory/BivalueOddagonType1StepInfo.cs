using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.RankTheory
{
	/// <summary>
	/// Provides a usage of <b>bi-value oddagon type 1</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">The loop used.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="ExtraCell">The extra cell.</param>
	public sealed record BivalueOddagonType1StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		in Cells Loop, int Digit1, int Digit2, int ExtraCell
	) : BivalueOddagonStepInfo(Conclusions, Views, Loop, Digit1, Digit2)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.0M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BivalueOddagonType1;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellStr = new Cells { ExtraCell }.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: If the cell {cellStr} only contains the digits " +
				$"{(Digit1 + 1).ToString()} and {(Digit2 + 1).ToString()} " +
				$"(or one of those two), the loop {Loop.ToString()} will form an error structure => {elimStr}";
		}
	}
}
