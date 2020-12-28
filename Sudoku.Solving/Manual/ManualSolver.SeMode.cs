﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.Models;
using Sudoku.Runtime;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual.Uniqueness;

namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		/// <summary>
		/// Solve the puzzle with <see cref="AnalyzeDifficultyStrictly"/> option.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="cloneation">(<see langword="ref"/> parameter) The cloneation grid to calculate.</param>
		/// <param name="steps">All steps found.</param>
		/// <param name="solution">(<see langword="in"/> parameter) The solution.</param>
		/// <param name="sukaku">Indicates whether the current mode is sukaku mode.</param>
		/// <param name="progressResult">
		/// (<see langword="ref"/> parameter)
		/// The progress result. This parameter is used for modify the state of UI controls.
		/// The current argument won't be used until <paramref name="progress"/> isn't <see langword="null"/>.
		/// In the default case, this parameter being
		/// <see langword="default"/>(<see cref="GridProgressResult"/>) is okay.
		/// </param>
		/// <param name="progress">
		/// The progress used for report the current state. If we don't need, the value should
		/// be assigned <see langword="null"/>.
		/// </param>
		/// <returns>The analysis result.</returns>
		/// <exception cref="WrongHandlingException">
		/// Throws when the solver can't solved due to wrong handling.
		/// </exception>
		/// <seealso cref="GridProgressResult"/>
		private unsafe AnalysisResult SolveSeMode(
			in SudokuGrid grid, ref SudokuGrid cloneation, IList<StepInfo> steps, in SudokuGrid solution,
			bool sukaku, ref GridProgressResult progressResult, IProgress<IProgressResult>? progress)
		{
			var searchers = this.GetSeModeSearchers(solution);
			var stepGrids = new List<SudokuGrid>();
			var bag = new List<StepInfo>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();

		Restart:
			StepSearcher.InitializeMaps(cloneation);
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcherListGroup = searchers[i];
				foreach (var searcher in searcherListGroup)
				{
					if ((sukaku, searcher) is (true, UniquenessStepSearcher))
					{
						// Sukaku mode can't use them.
						// In fact, sukaku can use uniqueness tests, however the program should
						// produce a large modification.
						continue;
					}

					var props = TechniqueProperties.GetPropertiesFrom(searcher)!;
					if (props is { IsEnabled: false, DisabledReason: not DisabledReason.HighAllocation })
					{
						continue;
					}

					if (EnableGarbageCollectionForcedly
						&& props.DisabledReason.Flags(DisabledReason.HighAllocation))
					{
						GC.Collect();
					}

					searcher.GetAll(bag, cloneation);
				}
				if (bag.Count == 0)
				{
					continue;
				}

				if (FastSearch)
				{
					decimal minDiff = bag.Min(static info => info.Difficulty);
					var selection = from info in bag where info.Difficulty == minDiff select info;
					if (selection.None())
					{
						continue;
					}

					bool allConclusionsAreValid = selection.All(&InternalChecking, solution);
					if (!CheckConclusionValidityAfterSearched || allConclusionsAreValid)
					{
						foreach (var step in selection)
						{
							if (
								RecordStep(
									steps, step, grid, ref cloneation, stopwatch, stepGrids, out var result))
							{
								stopwatch.Stop();
								return result;
							}
						}

						// The puzzle has not been finished,
						// we should turn to the first step finder
						// to continue solving puzzle.
						bag.Clear();

						if (progress is not null)
						{
							ReportProgress(cloneation, progress, ref progressResult);
						}

						goto Restart;
					}
					else
					{
						StepInfo? wrongStep = null;
						foreach (var step in selection)
						{
							if (!CheckConclusionsValidity(solution, step.Conclusions))
							{
								wrongStep = step;
								break;
							}
						}
						throw new WrongHandlingException(grid, $"The specified step is wrong: {wrongStep}.");
					}
				}
				else
				{
					StepInfo? step;
					unsafe
					{
						step = bag.GetElementByMinSelector<StepInfo, decimal>(&InternalSelector);
					}

					if (step is null)
					{
						// If current step can't find any steps,
						// we will turn to the next step finder to
						// continue solving puzzle.
						continue;
					}

					if (!CheckConclusionValidityAfterSearched
						|| CheckConclusionsValidity(solution, step.Conclusions))
					{
						if (RecordStep(steps, step, grid, ref cloneation, stopwatch, stepGrids, out var result))
						{
							// The puzzle has been solved.
							// :)
							stopwatch.Stop();
							return result;
						}
						else
						{
							// The puzzle has not been finished,
							// we should turn to the first step finder
							// to continue solving puzzle.
							bag.Clear();

							if (progress is not null)
							{
								ReportProgress(cloneation, progress, ref progressResult);
							}

							goto Restart;
						}
					}
					else
					{
						throw new WrongHandlingException(grid, $"The specified step is wrong: {step}.");
					}
				}
			}

			// All solver can't finish the puzzle...
			// :(
			stopwatch.Stop();
			return new(SolverName, grid, false, stopwatch.Elapsed)
			{
				Steps = steps.AsReadOnlyList(),
				StepGrids = stepGrids,
			};
		}
	}
}
