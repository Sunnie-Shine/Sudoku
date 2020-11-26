﻿using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

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
	public sealed record EripTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int StartCell, int EndCell,
		int Region, int Digit1, int Digit2) : AlsTechniqueInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 6.0M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.Erip;


		/// <inheritdoc/>
		public override string ToString()
		{
			int d1 = Digit1 + 1;
			int d2 = Digit2 + 1;
			string sCellStr = new GridMap { StartCell }.ToString();
			string eCellStr = new GridMap { EndCell }.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string regionStr = new RegionCollection(Region).ToString();
			return
				$"{Name}: Digits {d1}, {d2} in bivalue cells {sCellStr} and {eCellStr} " +
				$"with empty rectangle in {regionStr} => {elimStr}";
		}
	}
}