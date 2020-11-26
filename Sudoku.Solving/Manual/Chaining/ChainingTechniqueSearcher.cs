﻿using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Extensions;
using Sudoku.Runtime;
using System.Collections.Generic;
using System.Linq;
using static Sudoku.Constants.Processings;
using C = Sudoku.Solving.Manual.Chaining.ChainingTechniqueInfo;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a <b>chain</b> technique searcher.
	/// </summary>
	public abstract class ChainingTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// Get all available weak links.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="p">(<see langword="in"/> parameter) The current node.</param>
		/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
		/// <returns>All possible weak links.</returns>
		protected internal static ISet<Node> GetOnToOff(in SudokuGrid grid, in Node p, bool yEnabled)
		{
			var result = new Set<Node>();

			if (yEnabled)
			{
				// First rule: Other candidates for this cell get off.
				for (int digit = 0; digit < 9; digit++)
				{
					if (digit != p.Digit && grid.Exists(p.Cell, digit) is true)
					{
						result.Add(new(p.Cell, digit, false, p));
					}
				}
			}

			// Second rule: Other positions for this digit get off.
			for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
			{
				int region = GetRegion(p.Cell, label);
				for (int pos = 0; pos < 9; pos++)
				{
					int cell = RegionCells[region][pos];
					if (cell != p.Cell && grid.Exists(cell, p.Digit) is true)
					{
						result.Add(new(cell, p.Digit, false, p));
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Get all available strong links.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="p">(<see langword="in"/> parameter) The current node.</param>
		/// <param name="xEnabled">Indicates whether the X-Chains are enabled.</param>
		/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
		/// <returns>All possible strong links.</returns>
		protected internal static ISet<Node> GetOffToOn(
			in SudokuGrid grid, in Node p, bool xEnabled, bool yEnabled,
			in SudokuGrid source = default, ISet<Node>? offNodes = null)
		{
			var result = new Set<Node>();
			if (yEnabled)
			{
				// First rule: If there's only two candidates in this cell, the other one gets on.
				short mask = (short)(grid.GetCandidateMask(p.Cell) & ~(1 << p.Digit));
				if (BivalueMap[p.Cell] && mask.IsPowerOfTwo())
				{
					var pOn = new Node(p.Cell, mask.FindFirstSet(), true, p);
					if (source != default && offNodes is not null)
					{
						AddHiddenParentsOfCell(ref pOn, grid, source, offNodes);
					}

					result.Add(pOn);
				}
			}

			if (xEnabled)
			{
				// Second rule: If there's only two positions for this candidate, the other ont gets on.
				for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
				{
					int region = GetRegion(p.Cell, label);
					if ((CandMaps[p.Digit] & RegionMaps[region]) - p.Cell is { Count: 1 } cells)
					{
						var pOn = new Node(cells.First, p.Digit, true, p);
						if (source != default && offNodes is not null)
						{
							AddHiddenParentsOfRegion(ref pOn, grid, source, label, offNodes);
						}

						result.Add(pOn);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Add hidden parents of a cell.
		/// </summary>
		/// <param name="p">(<see langword="ref"/> parameter) The node.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="source">(<see langword="in"/> parameter) The source grid.</param>
		/// <param name="offNodes">All off nodes.</param>
		/// <exception cref="SudokuRuntimeException">
		/// Throws when the parent node of the specified node cannot be found.
		/// </exception>
		private static void AddHiddenParentsOfCell(
			ref Node p, in SudokuGrid grid, in SudokuGrid source, ISet<Node> offNodes)
		{
			foreach (int digit in (short)(source.GetCandidateMask(p.Cell) & ~grid.GetCandidateMask(p.Cell)))
			{
				// Add a hidden parent.
				var parent = new Node(p.Cell, digit, false);
				p.AddParent(
					offNodes.Contains(parent)
					? parent
					: throw new SudokuRuntimeException("Parent node not found."));
			}
		}

		/// <summary>
		/// Add hidden parents of a region.
		/// </summary>
		/// <param name="p">(<see langword="ref"/> parameter) The node.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="source">(<see langword="in"/> parameter) The source grid.</param>
		/// <param name="currRegion">The current region label.</param>
		/// <param name="offNodes">All off nodes.</param>
		/// <exception cref="SudokuRuntimeException">
		/// Throws when the parent node of the specified node cannot be found.
		/// </exception>
		private static void AddHiddenParentsOfRegion(
			ref Node p, in SudokuGrid grid, in SudokuGrid source, RegionLabel currRegion, ISet<Node> offNodes)
		{
			int region = GetRegion(p.Cell, currRegion);
			foreach (int pos in (short)(m(source, p.Digit, region) & ~m(grid, p.Digit, region)))
			{
				// Add a hidden parent.
				var parent = new Node(RegionCells[region][pos], p.Digit, false);
				p.AddParent(
					offNodes.Contains(parent)
					? parent
					: throw new SudokuRuntimeException("Parent node not found."));
			}

			static short m(in SudokuGrid grid, int digit, int region)
			{
				short result = 0;
				for (int i = 0; i < 9; i++)
				{
					if (grid.Exists(RegionCells[region][i], digit) is true)
					{
						result |= (short)(1 << i);
					}
				}

				return result;
			}
		}

		/// <summary>
		/// Remove duplicate information instances and sort them.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <returns>The result list.</returns>
		protected static IQueryable<C> SortInfo(IEnumerable<C> accumulator) =>
		(
			from info in new Set<C>(accumulator)
			orderby info.Difficulty, info.Complexity, info.SortKey
			select info
		).AsQueryable();
	}
}
