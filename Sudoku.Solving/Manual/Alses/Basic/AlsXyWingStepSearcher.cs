using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Techniques;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Encapsulates an <b>almost locked sets XY-Wing</b> (ALS-XY-Wing) technique.
	/// </summary>
	public sealed class AlsXyWingStepSearcher : AlsStepSearcher
	{
		/// <inheritdoc/>
		public AlsXyWingStepSearcher(bool allowOverlapping, bool alsShowRegions, bool allowAlsCycles)
			: base(allowOverlapping, alsShowRegions, allowAlsCycles)
		{
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(29, nameof(Technique.AlsXyWing))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			var rccs = new List<(Als Left, Als Right, short Mask)>();
			var alses = Als.GetAllAlses(grid);

			// Gather all RCCs.
			for (int i = 0, length = alses.Length; i < length - 1; i++)
			{
				var als1 = alses[i];
				var (_, mask1, map1) = als1;
				for (int j = i + 1; j < length; j++)
				{
					var als2 = alses[j];
					var (_, mask2, map2) = als2;
					var map = map1 | map2;
					if (map.InOneRegion || !(map1 & map2).IsEmpty)
					{
						continue;
					}

					if ((mask1 & mask2) is var mask and not 0)
					{
						short rccMask = 0;
						foreach (int digit in mask)
						{
							if ((map & CandMaps[digit]).InOneRegion)
							{
								rccMask |= (short)(1 << digit);
							}
						}
						if (rccMask == 0)
						{
							continue;
						}

						rccs.Add((als1, als2, rccMask));
					}
				}
			}

			// Now check them.
			for (int i = 0, count = rccs.Count; i < count - 1; i++)
			{
				var (als11, als12, mask1) = rccs[i];
				for (int j = i + 1; j < count; j++)
				{
					var (als21, als22, mask2) = rccs[j];
					if (mask1 == mask2 && mask1 != 0 && (mask1 & mask1 - 1) == 0
						&& mask2 != 0 && (mask2 & mask2 - 1) == 0)
					{
						// Cannot form a XY-Wing.
						continue;
					}

					if (!(als11 == als21 ^ als12 == als22 || als11 == als22 ^ als12 == als21))
					{
						continue;
					}

					// Get the logical order of three ALSes.
					var (a, b, c) = true switch
					{
						_ when als11 == als21 => (als12, als22, als11),
						_ when als11 == als22 => (als12, als21, als11),
						_ when als12 == als21 => (als11, als22, als12),
						_ => (als11, als21, als12)
					};

					var (_, aRegion, aMask, aMap, _, _) = a;
					var (_, bRegion, bMask, bMap, _, _) = b;
					var (_, cRegion, _, cMap, _, _) = c;
					var map = aMap | bMap;
					if (map == aMap || map == bMap)
					{
						continue;
					}

					if (!AllowOverlapping
						&& (!(aMap & bMap).IsEmpty || !(aMap & cMap).IsEmpty || !(bMap & cMap).IsEmpty))
					{
						continue;
					}

					foreach (int digit1 in mask1)
					{
						foreach (int digit2 in mask2)
						{
							if (digit1 == digit2)
							{
								continue;
							}

							short finalX = (short)(1 << digit1);
							short finalY = (short)(1 << digit2);
							short digitsMask = (short)(aMask & bMask & ~(finalX | finalY));
							if (digitsMask == 0)
							{
								continue;
							}

							// Gather eliminations.
							short finalZ = 0;
							var conclusions = new List<Conclusion>();
							foreach (int digit in digitsMask)
							{
								var elimMap = (
									((aMap | bMap) & CandMaps[digit]).PeerIntersection
									& CandMaps[digit]) - (aMap | bMap | cMap);
								if (elimMap.IsEmpty)
								{
									continue;
								}

								finalZ |= (short)(1 << digit);
								foreach (int cell in elimMap)
								{
									conclusions.Add(new(ConclusionType.Elimination, cell, digit));
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							// Record highlight candidates and cells.
							var cellOffsets = new List<PaintingPair<int>>();
							var candidateOffsets = new List<PaintingPair<int>>();
							foreach (int cell in aMap)
							{
								short mask = grid.GetCandidates(cell);
								short alsDigitsMask = (short)(mask & ~(finalX | finalZ));
								short xDigitsMask = (short)(mask & (finalX));
								short zDigitsMask = (short)(mask & finalZ);
								foreach (int digit in alsDigitsMask)
								{
									candidateOffsets.Add(new(-1, cell * 9 + digit));
								}
								foreach (int digit in xDigitsMask)
								{
									candidateOffsets.Add(new(1, cell * 9 + digit));
								}
								foreach (int digit in zDigitsMask)
								{
									candidateOffsets.Add(new(2, cell * 9 + digit));
								}
							}
							foreach (int cell in bMap)
							{
								short mask = grid.GetCandidates(cell);
								short alsDigitsMask = (short)(mask & ~(finalY | finalZ));
								short yDigitsMask = (short)(mask & finalY);
								short zDigitsMask = (short)(mask & finalZ);
								foreach (int digit in alsDigitsMask)
								{
									candidateOffsets.Add(new(-1, cell * 9 + digit));
								}
								foreach (int digit in yDigitsMask)
								{
									candidateOffsets.Add(new(1, cell * 9 + digit));
								}
								foreach (int digit in zDigitsMask)
								{
									candidateOffsets.Add(new(2, cell * 9 + digit));
								}
							}
							foreach (int cell in cMap)
							{
								short mask = grid.GetCandidates(cell);
								short alsDigitsMask = (short)(mask & ~(finalX | finalY));
								short xyDigitsMask = (short)(mask & (finalX | finalY));
								foreach (int digit in alsDigitsMask)
								{
									candidateOffsets.Add(new(-3, cell * 9 + digit));
								}
								foreach (int digit in xyDigitsMask)
								{
									candidateOffsets.Add(new(1, cell * 9 + digit));
								}
							}

							accumulator.Add(
								new AlsXyWingStepInfo(
									conclusions,
									new PresentationData[]
									{
										new()
										{
											Cells = AlsShowRegions ? null : cellOffsets,
											Candidates =AlsShowRegions ? candidateOffsets : null,
											Regions = AlsShowRegions
											? new PaintingPair<int>[]
											{
												new(-1, aRegion), new(-2, bRegion), new(-3, cRegion)
											}
											: null
										}
									},
									a,
									b,
									c,
									finalX,
									finalY,
									finalZ));
						}
					}
				}
			}
		}
	}
}
