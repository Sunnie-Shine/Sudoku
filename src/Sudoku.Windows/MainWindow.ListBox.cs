﻿using System.Windows.Controls;
using InfoTriplet = System.Collections.Generic.KeyedTuple<string, Sudoku.Solving.Manual.StepInfo, bool>;
using StepTriplet = System.Collections.Generic.KeyedTuple<string, int, Sudoku.Solving.Manual.StepInfo>;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void ListBoxPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (
				sender is ListBox
				{
					SelectedIndex: not -1,
					SelectedItem: ListBoxItem { Content: StepTriplet { Item2: var n, Item3: var s } }
				} && _analyisResult is { Steps: not null, StepGrids: not null })
			{
				var techniqueInfo = _analyisResult.Steps[n];
				_currentStepInfo = techniqueInfo;
				_currentViewIndex = 0;

				_currentPainter = _currentPainter with
				{
					Grid = _puzzle = _analyisResult.StepGrids[n],
					View = s.Views[0],
					Conclusions = techniqueInfo.Conclusions
				};

				_textBoxInfo.Text = techniqueInfo.ToString();

				UpdateImageGrid();
			}
		}

		private void ListBoxTechniques_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox { SelectedItem: ListBoxItem { Content: InfoTriplet triplet } })
			{
				if (triplet.Item3 && triplet.Item2 is var info and var (_, _, _, conclusions, views))
				{
					_currentStepInfo = info;
					_currentViewIndex = 0;
					_currentPainter.View = views[0];
					_currentPainter.Conclusions = conclusions;

					_textBoxInfo.Text = info.ToString();

					UpdateImageGrid();
				}
			}
			else
			{
				_contextMenuTechniques = null;
			}
		}
	}
}
