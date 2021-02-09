using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Resources;
using Sudoku.Solving.Manual.Exocets;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Encapsulates all information after searched a solving step,
	/// which include the conclusion, the difficulty and so on.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public abstract record StepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views)
	{
		/// <summary>
		/// The names of all subsets by their sizes.
		/// </summary>
		protected static readonly string[] SubsetNames =
		{
			string.Empty, "Single", "Pair", "Triple", "Quadruple",
			"Quintuple", "Sextuple", "Septuple", "Octuple", "Nonuple"
		};


		/// <summary>
		/// <para>
		/// Indicates whether the difficulty rating of this technique should be
		/// shown in the output screen. Some techniques such as <b>Gurth's symmetrical placement</b>
		/// doesn't need to show the difficulty (because the difficulty of this technique
		/// is unstable).
		/// </para>
		/// <para>
		/// If the value is <see langword="true"/>, the analysis result won't show the difficulty
		/// of this instance.
		/// </para>
		/// </summary>
		public virtual bool ShowDifficulty => true;

		/// <summary>
		/// Indicates the technique name.
		/// </summary>
		public virtual string Name => TextResources.Current[TechniqueCodeName];

		/// <summary>
		/// Indicates the technique name alias.
		/// </summary>
		public string[]? NameAlias =>
			TextResources.Current[$"{TechniqueCodeName}Alias"]?.Split(new[] { ';', ' ' });

		/// <summary>
		/// Indicates the acronym of the step name. For example, the acronym of the technique
		/// "Almost Locked Candidates" is ALC.
		/// </summary>
		/// <remarks>
		/// This property only contains the result in English. Other languages doesn't contain any
		/// abbreviations by default. On the other hand, this is really an easier way to implement
		/// than storing values in resource dictionary files.
		/// </remarks>
		public virtual string? Acronym => null;

		/// <summary>
		/// The difficulty or this step.
		/// </summary>
		public abstract decimal Difficulty { get; }

		/// <summary>
		/// The technique code of this instance used for comparison
		/// (e.g. search for specified puzzle that contains this technique).
		/// </summary>
		public abstract Technique TechniqueCode { get; }

		/// <summary>
		/// The technique tags of this instance.
		/// </summary>
		public abstract TechniqueTags TechniqueTags { get; }

		/// <summary>
		/// The difficulty level of this step.
		/// </summary>
		public abstract DifficultyLevel DifficultyLevel { get; }

		/// <summary>
		/// Indicates the technique code name.
		/// </summary>
		protected string TechniqueCodeName => TechniqueCode.ToString();


		/// <summary>
		/// Put this instance into the specified grid.
		/// </summary>
		/// <param name="grid">(<see langword="ref"/> parameter) The grid.</param>
		public void ApplyTo(ref SudokuGrid grid)
		{
			foreach (var conclusion in Conclusions)
			{
				conclusion.ApplyTo(ref grid);
			}
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		public void Deconstruct(out string name, out decimal difficulty)
		{
			name = Name;
			difficulty = Difficulty;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(<see langword="out"/> parameter) The difficulty level.</param>
		public void Deconstruct(out string name, out decimal difficulty, out DifficultyLevel difficultyLevel)
		{
			name = Name;
			difficulty = Difficulty;
			difficultyLevel = DifficultyLevel;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(<see langword="out"/> parameter) The difficulty level.</param>
		/// <param name="conclusions">(<see langword="out"/> parameter) All conclusions.</param>
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevel difficultyLevel,
			out IReadOnlyList<Conclusion> conclusions)
		{
			name = Name;
			difficulty = Difficulty;
			difficultyLevel = DifficultyLevel;
			conclusions = Conclusions;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(<see langword="out"/> parameter) The difficulty level.</param>
		/// <param name="conclusions">(<see langword="out"/> parameter) All conclusions.</param>
		/// <param name="views">(<see langword="out"/> parameter) All views.</param>
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevel difficultyLevel,
			out IReadOnlyList<Conclusion> conclusions, out IReadOnlyList<PresentationData> views)
		{
			name = Name;
			difficulty = Difficulty;
			difficultyLevel = DifficultyLevel;
			conclusions = Conclusions;
			views = Views;
		}

		/// <summary>
		/// Determine whether the current step information instance with the specified flags.
		/// </summary>
		/// <param name="flags">
		/// The flags. If the argument contains more than one set bit, all flags will be checked
		/// one by one.
		/// </param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public unsafe bool HasTag(TechniqueTags flags)
		{
			short value = (short)flags;
			delegate* managed<TechniqueTags, TechniqueTags, bool> func =
				(value & value - 1) != 0 ? &EnumEx.Flags : &EnumEx.MultiFlags;

			return func(TechniqueTags, flags);
		}

		/// <summary>
		/// Returns a string that only contains the name and the basic information. Different with
		/// <see cref="ToFullString()"/>, the method will only contains the basic introduction
		/// about the technique.
		/// For example, in the <see cref="ExocetStepInfo"/>, the detail will contain the several special
		/// eliminations, in this method, those won't be displayed, But the method <see cref="ToFullString()"/>
		/// will.
		/// </summary>
		/// <returns>The string instance.</returns>
		/// <seealso cref="ExocetStepInfo"/>
		/// <seealso cref="ToFullString()"/>
		public abstract override string ToString();

		/// <summary>
		/// Returns a string that only contains the name and the conclusions.
		/// </summary>
		/// <returns>The string instance.</returns>
		public string ToSimpleString() => $"{Name} => {new ConclusionCollection(Conclusions).ToString()}";

		/// <summary>
		/// Returns a string that contains the name, the conclusions and its all details.
		/// This method is used for displaying details in text box control.
		/// </summary>
		/// <returns>The string instance.</returns>
		public virtual string ToFullString() => ToString();
	}
}
