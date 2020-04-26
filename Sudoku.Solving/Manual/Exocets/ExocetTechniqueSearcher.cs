﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Utils;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.GridMap.InitializeOption;
using static Sudoku.GridProcessings;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// <para>Encapsulates an <b>exocet</b> technique searcher.</para>
	/// <para>
	/// The pattern will be like:
	/// <code>
	/// .-------.-------.-------.<br/>
	/// | B B * | * . . | * . . | B = Base Cells<br/>
	/// | . . * | Q . . | R . . | Q = 1st Object Pair<br/>
	/// | . . * | Q . . | R . . | R = 2nd Object Pair<br/>
	/// :-------+-------+-------: S = Cross-line Cells<br/>
	/// | . . S | S . . | S . . | * = Escape Cells<br/>
	/// | . . S | S . . | S . . |<br/>
	/// | . . S | S . . | S . . |<br/>
	/// :-------+-------+-------:<br/>
	/// | . . S | S . . | S . . |<br/>
	/// | . . S | S . . | S . . |<br/>
	/// | . . S | S . . | S . . |<br/>
	/// '-------'-------'-------'
	/// </code>
	/// In the data structure, all letters will be used as the same one in this exemplar.
	/// In addition, if as senior exocet, one of two target cells will lie on cross-line cells,
	/// and the lines of two target cells lying on cannot contain any base digits.
	/// </para>
	/// </summary>
	[TechniqueDisplay("Exocet")]
	public sealed partial class ExocetTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// Indicates all exocet patterns to iterate on.
		/// </summary>
		private static readonly Exocet[] Exocets;


		/// <summary>
		/// Indicates whether the searcher will find advanced eliminations.
		/// </summary>
		private readonly bool _checkAdvanced;


		/// <summary>
		/// Initializes an instance with the specified region maps.
		/// </summary>
		/// <param name="checkAdvanced">
		/// Indicates whether the searcher will find advanced eliminations.
		/// </param>
		public ExocetTechniqueSearcher(bool checkAdvanced) => _checkAdvanced = checkAdvanced;


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 94;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var mirror1 = (Span<int>)stackalloc int[2];
			var mirror2 = (Span<int>)stackalloc int[2];
			var modes = (Span<bool>)stackalloc[] { true, false };
			var emptyCellsMap = GetEmptyCellsMap(grid);
			var digitDistributions = GetDigitDistributions(grid);
			foreach (var exocet in Exocets)
			{
				var (baseMap, targetMap, _) = exocet;
				var (b1, b2, tq1, tq2, tr1, tr2, s, mq1, mq2, mr1, mr2) = exocet;
				foreach (bool jeMode in modes)
				{
					// The base cells cannot be given or modifiable.
					if ((baseMap - emptyCellsMap).IsNotEmpty)
					{
						continue;
					}

					// The number of different candidates in base cells cannot be greater than 5.
					short m1 = grid.GetCandidatesReversal(b1);
					short m2 = grid.GetCandidatesReversal(b2);
					short baseCandidatesMask = (short)(m1 | m2);
					if (baseCandidatesMask.CountSet() > 5)
					{
						continue;
					}

					// At least one cell should be empty.
					if (grid.GetCellStatus(tq1) != Empty && grid.GetCellStatus(tq2) != Empty
						&& grid.GetCellStatus(tr1) != Empty && grid.GetCellStatus(tr2) != Empty)
					{
						continue;
					}

					if (!CheckTarget(grid, tq1, tq2, baseCandidatesMask, out short nonBaseQ, out _)
						|| !CheckTarget(grid, tr1, tr2, baseCandidatesMask, out short nonBaseR, out _))
					{
						continue;
					}

					// Get all locked members.
					(mirror1[0], mirror1[1]) = (mq1.SetAt(0), mq1.SetAt(1));
					(mirror2[0], mirror2[1]) = (mq2.SetAt(0), mq2.SetAt(1));
					short temp = (short)(
						(short)(grid.GetCandidatesReversal(mirror1[0]) | grid.GetCandidatesReversal(mirror1[1]))
						| (short)(grid.GetCandidatesReversal(mirror2[0]) | grid.GetCandidatesReversal(mirror2[1])));
					short needChecking = (short)(baseCandidatesMask & temp);
					short lockedMemberQ = (short)(baseCandidatesMask & ~needChecking);

					(mirror1[0], mirror1[1]) = (mr1.SetAt(0), mr1.SetAt(1));
					(mirror2[0], mirror2[1]) = (mr2.SetAt(0), mr2.SetAt(1));
					temp = (short)(
						(short)(grid.GetCandidatesReversal(mirror1[0]) | grid.GetCandidatesReversal(mirror1[1]))
						| (short)(grid.GetCandidatesReversal(mirror2[0]) | grid.GetCandidatesReversal(mirror2[1])));
					needChecking &= temp;
					short lockedMemberR = (short)(baseCandidatesMask & ~(baseCandidatesMask & temp));
					if (!CheckCrossline(s, needChecking, digitDistributions))
					{
						continue;
					}

					// Record highlight cells and candidates.
					var cellOffsets = new List<(int, int)> { (0, b1), (0, b2) };
					var candidateOffsets = new List<(int, int)>();
					foreach (int digit in grid.GetCandidatesReversal(b1).GetAllSets())
					{
						candidateOffsets.Add((0, b1 * 9 + digit));
					}
					foreach (int digit in grid.GetCandidatesReversal(b2).GetAllSets())
					{
						candidateOffsets.Add((0, b2 * 9 + digit));
					}

					// Check target eliminations.
					// '|' first, '&&' second. (Do you know my mean?)
					var targetElims = new TargetEliminations();
					temp = (short)(nonBaseQ > 0 ? baseCandidatesMask | nonBaseQ : baseCandidatesMask);
					if (recordTargetEliminations(tq1) | recordTargetEliminations(tq2)
						&& nonBaseQ != 0 && grid.GetCellStatus(tq1) == Empty && grid.GetCellStatus(tq2) == Empty)
					{
						int conjugatPairDigit = nonBaseQ.FindFirstSet();
						if (grid.Exists(tq1, conjugatPairDigit) is true)
						{
							candidateOffsets.Add((1, tq1 * 9 + conjugatPairDigit));
						}
						if (grid.Exists(tq2, conjugatPairDigit) is true)
						{
							candidateOffsets.Add((1, tq2 * 9 + conjugatPairDigit));
						}
					}
					temp = (short)(nonBaseR > 0 ? baseCandidatesMask | nonBaseR : baseCandidatesMask);
					if (recordTargetEliminations(tr1) | recordTargetEliminations(tr2)
						&& nonBaseR != 0 && grid.GetCellStatus(tr1) == Empty && grid.GetCellStatus(tr2) == Empty)
					{
						int conjugatPairDigit = nonBaseR.FindFirstSet();
						if (grid.Exists(tr1, conjugatPairDigit) is true)
						{
							candidateOffsets.Add((1, tr1 * 9 + conjugatPairDigit));
						}
						if (grid.Exists(tr2, conjugatPairDigit) is true)
						{
							candidateOffsets.Add((1, tr2 * 9 + conjugatPairDigit));
						}
					}

					bool isRow = new GridMap(stackalloc[] { b1, b2 }).CoveredLine < 18;
					if (jeMode)
					{
						var (tar1, mir1) = recordMirrorEliminations(tq1, tq2, tr1, tr2, mq1, mq2, nonBaseQ, 0);
						var (tar2, mir2) = recordMirrorEliminations(tr1, tr2, tq1, tq2, mr1, mr2, nonBaseR, 1);
						var targetEliminations = TargetEliminations.MergeAll(targetElims, tar1, tar2);
						var mirrorEliminations = MirrorEliminations.MergeAll(mir1, mir2);
						var bibiEliminations = new BibiPatternEliminations();
						var targetPairEliminations = new TargetPairEliminations();
						var swordfishEliminations = new SwordfishEliminations();
						if (_checkAdvanced && baseCandidatesMask.CountSet() > 2)
						{
							CheckBibiPattern(
								grid, baseCandidatesMask, b1, b2, tq1, tq2, tr1, tr2, s,
								isRow, out bibiEliminations, out targetPairEliminations,
								out swordfishEliminations);
						}

						if (_checkAdvanced && targetEliminations.Count == 0 && mirrorEliminations.Count == 0
							&& bibiEliminations.Count == 0
							|| !_checkAdvanced && targetEliminations.Count == 0)
						{
							continue;
						}

						cellOffsets.AddRange(new[] { (1, tq1), (1, tq2), (1, tr1), (1, tr2) });
						foreach (int cell in s.Offsets)
						{
							cellOffsets.Add((2, cell));
						}

						accumulator.Add(
							new JuniorExocetTechniqueInfo(
								conclusions: new List<Conclusion>(),
								views: new[]
								{
									new View(
										cellOffsets,
										candidateOffsets,
										regionOffsets: null,
										links: null)
								},
								exocet,
								digits: baseCandidatesMask.GetAllSets(),
								lockedMemberQ: lockedMemberQ == 0 ? null : lockedMemberQ.GetAllSets(),
								lockedMemberR: lockedMemberR == 0 ? null : lockedMemberR.GetAllSets(),
								targetEliminations,
								mirrorEliminations: _checkAdvanced ? mirrorEliminations : new MirrorEliminations(),
								bibiEliminations,
								targetPairEliminations,
								swordfishEliminations));
					}
					else
					{
						// TODO: Check senior exocet.
					}

					bool recordTargetEliminations(int cell)
					{
						short candidateMask = (short)(grid.GetCandidatesReversal(cell) & ~temp);
						if (grid.GetCellStatus(cell) == Empty && candidateMask != 0
							&& (grid.GetCandidatesReversal(cell) & baseCandidatesMask) != 0)
						{
							foreach (int digit in candidateMask.GetAllSets())
							{
								targetElims.Add(new Conclusion(Elimination, cell, digit));
							}

							return true;
						}

						return false;
					}

					(TargetEliminations, MirrorEliminations) recordMirrorEliminations(
						int tq1, int tq2, int tr1, int tr2, GridMap m1, GridMap m2,
						short lockedNonTarget, int x)
					{
						if ((grid.GetCandidatesReversal(tq1) & baseCandidatesMask) != 0)
						{
							short mask1 = grid.GetCandidatesReversal(tr1);
							short mask2 = grid.GetCandidatesReversal(tr2);
							var (target, target2, mirror) = (tq1, tq2, m1);
							return CheckMirror(
								grid,
								target,
								target2,
								lockedNonTarget: lockedNonTarget > 0 ? lockedNonTarget : (short)0,
								baseCandidatesMask,
								mirror,
								digitDistributions,
								x,
								onlyOne:
									(mask1 & baseCandidatesMask) != 0 && (mask2 & baseCandidatesMask) == 0
										? tr1
										: (mask1 & baseCandidatesMask) == 0 && (mask2 & baseCandidatesMask) != 0
											? tr2
											: -1,
								cellOffsets,
								candidateOffsets);
						}
						else if ((grid.GetCandidatesReversal(tq2) & baseCandidatesMask) != 0)
						{
							short mask1 = grid.GetCandidatesReversal(tq1);
							short mask2 = grid.GetCandidatesReversal(tq2);
							var (target, target2, mirror) = (tq2, tq1, m2);
							return CheckMirror(
								grid,
								target,
								target2,
								lockedNonTarget: lockedNonTarget > 0 ? lockedNonTarget : (short)0,
								baseCandidatesMask,
								mirror,
								digitDistributions,
								x,
								onlyOne:
									(mask1 & baseCandidatesMask) != 0 && (mask2 & baseCandidatesMask) == 0
										? tr1
										: (mask1 & baseCandidatesMask) == 0 && (mask2 & baseCandidatesMask) != 0
											? tr2
											: -1,
								cellOffsets,
								candidateOffsets);
						}
						else
						{
							return (new TargetEliminations(), new MirrorEliminations());
						}
					}
				}
			}
		}

		/// <summary>
		/// Check the cross-line cells.
		/// </summary>
		/// <param name="crossline">The cross line cells.</param>
		/// <param name="digitsNeedChecking">The digits that need checking.</param>
		/// <param name="digitDistributions">All digit distributions.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the structure passed the validation.
		/// </returns>
		private bool CheckCrossline(GridMap crossline, short digitsNeedChecking, GridMap[] digitDistributions)
		{
			foreach (int digit in digitsNeedChecking.GetAllSets())
			{
				var crosslinePerCandidate = crossline & digitDistributions[digit];
				int r = crosslinePerCandidate.RowMask, c = crosslinePerCandidate.ColumnMask;
				if (r.CountSet() <= 2 || c.CountSet() <= 2)
				{
					continue;
				}

				bool flag = false;
				foreach (int d1 in r.GetAllSets())
				{
					foreach (int d2 in c.GetAllSets())
					{
						if ((crosslinePerCandidate - (RegionMaps[d1 + 9] | RegionMaps[d2 + 18])).IsEmpty)
						{
							flag = true;
							break;
						}
					}

					if (flag)
					{
						break;
					}
				}

				if (!flag)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Check the target cells.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="pos1">The cell 1 to determine.</param>
		/// <param name="pos2">The cell 2 to determine.</param>
		/// <param name="baseCandidatesMask">The base candidate mask.</param>
		/// <param name="otherCandidatesMask">
		/// (<see langword="out"/> parameter) The other candidate mask.
		/// </param>
		/// <param name="otherRegion">
		/// (<see langword="out"/> parameter) The other region.
		/// </param>
		/// <returns>The <see cref="bool"/> value.</returns>
		private bool CheckTarget(
			IReadOnlyGrid grid, int pos1, int pos2, int baseCandidatesMask,
			out short otherCandidatesMask, out int otherRegion)
		{
			otherRegion = -1;
			otherCandidatesMask = -1;

			short m1 = grid.GetCandidatesReversal(pos1);
			short m2 = grid.GetCandidatesReversal(pos2);
			if ((baseCandidatesMask & m1) != 0 ^ (baseCandidatesMask & m2) != 0)
			{
				// One cell contains the digit that base candidate holds,
				// and another one does not contain.
				return true;
			}

			if ((m1 & baseCandidatesMask) == 0 && (m2 & baseCandidatesMask) == 0
				|| (m1 & ~baseCandidatesMask) == 0 && (m2 & ~baseCandidatesMask) == 0)
			{
				// Two cells don't contain any digits in the base cells neither,
				// or both contains only digits from base cells,
				// which is not allowed in the exocet rule (or does not contain
				// any eliminations).
				return false;
			}

			// Now we check the special cases, in other words, the last two cells both contain
			// digits from base cells, and at least one cell contains non-base digits.
			// Therefore, we should check on non-base digits, whether the non-base digits
			// covers only one of two last cells; otherwise, false.
			short candidatesMask = (short)((m1 | m2) & ~baseCandidatesMask);
			var (r1, c1, b1) = CellUtils.GetRegion(pos1);
			var (r2, c2, b2) = CellUtils.GetRegion(pos2);
			var span = (Span<int>)stackalloc[] { b1, r1 == r2 ? r1 + 9 : c1 + 18 };
			foreach (short mask in GetCombinations(candidatesMask))
			{
				for (int i = 0; i < 2; i++)
				{
					int count = 0;
					for (int j = 0; j < 9; j++)
					{
						int p = RegionCells[span[i]][j];
						if (p == pos1 || p == pos2 || grid.GetCellStatus(p) != Empty
							|| (grid.GetCandidatesReversal(p) & mask) == 0)
						{
							continue;
						}

						count++;
					}

					if (count == mask.CountSet() - 1)
					{
						for (int j = 0; j < 9; j++)
						{
							int p = RegionCells[span[i]][j];
							if (grid.GetCellStatus(p) != Empty || (grid.GetCandidatesReversal(p) & mask) == 0)
							{
								continue;
							}

							if ((grid.GetCandidatesReversal(p) & ~mask) == 0
								|| p == pos1 || p == pos2)
							{
								continue;
							}
						}

						otherCandidatesMask = mask;
						otherRegion = span[i];
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Check Bi-bi pattern eliminations.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="baseCandidatesMask">The base candidate mask.</param>
		/// <param name="b1">The base cell 1.</param>
		/// <param name="b2">The base cell 2.</param>
		/// <param name="tq1">The target Q1 cell.</param>
		/// <param name="tq2">The target Q2 cell.</param>
		/// <param name="tr1">The target R1 cell.</param>
		/// <param name="tr2">The target R2 cell.</param>
		/// <param name="crossline">The cross-line cells.</param>
		/// <param name="isRow">
		/// Indicates whether the specified exocet is in the horizontal direction.
		/// </param>
		/// <param name="bibiElims">
		/// (<see langword="out"/> parameter) The Bi-bi pattern eliminations.
		/// </param>
		/// <param name="targetPairElims">
		/// (<see langword="out"/> parameter) The target pair eliminations.
		/// </param>
		/// <param name="swordfishElims">
		/// (<see langword="out"/> parameter) The swordfish eliminations.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating whether the pattern exists.</returns>
		private bool CheckBibiPattern(
			IReadOnlyGrid grid, short baseCandidatesMask, int b1, int b2,
			int tq1, int tq2, int tr1, int tr2, GridMap crossline, bool isRow,
			out BibiPatternEliminations bibiElims, out TargetPairEliminations targetPairElims,
			out SwordfishEliminations swordfishElims)
		{
			bibiElims = new BibiPatternEliminations();
			targetPairElims = new TargetPairEliminations();
			swordfishElims = new SwordfishEliminations();
			var playground = (Span<short>)stackalloc short[3];
			(_, _, int block) = CellUtils.GetRegion(b1);
			short[] temp = new short[4];
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					int p = RegionCells[BibiIter[block, j]][i];
					if (grid.GetCandidatesReversal(p).CountSet() != 1)
					{
						continue;
					}

					temp[j] |= grid.GetCandidatesReversal(p);
				}
			}

			short commonCandidatesMask = (short)((temp[0] | temp[3]) & (temp[1] | temp[2]) & baseCandidatesMask);
			playground[1] = (short)(temp[0] & temp[3] & baseCandidatesMask & ~commonCandidatesMask & baseCandidatesMask);
			playground[2] = (short)(temp[1] & temp[2] & baseCandidatesMask & ~commonCandidatesMask & baseCandidatesMask);

			if (playground[1] == 0 || playground[2] == 0)
			{
				// Contains no Bi-bi pattern.
				return false;
			}

			var dic = new Dictionary<int, short>
			{
				[b1] = grid.GetCandidatesReversal(b1),
				[b2] = grid.GetCandidatesReversal(b2)
			};
			for (int i = 1; i <= 2; i++)
			{
				for (int j = 1; j <= 2; j++)
				{
					var (pos1, pos2) = j == 1 ? (b1, b2) : (b2, b1);
					short ck = (short)(grid.GetCandidatesReversal(pos1) & playground[i]);
					if (ck.CountSet() > 1
						|| (grid.GetCandidatesReversal(pos1) & ~(ck | playground[i == 1 ? 2 : 1])) != 0)
					{
						continue;
					}

					short candidateMask = ck != 0 ? ck : playground[i];
					candidateMask &= grid.GetCandidatesReversal(pos2);
					if (candidateMask == 0)
					{
						continue;
					}

					foreach (int digit in candidateMask.GetAllSets())
					{
						bibiElims.Add(new Conclusion(Elimination, pos2, digit));
						dic[pos2] &= (short)~(1 << digit);
					}
				}
			}

			// Now check all base digits last.
			short last = (short)(dic[b1] | dic[b2]);
			foreach (int digit in (511 & ~last).GetAllSets())
			{
				if (grid.Exists(tq1, digit) is true)
				{
					bibiElims.Add(new Conclusion(Elimination, tq1, digit));
				}
				if (grid.Exists(tq2, digit) is true)
				{
					bibiElims.Add(new Conclusion(Elimination, tq2, digit));
				}
				if (grid.Exists(tr1, digit) is true)
				{
					bibiElims.Add(new Conclusion(Elimination, tr1, digit));
				}
				if (grid.Exists(tr2, digit) is true)
				{
					bibiElims.Add(new Conclusion(Elimination, tr2, digit));
				}
			}

			// Then check target pairs if worth.
			if (last.CountSet() == 2)
			{
				var elimMap = new GridMap(
					from cell in new[] { tq1, tq2, tr1, tr2 }
					where grid.GetCellStatus(cell) == Empty
					select cell, ProcessPeersWithoutItself);

				if (elimMap.Count == 0)
				{
					// Exit the method.
					return true;
				}

				var digits = last.GetAllSets();
				foreach (int cell in elimMap.Offsets)
				{
					foreach (int digit in digits)
					{
						if (grid.Exists(cell, digit) is true)
						{
							targetPairElims.Add(new Conclusion(Elimination, cell, digit));
						}
					}
				}
				elimMap = new GridMap(stackalloc[] { b1, b2 }, ProcessPeersWithoutItself);
				if (elimMap.Count == 0)
				{
					return true;
				}

				foreach (int cell in elimMap.Offsets)
				{
					foreach (int digit in digits)
					{
						if (grid.Exists(cell, digit) is true)
						{
							targetPairElims.Add(new Conclusion(Elimination, cell, digit));
						}
					}
				}

				// Then check swordfish pattern.
				foreach (int digit in digits)
				{
					short mask = isRow ? crossline.RowMask : crossline.ColumnMask;
					foreach (int offset in mask.GetAllSets())
					{
						int region = offset + (isRow ? 9 : 18);
						var map = crossline & RegionMaps[region];
						if (map.Offsets.All(c => !(grid.Exists(c, digit) is true)))
						{
							continue;
						}

						foreach (int cell in RegionCells[region])
						{
							if (crossline[cell] || !(grid.Exists(cell, digit) is true))
							{
								continue;
							}

							swordfishElims.Add(new Conclusion(Elimination, cell, digit));
						}
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Check mirror eliminations.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="target">The target cell.</param>
		/// <param name="target2">
		/// The another target cell that is adjacent with <paramref name="target"/>.
		/// </param>
		/// <param name="lockedNonTarget">The locked member that is non-target digits.</param>
		/// <param name="baseCandidateMask">The base candidate mask.</param>
		/// <param name="mirror">The mirror map.</param>
		/// <param name="digitDistributions">The digit distributions.</param>
		/// <param name="x">The x.</param>
		/// <param name="onlyOne">The only one cell.</param>
		/// <param name="cellOffsets">The cell offsets.</param>
		/// <param name="candidateOffsets">The candidate offsets.</param>
		/// <returns>All mirror eliminations.</returns>
		private (TargetEliminations, MirrorEliminations) CheckMirror(
			IReadOnlyGrid grid, int target, int target2, short lockedNonTarget, short baseCandidateMask,
			GridMap mirror, GridMap[] digitDistributions, int x, int onlyOne,
			IList<(int, int)> cellOffsets, IList<(int, int)> candidateOffsets)
		{
			var targetElims = new TargetEliminations();
			var mirrorElims = new MirrorEliminations();
			var playground = (Span<int>)stackalloc[] { mirror.SetAt(0), mirror.SetAt(1) };
			short mirrorCandidatesMask = (short)(
				grid.GetCandidatesReversal(playground[0]) | grid.GetCandidatesReversal(playground[1]));
			short commonBase = (short)(
				mirrorCandidatesMask & baseCandidateMask & grid.GetCandidatesReversal(target));
			short targetElimination = (short)(grid.GetCandidatesReversal(target) & ~(short)(commonBase | lockedNonTarget));
			if (targetElimination != 0
				&& grid.GetCellStatus(target) != Empty ^ grid.GetCellStatus(target2) != Empty)
			{
				foreach (int digit in targetElimination.GetAllSets())
				{
					targetElims.Add(new Conclusion(Elimination, target, digit));
				}
			}

			if (_checkAdvanced)
			{
				var regions = (Span<int>)stackalloc int[2];
				short m1 = (short)(grid.GetCandidatesReversal(playground[0]) & baseCandidateMask);
				short m2 = (short)(grid.GetCandidatesReversal(playground[1]) & baseCandidateMask);
				if (m1 != 0 ^ m2 != 0)
				{
					int p = playground[m1 != 0 ? 0 : 1];
					short candidateMask = (short)(grid.GetCandidatesReversal(p) & ~commonBase);
					if (candidateMask != 0
						&& grid.GetCellStatus(target) != Empty ^ grid.GetCellStatus(target2) != Empty)
					{
						cellOffsets.Add((3, playground[0]));
						cellOffsets.Add((3, playground[1]));
						foreach (int digit in candidateMask.GetAllSets())
						{
							mirrorElims.Add(new Conclusion(Elimination, p, digit));
						}
					}

					return (targetElims, mirrorElims);
				}

				short nonBase = (short)(mirrorCandidatesMask & ~baseCandidateMask);
				var (r1, c1, b1) = CellUtils.GetRegion(playground[0]);
				var (r2, c2, b2) = CellUtils.GetRegion(playground[1]);
				(regions[0], regions[1]) = (b1, r1 == r2 ? r1 + 9 : c1 + 18);
				short locked = default;
				foreach (short mask in GetCombinations(nonBase))
				{
					for (int i = 0; i < 2; i++)
					{
						int count = 0;
						for (int j = 0; j < 9; j++)
						{
							int p = RegionCells[regions[i]][j];
							if (p == playground[0] || p == playground[1] || p == onlyOne)
							{
								continue;
							}

							if ((grid.GetCandidatesReversal(p) & mask) != 0)
							{
								count++;
							}
						}

						if (count == mask.CountSet() - 1)
						{
							for (int j = 0; j < 9; j++)
							{
								int p = RegionCells[regions[i]][j];
								if ((grid.GetCandidatesReversal(p) & mask) == 0
									|| grid.GetCellStatus(p) != Empty || p == onlyOne
									|| p == playground[0] || p == playground[1]
									|| (grid.GetCandidatesReversal(p) & ~mask) == 0)
								{
									continue;
								}
							}

							locked = mask;
							break;
						}
					}

					if (locked != 0)
					{
						// Here you should use '|' operator rather than '||'.
						// Operator '||' will not execute the second method if the first condition is true.
						if (record(playground, 0) | record(playground, 1))
						{
							cellOffsets.Add((3, playground[0]));
							cellOffsets.Add((3, playground[1]));
						}

						short mask1 = grid.GetCandidatesReversal(playground[0]);
						short mask2 = grid.GetCandidatesReversal(playground[1]);
						if (locked.CountSet() == 1 && (mask1 & locked) != 0 ^ (mask2 & locked) != 0)
						{
							short candidateMask = (short)(
								~(
									grid.GetCandidatesReversal(
										playground[(mask1 & locked) != 0 ? 1 : 0]
									) & grid.GetCandidatesReversal(target) & baseCandidateMask
								) & grid.GetCandidatesReversal(target) & baseCandidateMask);
							if (candidateMask != 0)
							{
								foreach (int digit in candidateMask.GetAllSets())
								{
									mirrorElims.Add(new Conclusion(Elimination, target, digit));
								}
							}
						}

						break;

						bool record(Span<int> playground, int i)
						{
							short candidateMask = (short)(
								grid.GetCandidatesReversal(playground[i]) & ~(baseCandidateMask | locked));
							if (candidateMask != 0)
							{
								foreach (int digit in locked.GetAllSets())
								{
									if (grid.Exists(playground[i], digit) is true)
									{
										candidateOffsets.Add((1, playground[i] * 9 + digit));
									}
								}

								foreach (int digit in candidateMask.GetAllSets())
								{
									mirrorElims.Add(new Conclusion(Elimination, playground[i], digit));
								}

								return true;
							}

							return false;
						}
					}
				}
			}

			return (targetElims, mirrorElims);
		}


		/// <summary>
		/// Get the map of empty cells.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The map.</returns>
		private static GridMap GetEmptyCellsMap(IReadOnlyGrid grid)
		{
			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (grid.GetCellStatus(cell) == Empty)
				{
					result.Add(cell);
				}
			}

			return result;
		}

		/// <summary>
		/// Get all distributions for digits.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The digit distributions.</returns>
		private static GridMap[] GetDigitDistributions(IReadOnlyGrid grid)
		{
			var digitDistributions = new GridMap[9];
			for (int digit = 0; digit < 9; digit++)
			{
				ref var map = ref digitDistributions[digit];
				for (int cell = 0; cell < 81; cell++)
				{
					if ((grid.GetCandidates(cell) >> digit & 1) == 0)
					{
						map.Add(cell);
					}
				}
			}

			return digitDistributions;
		}

		/// <summary>
		/// Get all combinations that contains all set bits from the specified number.
		/// </summary>
		/// <param name="seed">The specified number.</param>
		/// <returns>All combinations.</returns>
		private static IEnumerable<short> GetCombinations(short seed)
		{
			for (int i = 0; i < 9; i++)
			{
				foreach (short mask in new BitCombinationGenerator(9, i))
				{
					if ((mask & seed) == mask)
					{
						yield return mask;
					}
				}
			}
		}
	}
}
