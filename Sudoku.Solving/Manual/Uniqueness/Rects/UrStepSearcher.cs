using System;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Encapsulates an <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) technique searcher.
	/// </summary>
	public sealed partial class UrStepSearcher : UniquenessStepSearcher
	{
		/// <summary>
		/// Indicates whether the UR can be incomplete. In other words,
		/// some of UR candidates can be removed before the pattern forms.
		/// </summary>
		private readonly bool _allowIncompleteUr;

		/// <summary>
		/// Indicates whether the searcher can search for extended URs.
		/// </summary>
		private readonly bool _searchExtended;


		/// <summary>
		/// Initializes an instance with the specified value indicating
		/// whether the structure can be incomplete, and a value indicating
		/// whether the searcher can search for extended URs.
		/// </summary>
		/// <param name="allowIncomplete">
		/// A <see cref="bool"/> value indicating that.
		/// </param>
		/// <param name="searchExtended">A <see cref="bool"/> value indicating that.</param>
		public UrStepSearcher(bool allowIncomplete, bool searchExtended)
		{
			_allowIncompleteUr = allowIncomplete;
			_searchExtended = searchExtended;
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(8, nameof(Technique.UrType1))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// Iterate on mode (whether use AR or UR mode to search).
			var tempList = new List<UrStepInfo>();
			GetAll(tempList, grid, false);
			GetAll(tempList, grid, true);

			// Sort and remove duplicate instances if worth.
			if (tempList.Count == 0)
			{
				return;
			}

			accumulator.AddRange(
				from info in tempList.RemoveDuplicateItems()
				orderby info.TechniqueCode, info.AbsoluteOffset
				select info);
		}

		/// <summary>
		/// Get all possible hints from the grid.
		/// </summary>
		/// <param name="gathered">The list stored the result.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
		private void GetAll(IList<UrStepInfo> gathered, in SudokuGrid grid, bool arMode)
		{
			// Iterate on each possible UR structure.
			for (int index = 0, l = PossibleUrList.Length; index < l; index++)
			{
				int[] urCells = PossibleUrList[index];

				// Check preconditions.
				if (!CheckPreconditions(grid, urCells, arMode))
				{
					continue;
				}

				// Get all candidates that all four cells appeared.
				short mask = 0;
				foreach (int urCell in urCells)
				{
					mask |= grid.GetCandidates(urCell);
				}

				// Iterate on each possible digit combination.
				var allDigitsInThem = mask.GetAllSets();
				for (int i = 0, length = allDigitsInThem.Length; i < length - 1; i++)
				{
					int d1 = allDigitsInThem[i];
					for (int j = i + 1; j < length; j++)
					{
						int d2 = allDigitsInThem[j];

						// All possible UR patterns should contain at least one cell
						// that contains both 'd1' and 'd2'.
						short comparer = (short)(1 << d1 | 1 << d2);
						bool isNotPossibleUr = true;
						foreach (int cell in urCells)
						{
							if (PopCount((uint)(grid.GetCandidates(cell) & comparer)) == 2)
							{
								isNotPossibleUr = false;
								break;
							}
						}
						if (!arMode && isNotPossibleUr)
						{
							continue;
						}

						// Iterate on each corner of four cells.
						for (int c1 = 0; c1 < 4; c1++)
						{
							int corner1 = urCells[c1];
							var otherCellsMap = new Cells(urCells) { ~corner1 };

							CheckType1(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);
							CheckType5(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);
							CheckHidden(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);

							if (!arMode && _searchExtended)
							{
								Check3X(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
								Check3X2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
								Check3N2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
								Check3U2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
								Check3E2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
							}

							if (c1 == 3)
							{
								break;
							}

							for (int c2 = c1 + 1; c2 < 4; c2++)
							{
								int corner2 = urCells[c2];
								var tempOtherCellsMap = otherCellsMap - corner2;

								// Both diagonal and non-diagonal.
								CheckType2(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

								if (_searchExtended)
								{
									for (int size = 2; size <= 4; size++)
									{
										CheckWing(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, size, index);
									}
								}

								switch ((c1, c2))
								{
									case (0, 3) or (1, 2) when !arMode: // Diagonal type.
									{
										CheckType6(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

										if (_searchExtended)
										{
											Check2D(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
											Check2D1SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
										}
										break;
									}
									default: // Non-diagonal type.
									{
										CheckType3Naked(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

										if (!arMode)
										{
											CheckType4(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

											if (_searchExtended)
											{
												Check2B1SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
												Check4X3SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
												Check4C3SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
											}
										}

										if (_searchExtended)
										{
											CheckSdc(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
										}

										break;
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Check preconditions.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
		/// <returns>Indicates whether the UR is passed to check.</returns>
		private static bool CheckPreconditions(in SudokuGrid grid, IEnumerable<int> urCells, bool arMode)
		{
			byte emptyCountWhenArMode = 0, modifiableCount = 0;
			foreach (int urCell in urCells)
			{
				switch (grid.GetStatus(urCell))
				{
					case CellStatus.Given:
					case CellStatus.Modifiable when !arMode:
					{
						return false;
					}
					case CellStatus.Empty when arMode:
					{
						emptyCountWhenArMode++;
						break;
					}
					case CellStatus.Modifiable:
					{
						modifiableCount++;
						break;
					}
				}
			}

			return modifiableCount != 4 && emptyCountWhenArMode != 4;
		}


		/// <summary>
		/// To determine whether the specified region forms a conjugate pair
		/// of the specified digit, and the cells where they contain the digit
		/// is same as the given map contains.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		/// <param name="region">The region.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsConjugatePair(int digit, in Cells map, int region) =>
			(RegionMaps[region] & CandMaps[digit]) == map;

		/// <summary>
		/// Check whether the highlight UR candidates is incomplete.
		/// </summary>
		/// <param name="list">The list to check.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe bool IsIncompleteUr(IEnumerable<PaintingPair<int>> list) =>
			!_allowIncompleteUr && list.Count(static pair => pair.PaletteColorIndex == 0) != 8;

		/// <summary>
		/// Get a cell that can't see each other.
		/// </summary>
		/// <param name="urCells">The UR cells.</param>
		/// <param name="cell">The current cell.</param>
		/// <returns>The diagonal cell.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when the specified argument <paramref name="cell"/> is invalid.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetDiagonalCell(int[] urCells, int cell) =>
			cell == urCells[0]
			? urCells[3]
			: cell == urCells[1]
			? urCells[2]
			: cell == urCells[2] ? urCells[1] : urCells[0];

		/// <summary>
		/// Get a cell that is in the same region of the specified cell lies in.
		/// </summary>
		/// <param name="currentCell">The current cell.</param>
		/// <param name="anotherCell">Another cell to check.</param>
		/// <param name="region">
		/// (<see langword="out"/> parameter) The result regions that both cells lie in.
		/// If the cell can't be found, the parameter will be an empty array of type <see cref="int"/>.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/> value indicating whether the another cell is same region as the current one.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsSameRegionCell(int currentCell, int anotherCell, out int region)
		{
			int coveredRegions = new Cells { anotherCell, currentCell }.CoveredRegions;
			if (coveredRegions != 0)
			{
				region = 0;
				return false;
			}
			else
			{
				region = coveredRegions;
				return true;
			}
		}

		/// <summary>
		/// Get all highlight cells.
		/// </summary>
		/// <param name="urCells">The all UR cells used.</param>
		/// <returns>The list of highlight cells.</returns>
		private static IReadOnlyList<PaintingPair<int>> GetHighlightCells(int[] urCells)
		{
			var result = new PaintingPair<int>[4];
			for (int i = 0; i < 4; i++)
			{
				result[i] = new(0, urCells[i]);
			}

			return result;
		}

		partial void CheckType1(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void CheckType2(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckType3Naked(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckType4(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckType5(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void CheckType6(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckHidden(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check2D(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void Check2B1SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void Check2D1SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void Check3X(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check3X2SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check3N2SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check3U2SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check3E2SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check4X3SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void Check4C3SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckWing(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int size, int index);
		partial void CheckSdc(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
	}
}
