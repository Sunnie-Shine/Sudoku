using System.Collections.Generic;
using System.Extensions;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Provides a usage of <b>death blossom</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pivot">The pivot cell.</param>
	/// <param name="Alses">All ALSes used.</param>
	public sealed record DbStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, int Pivot,
		IReadOnlyDictionary<int, Als> Alses
	) : AlsStepInfo(Conclusions, Views)
	{
		/// <summary>
		/// Indicates how many petals used.
		/// </summary>
		public int PetalsCount => Alses.Count;

		/// <inheritdoc/>
		public override decimal Difficulty => 8.0M + PetalsCount * .1M;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.LongChaining;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.DeathBlossom;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			string pivotStr = new Cells { Pivot }.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: Cell {pivotStr} - {AlsPetalsToString()} => {elimStr}";
		}

		/// <summary>
		/// Get the string result from those ALS petals and their own branches.
		/// </summary>
		/// <returns>The string result.</returns>
		private StringBuilder AlsPetalsToString()
		{
#if DEBUG
			if (Alses is null)
			{
				return null!;
			}
#endif

			const string separator = ", ";

			return new StringBuilder().AppendRange(Alses, appender).RemoveFromEnd(separator.Length);

			static StringBuilder appender(KeyValuePair<int, Als> pair) =>
				new StringBuilder()
				.Append(pair.Key + 1)
				.Append(" - ")
				.Append(pair.Value.ToString())
				.Append(separator);
		}
	}
}
