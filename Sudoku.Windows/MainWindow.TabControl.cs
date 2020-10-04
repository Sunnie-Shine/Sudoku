﻿using System.Windows.Controls;
using Sudoku.DocComments;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		/// <inheritdoc cref="Events.SelectionChanged(object?, System.EventArgs)"/>
		private void TabControlInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ReferenceEquals(_tabControlInfo.SelectedItem, _tabItemDrawing))
			{
				_currentViewIndex = -1;
				_currentTechniqueInfo = null;
				_currentPainter = _currentPainter with { View = null };

				UpdateImageGrid();
			}
		}
	}
}
