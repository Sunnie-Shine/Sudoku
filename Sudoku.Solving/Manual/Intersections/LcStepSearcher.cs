﻿using System.Collections.Generic;
using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Encapsulates a <b>locked candidates</b> (LC) technique searcher.
	/// </summary>
	public sealed class LcStepSearcher : IntersectionStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(26, nameof(TechniqueCode.Pointing))
		{
			DisplayLevel = 1
		};


		/// <inheritdoc/>
		[SkipLocalsInit]
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			var r = (stackalloc int[2]);
			foreach (var ((baseSet, coverSet), (a, b, c)) in IntersectionMaps)
			{
				if (!EmptyMap.Overlaps(c))
				{
					continue;
				}

				short m = (short)(grid.BitwiseOrMasks(c) & (grid.BitwiseOrMasks(a) ^ grid.BitwiseOrMasks(b)));
				if (m == 0)
				{
					continue;
				}

				foreach (int digit in m)
				{
					Cells elimMap;
					if (a.Overlaps(CandMaps[digit]))
					{
						r[0] = coverSet;
						r[1] = baseSet;
						elimMap = a & CandMaps[digit];
					}
					else
					{
						r[0] = baseSet;
						r[1] = coverSet;
						elimMap = b & CandMaps[digit];
					}
					if (elimMap.IsEmpty)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap)
					{
						conclusions.Add(new(Elimination, cell, digit));
					}

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in c & CandMaps[digit])
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
					}

					accumulator.Add(
						new LcStepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[] { new(0, r[0]) }
								}
							},
							digit,
							r[0],
							r[1]));
				}
			}
		}
	}
}