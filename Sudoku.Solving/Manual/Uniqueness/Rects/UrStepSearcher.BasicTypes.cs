﻿using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Solving.Manual.Uniqueness.Rects.UrTypeCode;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	partial class UrStepSearcher
	{
		/// <summary>
		/// Check type 1.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void CheckType1(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index)
		{
			//   ↓ cornerCell
			// (abc) ab
			//  ab   ab

			// Get the summary mask.
			if (grid.BitwiseOrMasks(otherCellsMap) is var mask && mask != comparer)
			{
				return;
			}

			// Type 1 found. Now check elimination.
			var conclusions = new List<Conclusion>();
			if (grid.Exists(cornerCell, d1) is true)
			{
				conclusions.Add(new(Elimination, cornerCell, d1));
			}
			if (grid.Exists(cornerCell, d2) is true)
			{
				conclusions.Add(new(Elimination, cornerCell, d2));
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var candidateOffsets = new List<DrawingInfo>();
			foreach (int cell in otherCellsMap)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(0, cell * 9 + digit));
				}
			}

			if (!_allowIncompleteUr && (candidateOffsets.Count, conclusions.Count) != (6, 2))
			{
				return;
			}

			accumulator.Add(
				new UrType1StepInfo(
					conclusions,
					new View[]
					{
						new()
						{
							Cells = arMode ? GetHighlightCells(urCells) : null,
							Candidates = arMode ? null : candidateOffsets
						}
					},
					d1,
					d2,
					urCells,
					arMode,
					index));
		}

		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void CheckType2(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			//   ↓ corner1 and corner2
			// (abc) (abc)
			//  ab    ab

			// Get the summary mask.
			if (grid.BitwiseOrMasks(otherCellsMap) is var mask && mask != comparer)
			{
				return;
			}

			int extraMask = (grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) ^ comparer;
			if (extraMask.PopCount() != 1)
			{
				return;
			}

			// Type 2 or 5 found. Now check elimination.
			int extraDigit = extraMask.FindFirstSet();
			var elimMap = new Cells { corner1, corner2 }.PeerIntersection & CandMaps[extraDigit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new(Elimination, cell, extraDigit));
			}

			var candidateOffsets = new List<DrawingInfo>();
			foreach (int cell in urCells)
			{
				if (grid.GetStatus(cell) == Empty)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(digit == extraDigit ? 1 : 0, cell * 9 + digit));
					}
				}
			}

			unsafe
			{
				if (!_allowIncompleteUr && candidateOffsets.Count(&CheckHighlightType) != 8)
				{
					return;
				}
			}

			bool isType5 = !new Cells { corner1, corner2 }.InOneRegion;
			accumulator.Add(
				new UrType2StepInfo(
					conclusions,
					new View[]
					{
						new()
						{
							Cells = arMode ? GetHighlightCells(urCells) : null,
							Candidates = candidateOffsets
						}
					},
					(arMode, isType5) switch
					{
						(true, true) => AType5,
						(true, false) => AType2,
						(false, true) => Type5,
						(false, false) => Type2
					},
					d1,
					d2,
					urCells,
					arMode,
					extraDigit,
					index));
		}

		/// <summary>
		/// Check type 3.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void CheckType3Naked(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			//  ↓ corner1, corner2
			// (ab ) (ab )
			//  abx   aby
			static bool isSatisfiedType3(int cell, in short comparer, in bool arMode, in SudokuGrid grid) =>
				grid.GetCandidates(cell) is var mask && (mask & comparer) == 0
				|| mask == comparer || arMode && grid.GetStatus(cell) != Empty;

			bool satisfiedType3;
			unsafe
			{
				satisfiedType3 = otherCellsMap.Any(&isSatisfiedType3, comparer, arMode, grid);
			}

			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer || satisfiedType3)
			{
				return;
			}

			short mask = grid.BitwiseOrMasks(otherCellsMap);
			if ((mask & comparer) != comparer)
			{
				return;
			}

			short otherDigitsMask = (short)(mask ^ comparer);
			foreach (int region in otherCellsMap.CoveredRegions)
			{
				if ((ValueMaps[d1] | ValueMaps[d2]).Overlaps(RegionMaps[region]))
				{
					return;
				}

				var iterationMap = (RegionMaps[region] & EmptyMap) - otherCellsMap;
				for (int size = otherDigitsMask.PopCount() - 1; size < iterationMap.Count; size++)
				{
					foreach (int[] iteratedCells in iterationMap.Offsets.GetSubsets(size))
					{
						short tempMask = 0;
						foreach (int cell in iteratedCells)
						{
							tempMask |= grid.GetCandidates(cell);
						}
						if ((tempMask & comparer) != 0 || tempMask.PopCount() - 1 != size
							|| (tempMask & otherDigitsMask) != otherDigitsMask)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in tempMask)
						{
							foreach (int cell in (iterationMap - iteratedCells) & CandMaps[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var cellOffsets = new List<DrawingInfo>();
						foreach (int cell in urCells)
						{
							if (grid.GetStatus(cell) != Empty)
							{
								cellOffsets.Add(new(0, cell));
							}
						}

						var candidateOffsets = new List<DrawingInfo>();
						foreach (int cell in urCells)
						{
							if (grid.GetStatus(cell) == Empty)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(new((tempMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
								}
							}
						}
						foreach (int cell in iteratedCells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new UrType3StepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Cells = arMode ? cellOffsets : null,
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[] { new(0, region) }
									}
								},
								d1,
								d2,
								urCells,
								arMode,
								otherDigitsMask.GetAllSets().ToArray(),
								iteratedCells,
								region,
								true,
								index));
					}
				}
			}
		}

		/// <summary>
		/// Check type 4.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void CheckType4(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			//  ↓ corner1, corner2
			// (ab ) ab
			//  abx  aby
			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
			{
				return;
			}

			foreach (int region in otherCellsMap.CoveredRegions)
			{
				if (region < 9)
				{
					// Process the case in lines.
					continue;
				}

				foreach (int digit in stackalloc[] { d1, d2 })
				{
					if (!IsConjugatePair(digit, otherCellsMap, region))
					{
						continue;
					}

					// Yes, Type 4 found.
					// Now check elimination.
					int elimDigit = (comparer ^ (1 << digit)).FindFirstSet();
					var elimMap = otherCellsMap & CandMaps[elimDigit];
					if (elimMap.IsEmpty)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap)
					{
						conclusions.Add(new(Elimination, cell, elimDigit));
					}

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in urCells)
					{
						if (grid.GetStatus(cell) != Empty)
						{
							continue;
						}

						if (otherCellsMap[cell])
						{
							if (d1 != elimDigit && grid.Exists(cell, d1) is true)
							{
								candidateOffsets.Add(new(1, cell * 9 + d1));
							}
							if (d2 != elimDigit && grid.Exists(cell, d2) is true)
							{
								candidateOffsets.Add(new(1, cell * 9 + d2));
							}
						}
						else
						{
							// Corner1 and corner2.
							foreach (int d in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(0, cell * 9 + d));
							}
						}
					}

					if (!_allowIncompleteUr && (candidateOffsets.Count, conclusions.Count) != (6, 2))
					{
						continue;
					}

					int[] offsets = otherCellsMap.Offsets;
					accumulator.Add(
						new UrPlusStepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Cells = arMode ? GetHighlightCells(urCells) : null,
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[] { new(0, region) }
								}
							},
							Type4,
							d1,
							d2,
							urCells,
							arMode,
							new ConjugatePair[] { new(offsets[0], offsets[1], digit) },
							index));
				}
			}
		}

		/// <summary>
		/// Check type 5.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void CheckType5(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index)
		{
			//  ↓ cornerCell
			// (ab ) abc
			//  abc  abc
			if (grid.GetCandidates(cornerCell) != comparer)
			{
				return;
			}

			// Get the summary mask.
			if ((grid.BitwiseOrMasks(otherCellsMap) ^ comparer) is var extraMask && extraMask.PopCount() != 1)
			{
				return;
			}

			// Type 5 found. Now check elimination.
			int extraDigit = extraMask.FindFirstSet();
			var conclusions = new List<Conclusion>();
			var cellsThatContainsExtraDigit = otherCellsMap & CandMaps[extraDigit];
			if (cellsThatContainsExtraDigit.HasOnlyOneElement())
			{
				return;
			}

			foreach (int cell in cellsThatContainsExtraDigit.PeerIntersection & CandMaps[extraDigit])
			{
				conclusions.Add(new(Elimination, cell, extraDigit));
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var candidateOffsets = new List<DrawingInfo>();
			foreach (int cell in urCells)
			{
				if (grid.GetStatus(cell) != Empty)
				{
					continue;
				}

				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(digit == extraDigit ? 1 : 0, cell * 9 + digit));
				}
			}
			unsafe
			{
				if (!_allowIncompleteUr && candidateOffsets.Count(&CheckHighlightType) != 8)
				{
					return;
				}
			}

			accumulator.Add(
				new UrType2StepInfo(
					conclusions,
					new View[]
					{
						new()
						{
							Cells = arMode ? GetHighlightCells(urCells) : null,
							Candidates = candidateOffsets
						}
					},
					arMode ? AType5 : Type5,
					d1,
					d2,
					urCells,
					arMode,
					extraDigit,
					index));
		}

		/// <summary>
		/// Check type 6.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void CheckType6(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			//  ↓ corner1
			// (ab )  aby
			//  abx  (ab)
			//        ↑corner2
			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
			{
				return;
			}

			int[] offsets = otherCellsMap.Offsets;
			int o1 = offsets[0], o2 = offsets[1];
			int r1 = RegionLabel.Row.ToRegion(corner1), c1 = RegionLabel.Column.ToRegion(corner1);
			int r2 = RegionLabel.Row.ToRegion(corner2), c2 = RegionLabel.Column.ToRegion(corner2);
			foreach (int digit in stackalloc[] { d1, d2 })
			{
				foreach (var (region1, region2) in stackalloc[] { (r1, r2), (c1, c2) })
				{
					gather(grid, otherCellsMap, region1 is >= 9 and < 18, digit, region1, region2);
				}
			}

			void gather(
				in SudokuGrid grid, in Cells otherCellsMap, bool isRow, int digit, int region1, int region2)
			{
				if ((!isRow
					|| !IsConjugatePair(digit, new() { corner1, o1 }, region1)
					|| !IsConjugatePair(digit, new() { corner2, o2 }, region2))
					&& (isRow
					|| !IsConjugatePair(digit, new() { corner1, o2 }, region1)
					|| !IsConjugatePair(digit, new() { corner2, o1 }, region2)))
				{
					return;
				}

				// Check eliminations.
				var elimMap = otherCellsMap & CandMaps[digit];
				if (elimMap.IsEmpty)
				{
					return;
				}

				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap)
				{
					conclusions.Add(new(Elimination, cell, digit));
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int cell in urCells)
				{
					if (otherCellsMap[cell])
					{
						if (d1 != digit && grid.Exists(cell, d1) is true)
						{
							candidateOffsets.Add(new(0, cell * 9 + d1));
						}
						if (d2 != digit && grid.Exists(cell, d2) is true)
						{
							candidateOffsets.Add(new(0, cell * 9 + d2));
						}
					}
					else
					{
						foreach (int d in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(d == digit ? 1 : 0, cell * 9 + d));
						}
					}
				}

				if (!_allowIncompleteUr && (candidateOffsets.Count, conclusions.Count) != (6, 2))
				{
					return;
				}

				accumulator.Add(
					new UrPlusStepInfo(
						conclusions,
						new View[]
						{
							new()
							{
								Cells = arMode ? GetHighlightCells(urCells) : null,
								Candidates = candidateOffsets,
								Regions = new DrawingInfo[] { new(0, region1), new(0, region2) }
							}
						},
						Type6,
						d1,
						d2,
						urCells,
						false,
						new ConjugatePair[]
						{
							new(corner1, isRow ? o1 : o2, digit),
							new(corner2, isRow ? o2 : o1, digit)
						},
						index));
			}
		}

		/// <summary>
		/// Check hidden UR.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void CheckHidden(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index)
		{
			//  ↓ cornerCell
			// (ab ) abx
			//  aby  abz
			if (grid.GetCandidates(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = otherCellsMap - abzCell;
			int r = RegionLabel.Row.ToRegion(abzCell), c = RegionLabel.Column.ToRegion(abzCell);

			foreach (int digit in stackalloc[] { d1, d2 })
			{
				int[] offsets = adjacentCellsMap.Offsets;
				int abxCell = offsets[0], abyCell = offsets[1];
				Cells map1 = new() { abzCell, abxCell }, map2 = new() { abzCell, abyCell };
				if (!IsConjugatePair(digit, map1, map1.CoveredLine)
					|| !IsConjugatePair(digit, map2, map2.CoveredLine))
				{
					continue;
				}

				// Hidden UR found. Now check eliminations.
				int elimDigit = (comparer ^ (1 << digit)).FindFirstSet();
				if (grid.Exists(abzCell, elimDigit) is not true)
				{
					continue;
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int cell in urCells)
				{
					if (grid.GetStatus(cell) != Empty)
					{
						continue;
					}

					if (otherCellsMap[cell])
					{
						if ((cell != abzCell || d1 != elimDigit) && grid.Exists(cell, d1) is true)
						{
							candidateOffsets.Add(new(d1 != elimDigit ? 1 : 0, cell * 9 + d1));
						}
						if ((cell != abzCell || d2 != elimDigit) && grid.Exists(cell, d2) is true)
						{
							candidateOffsets.Add(new(d2 != elimDigit ? 1 : 0, cell * 9 + d2));
						}
					}
					else
					{
						foreach (int d in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(0, cell * 9 + d));
						}
					}
				}

				if (!_allowIncompleteUr && candidateOffsets.Count != 7)
				{
					continue;
				}

				accumulator.Add(
					new HiddenUrStepInfo(
						new Conclusion[] { new(Elimination, abzCell, elimDigit) },
						new View[]
						{
							new()
							{
								Cells = arMode ? GetHighlightCells(urCells) : null,
								Candidates = candidateOffsets,
								Regions = new DrawingInfo[] { new(0, r), new(0, c) }
							}
						},
						d1,
						d2,
						urCells,
						arMode,
						new ConjugatePair[]
						{
							new(abzCell, abxCell, digit),
							new(abzCell, abyCell, digit),
						},
						index));
			}
		}
	}
}