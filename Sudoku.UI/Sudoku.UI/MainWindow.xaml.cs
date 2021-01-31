﻿using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Sudoku.UI.Data;
using Sudoku.UI.Dictionaries;

namespace Sudoku.UI
{
	/// <summary>
	/// The main window of the program. This program will open this window at first.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		/// <summary>
		/// Initializes a <see cref="MainWindow"/> instance with the default instantiation behavior.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			LoadPreferences();
			InitializeProgramTitle();
		}


		/// <summary>
		/// Indicates the inner preferences.
		/// </summary>
		public Preferences Preferences { get; private set; }


		/// <summary>
		/// To load the preferences from local.
		/// </summary>
		[MemberNotNull(nameof(Preferences))]
		private void LoadPreferences()
		{
			// Check whether the local hard drive contains that configuration file.
			if (!File.Exists(Paths.ConfigurationFile))
			{
				Preferences = new();
			}

			// Deserialize the object.
			Preferences = JsonSerializer.Deserialize<Preferences>(
				json: File.ReadAllText(Paths.ConfigurationFile),
				options: JsonSerializerOptionsList.WithIndenting
			) ?? new();

			// Then set the instance to the main page. In this way we can get the preferences
			// from this property in the main page.
			MainPage.BaseWindow = this;
		}

		/// <summary>
		/// To save the preferences to local.
		/// </summary>
		private void SavePreferences()
		{
			string json = JsonSerializer.Serialize(Preferences, JsonSerializerOptionsList.WithIndenting);
			File.WriteAllText(Paths.ConfigurationFile, json);
		}

		/// <summary>
		/// To initialize the property <see cref="Window.Title"/>.
		/// </summary>
		/// <remarks>
		/// When I filled with this property into the XAML page, it'll give me a complier error
		/// <c>WMC061: Property not found</c>, so I moved that statement to here.
		/// </remarks>
		private void InitializeProgramTitle() => Title = ResourceFinder.Current.ProgramTitle;


		/// <summary>
		/// Triggers when the window is closed.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="args">The event arguments provided.</param>
		private void Window_Closed(object sender, WindowEventArgs args) => SavePreferences();
	}
}
