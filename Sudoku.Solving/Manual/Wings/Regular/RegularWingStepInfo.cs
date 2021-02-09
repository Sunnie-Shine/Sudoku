using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;

namespace Sudoku.Solving.Manual.Wings.Regular
{
	/// <summary>
	/// Provides a usage of <b>regular wing</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pivot">The pivot cell.</param>
	/// <param name="PivotCandidatesCount">The number of candidates that is in the pivot.</param>
	/// <param name="DigitsMask">The mask of all digits used.</param>
	/// <param name="Cells">The cells used.</param>
	public sealed record RegularWingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, int Pivot,
		int PivotCandidatesCount, short DigitsMask, IReadOnlyList<int> Cells
	) : WingStepInfo(Conclusions, Views)
	{
		/// <summary>
		/// The difficulty rating.
		/// </summary>
		private static readonly decimal[] DifficultyRating = { 0, 0, 0, 0, 4.6M, 4.8M, 5.1M, 5.4M, 5.7M, 6.0M };

		/// <summary>
		/// The names of all regular wings by their sizes.
		/// </summary>
		public static readonly string[] RegularWingNames =
		{
			string.Empty, string.Empty, string.Empty, string.Empty, "WXYZ-Wing", "VWXYZ-Wing",
			"UVWXYZ-Wing", "TUVWXYZ-Wing", "STUVWXYZ-Wing", "RSTUVWXYZ-Wing"
		};


		/// <summary>
		/// Indicates whether the structure is incomplete.
		/// </summary>
		public bool IsIncomplete => Size == PivotCandidatesCount + 1;

		/// <summary>
		/// Indicates the size of this regular wing.
		/// </summary>
		public int Size => PopCount((uint)DigitsMask);

		/// <inheritdoc/>
		public override decimal Difficulty =>
			Size switch
			{
				3 => IsIncomplete ? 4.2M : 4.4M,
				>= 4 and < 9 => IsIncomplete ? DifficultyRating[Size] + .1M : DifficultyRating[Size]
			};

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel =>
			Size switch
			{
				>= 3 and <= 4 => DifficultyLevel.Hard,
				> 4 and < 9 => DifficultyLevel.Fiendish
			};

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			InternalName switch
			{
				"XY-Wing" => Technique.XyWing,
				"XYZ-Wing" => Technique.XyzWing,
				"WXYZ-Wing" => Technique.WxyzWing,
				"VWXYZ-Wing" => Technique.VwxyzWing,
				"UVWXYZ-Wing" => Technique.UvwxyzWing,
				"TUVWXYZ-Wing" => Technique.TuvwxyzWing,
				"STUVWXYZ-Wing" => Technique.StuvwxyzWing,
				"RSTUVWXYZ-Wing" => Technique.RstuvwxyzWing,
				"Incomplete WXYZ-Wing" => Technique.IncompleteWxyzWing,
				"Incomplete VWXYZ-Wing" => Technique.IncompleteVwxyzWing,
				"Incomplete UVWXYZ-Wing" => Technique.IncompleteUvwxyzWing,
				"Incomplete TUVWXYZ-Wing" => Technique.IncompleteTuvwxyzWing,
				"Incomplete STUVWXYZ-Wing" => Technique.IncompleteStuvwxyzWing,
				"Incomplete RSTUVWXYZ-Wing" => Technique.IncompleteRstuvwxyzWing
			};

		/// <summary>
		/// Indicates the internal name.
		/// </summary>
		private string InternalName =>
			Size switch
			{
				3 => IsIncomplete ? "XY-Wing" : "XYZ-Wing",
				>= 4 and < 9 => IsIncomplete ? $"Incomplete {RegularWingNames[Size]}" : RegularWingNames[Size]
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask).ToString();
			string pivotCellStr = new Cells { Pivot }.ToString();
			string cellOffsetsStr = new Cells(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitsStr} in {pivotCellStr} with {cellOffsetsStr} => {elimStr}";
		}
	}
}
