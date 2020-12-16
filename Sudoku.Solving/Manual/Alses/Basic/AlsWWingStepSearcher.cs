﻿using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Encapsulates an <b>almost locked sets W-Wing</b> (ALS-W-Wing) technique.
	/// </summary>
	public sealed class AlsWWingStepSearcher : AlsStepSearcher
	{
		/// <inheritdoc/>
		public AlsWWingStepSearcher(bool allowOverlapping, bool alsShowRegions, bool allowAlsCycles)
			: base(allowOverlapping, alsShowRegions, allowAlsCycles)
		{
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(62, nameof(TechniqueCode.AlsWWing))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			var alses = Als.GetAllAlses(grid).ToArray();

			// Gather all conjugate pairs.
			var conjugatePairs = new ICollection<ConjugatePair>?[9];
			for (int digit = 0; digit < 9; digit++)
			{
				for (int region = 0; region < 27; region++)
				{
					if ((RegionMaps[region] & CandMaps[digit]) is { Count: 2 } temp)
					{
						(conjugatePairs[digit] ??= new List<ConjugatePair>()).Add(new(temp, digit));
					}
				}
			}

			// Iterate on each ALS.
			for (int i = 0, length = alses.Length; i < length - 1; i++)
			{
				var als1 = alses[i];
				var (_, region1, mask1, map1, _, _) = als1;
				for (int j = i + 1; j < length; j++)
				{
					var als2 = alses[j];
					var (_, region2, mask2, map2, _, _) = als2;
					if (map1.Overlaps(map2) || (map1 | map2).InOneRegion)
					{
						continue;
					}

					var mask = (short)(mask1 & mask2);
					if (mask.PopCount() < 2)
					{
						continue;
					}

					foreach (int x in mask)
					{
						if (conjugatePairs[x] is null or { Count: 0 })
						{
							continue;
						}

						var p1 = (map1 & CandMaps[x]).PeerIntersection & CandMaps[x];
						var p2 = (map2 & CandMaps[x]).PeerIntersection & CandMaps[x];
						if (p1.IsEmpty || p2.IsEmpty)
						{
							// At least one of two ALSes can't see the node of the conjugate pair.
							continue;
						}

						// Iterate on each conjugate pair.
						short wDigitsMask = 0;
						var conclusions = new List<Conclusion>();
						if (conjugatePairs[x] is var conjPairs and not null)
						{
							foreach (var conjugatePair in conjPairs)
							{
								var cpMap = conjugatePair.Map;
								if (cpMap.Overlaps(map1) || cpMap.Overlaps(map2))
								{
									// Conjugate pair can't overlap with the ALS structure.
									continue;
								}

								if ((cpMap & p1).Count != 1 || (cpMap & p2).Count != 1)
								{
									continue;
								}

								if (((p1 | p2) & cpMap).Count != 2)
								{
									continue;
								}

								foreach (int w in mask & ~(1 << x))
								{
									var tempMap = ((map1 | map2) & CandMaps[w]).PeerIntersection & CandMaps[w];
									if (tempMap.IsEmpty)
									{
										continue;
									}

									wDigitsMask |= (short)(1 << w);
									foreach (int cell in tempMap)
									{
										conclusions.Add(new(Elimination, cell, w));
									}
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record highlight cell and candidate offsets.
								var cellOffsets = new List<DrawingInfo>();
								cellOffsets.AddRange(from cell in map1 select new DrawingInfo(-1, cell));
								cellOffsets.AddRange(from cell in map2 select new DrawingInfo(-2, cell));

								int[] offsets = cpMap.Offsets;
								var candidateOffsets = new List<DrawingInfo>
								{
									new(0, offsets[0] * 9 + x),
									new(0, offsets[1] * 9 + x)
								};
								foreach (int cell in map1)
								{
									foreach (int digit in grid.GetCandidates(cell))
									{
										candidateOffsets.Add(
											new(
												true switch
												{
													_ when digit == x => 1,
													_ when (wDigitsMask >> digit & 1) != 0 => 2,
													_ => -1
												},
												cell * 9 + digit));
									}
								}
								foreach (int cell in map2)
								{
									foreach (int digit in grid.GetCandidates(cell))
									{
										candidateOffsets.Add(
											new(
												true switch
												{
													_ when digit == x => 1,
													_ when (wDigitsMask >> digit & 1) != 0 => 2,
													_ => -2
												},
												cell * 9 + digit));
									}
								}

								accumulator.Add(
									new AlsWWingStepInfo(
										conclusions,
										new View[]
										{
											new()
											{
												Cells = AlsShowRegions ? null : cellOffsets,
												Candidates = AlsShowRegions ? candidateOffsets : null,
												Regions = AlsShowRegions
												? new DrawingInfo[]
												{
													new(-1, region1),
													new(-2, region2),
													new(0, conjugatePair.Regions.FindFirstSet())
												}
												: null
											}
										},
										als1,
										als2,
										conjugatePair,
										wDigitsMask,
										x));
							}
						}
					}
				}
			}
		}
	}
}