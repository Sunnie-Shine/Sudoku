using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>empty rectangle intersection pair</b> (ERIP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="StartCell">The start cell.</param>
	/// <param name="EndCell">The end cell.</param>
	/// <param name="Region">The region that empty rectangle forms.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	public sealed record EripStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views,
		int StartCell, int EndCell, int Region, int Digit1, int Digit2
	) : AlsStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 6.0M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.Erip;


		/// <inheritdoc/>
		public override string ToString()
		{
			int d1 = Digit1 + 1;
			int d2 = Digit2 + 1;
			string sCellStr = new Cells { StartCell }.ToString();
			string eCellStr = new Cells { EndCell }.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string regionStr = new RegionCollection(Region).ToString();
			return
				$"{Name}: Digits {d1.ToString()}, {d2.ToString()} in bivalue cells {sCellStr} and {eCellStr} " +
				$"with empty rectangle in {regionStr} => {elimStr}";
		}
	}
}
