﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializeOption;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Encapsulates an <b>almost locked sets W-Wing</b> (ALS-W-Wing) technique.
	/// </summary>
	[TechniqueDisplay("Almost Locked Sets W-Wing")]
	public sealed class AlsWWingTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <inheritdoc/>
		public AlsWWingTechniqueSearcher(bool allowOverlapping, bool alsShowRegions, bool allowAlsCycles)
			: base(allowOverlapping, alsShowRegions, allowAlsCycles)
		{
		}


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 62;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var alses = Als.GetAllAlses(grid).ToArray();
			var candsMap = grid.GetCandidatesMap();

			// Gather all conjugate pairs.
			var conjugatePairs = new ICollection<ConjugatePair>?[9];
			for (int digit = 0; digit < 9; digit++)
			{
				for (int region = 0; region < 27; region++)
				{
					var temp = RegionMaps[region] & candsMap[digit];
					if (temp.Count != 2)
					{
						continue;
					}

					(conjugatePairs[digit] ??= new List<ConjugatePair>()).Add(new ConjugatePair(temp, digit));
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
					if (map1.Overlaps(map2) || (map1 | map2).AllSetsAreInOneRegion(out _))
					{
						continue;
					}

					short mask = (short)(mask1 & mask2);
					if (mask.CountSet() < 2)
					{
						continue;
					}

					foreach (int x in mask.GetAllSets())
					{
						if ((conjugatePairs[x]?.Count ?? 0) == 0)
						{
							continue;
						}

						var p1 = new GridMap(map1 & candsMap[x], ProcessPeersWithoutItself) & candsMap[x];
						var p2 = new GridMap(map2 & candsMap[x], ProcessPeersWithoutItself) & candsMap[x];
						if (p1.IsEmpty || p2.IsEmpty)
						{
							// At least one of two ALSes cannot see the node of the conjugate pair.
							continue;
						}

						// Iterate on each conjugate pair.
						short wDigitsMask = 0;
						var conclusions = new List<Conclusion>();
						foreach (var conjugatePair in conjugatePairs[x] ?? Array.Empty<ConjugatePair>())
						{
							var cpMap = conjugatePair.Map;
							if (cpMap.Overlaps(map1) || cpMap.Overlaps(map2))
							{
								// Conjugate pair cannot overlap with the ALS structure.
								continue;
							}

							if ((cpMap & p1).Count != 1 || (cpMap & p2).Count != 1 || ((p1 | p2) & cpMap).Count != 2)
							{
								continue;
							}

							foreach (int w in (mask & ~(1 << x)).GetAllSets())
							{
								var tempMap =
									new GridMap((map1 | map2) & candsMap[w], ProcessPeersWithoutItself) & candsMap[w];
								if (tempMap.IsEmpty)
								{
									continue;
								}

								wDigitsMask |= (short)(1 << w);
								foreach (int cell in tempMap.Offsets)
								{
									conclusions.Add(new Conclusion(Elimination, cell, w));
								}
							}

							if (conclusions.Count == 0)
							{
								continue;
							}

							// Record highlight cell and candidate offsets.
							var cellOffsets = new List<(int, int)>();
							cellOffsets.AddRange(from cell in map1.Offsets select (-1, cell));
							cellOffsets.AddRange(from cell in map2.Offsets select (-2, cell));

							var candidateOffsets = new List<(int, int)>
							{
								(0, cpMap.SetAt(0) * 9 + x), (0, cpMap.SetAt(1) * 9 + x)
							};
							foreach (int cell in map1.Offsets)
							{
								foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
								{
									candidateOffsets.Add((
										true switch
										{
											_ when digit == x => 1,
											_ when (wDigitsMask >> digit & 1) != 0 => 2,
											_ => -1
										},
										cell * 9 + digit));
								}
							}
							foreach (int cell in map2.Offsets)
							{
								foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
								{
									candidateOffsets.Add((
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
								new AlsWWingTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: _alsShowRegions ? null : cellOffsets,
											candidateOffsets: _alsShowRegions ? candidateOffsets : null,
											regionOffsets:
												_alsShowRegions
													? new[]
													{
														(-1, region1),
														(-2, region2),
														(0, conjugatePair.Region.First())
													}
													: null,
											links: null)
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
