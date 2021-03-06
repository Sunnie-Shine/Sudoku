﻿using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>template</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="IsTemplateDeletion">Indicates whether the current instance is template deletion.</param>
	public sealed record TemplateStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		bool IsTemplateDeletion
	) : LastResortStepInfo(Conclusions, Views)
	{
		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit => Conclusions[0].Digit;

		/// <inheritdoc/>
		public override decimal Difficulty => 9.0M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			IsTemplateDeletion ? Technique.TemplateDelete : Technique.TemplateSet;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.Templating;


		/// <inheritdoc/>
		public override string ToString()
		{
			string conclusionsStr = new ConclusionCollection(Conclusions).ToString();
			string digitStr = (Digit + 1).ToString();
			return $"{Name}: Digit {digitStr} => {conclusionsStr}";
		}
	}
}
