﻿using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle + sue de coq</b> (UR + SDC) or
	/// <b>avoidable rectangle + sue de coq</b> (AR + SDC) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells used.</param>
	/// <param name="IsAvoidable">Indicates whether the instance is an AR.</param>
	/// <param name="AbsoluteOffset">The absolute offset.</param>
	/// <param name="Block">The block.</param>
	/// <param name="Line">The line.</param>
	/// <param name="BlockMask">The block mask.</param>
	/// <param name="LineMask">The line mask.</param>
	/// <param name="IntersectionMask">The intersection mask.</param>
	/// <param name="IsCannibalistic">Indicates whether the current SdC used is cannibal.</param>
	/// <param name="IsolatedDigitsMask">The isolated digits mask.</param>
	/// <param name="BlockCells">The block cells.</param>
	/// <param name="LineCells">The line cells.</param>
	/// <param name="IntersectionCells">The intersection cells.</param>
	public sealed record UrWithSdcStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit1, int Digit2,
		int[] Cells, bool IsAvoidable, int AbsoluteOffset, int Block, int Line, short BlockMask,
		short LineMask, short IntersectionMask, bool IsCannibalistic, short IsolatedDigitsMask,
		in Cells BlockCells, in Cells LineCells, in Cells IntersectionCells)
		: UrStepInfo(
			Conclusions, Views, IsAvoidable ? TechniqueCode.ArSdc : TechniqueCode.UrSdc,
			Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			5.0M + SdCDifficulty + IsolatedDifficulty + CannibalDifficulty + ArDifficulty;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <summary>
		/// The extra difficulty that assesses SDC's size.
		/// </summary>
		private decimal SdCDifficulty => (LineCells | BlockCells).Count * .1M;

		/// <summary>
		/// The extra difficulty that assesses isolated digit.
		/// </summary>
		private decimal IsolatedDifficulty => !IsCannibalistic && IsolatedDigitsMask != 0 ? .1M : 0;

		/// <summary>
		/// The extra difficulty that assesses cannibalism.
		/// </summary>
		private decimal CannibalDifficulty => IsCannibalistic ? .1M : 0;

		/// <summary>
		/// The extra difficulty that assesses whether the structure is an AR.
		/// </summary>
		private decimal ArDifficulty => IsAvoidable ? .1M : 0;


		/// <inheritdoc/>
		public override string ToString() => base.ToString();

		/// <inheritdoc/>
		protected override string GetAdditional()
		{
			var digits = new DigitCollection((LineMask | BlockMask).GetAllSets()).ToString();
			var mergedCells = LineCells | BlockCells;
			return $"a generalized Sue de Coq in cells {mergedCells} of digits {digits}";
		}
	}
}