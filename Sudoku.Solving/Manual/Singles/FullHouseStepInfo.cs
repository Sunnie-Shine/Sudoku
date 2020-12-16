﻿using System.Collections.Generic;
using System.Text;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a usage of <b>full house</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Digit">The digit.</param>
	public sealed record FullHouseStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Cell, int Digit)
		: SingleStepInfo(Conclusions, Views, Cell, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 1.0M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.FullHouse;


		/// <inheritdoc/>
		public override string ToString() =>
			new StringBuilder()
			.Append(Name)
			.Append(Resources.GetValue("Colon"))
			.Append(Resources.GetValue("Space"))
			.Append(new Cells { Cell }.ToString())
			.Append(Resources.GetValue("Equals"))
			.Append(Digit + 1)
			.ToString();

		/// <inheritdoc/>
		public override string ToFullString(CountryCode countryCode)
		{
			return countryCode switch
			{
				CountryCode.ZhCn => toChinese(),
				_ => base.ToFullString(countryCode)
			};

			string toChinese()
			{
				string cellStr = new Cells { Cell }.ToString();
				int digit = Digit + 1;
				return new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append(cellStr)
					.Append(Resources.GetValue("_FullHouse1"))
					.Append(digit)
					.Append(Resources.GetValue("_FullHouse2"))
					.Append(cellStr)
					.Append(Resources.GetValue("Equals"))
					.Append(digit)
					.Append(Resources.GetValue("Period"))
					.ToString();
			}
		}
	}
}