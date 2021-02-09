using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets XZ rule</b> (ALS-XZ)
	/// or <b>extended subset principle</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Als1">The ALS 1 used.</param>
	/// <param name="Als2">The ALS 2 used.</param>
	/// <param name="XDigitsMask">The X digits mask.</param>
	/// <param name="ZDigitsMask">The Z digits mask.</param>
	/// <param name="IsDoublyLinked">
	/// <para>Indicates whether the instance is a doubly linked ALS-XZ.</para>
	/// <para>
	/// The property contains three different values:
	/// <list type="table">
	/// <item>
	/// <term><c><see langword="true"/></c></term>
	/// <description>The current instance is a Doubly Linked ALS-XZ.</description>
	/// </item>
	/// <item>
	/// <term><c><see langword="false"/></c></term>
	/// <description>The current instance is a Singly Linked ALS-XZ.</description>
	/// </item>
	/// <item>
	/// <term><c><see langword="null"/></c></term>
	/// <description>The current instance is a Extended Subset Principle.</description>
	/// </item>
	/// </list>
	/// </para>
	/// </param>
	public sealed record AlsXzStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, in Als Als1, in Als Als2,
		short XDigitsMask, short ZDigitsMask, bool? IsDoublyLinked
	) : AlsStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => IsDoublyLinked is true ? 5.7M : 5.5M;

		/// <inheritdoc/>
		public override string? Acronym => "ALS-XZ";

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			IsDoublyLinked switch
			{
				true => Technique.DoublyLinkedAlsXz,
				false => Technique.SinglyLinkedAlsXz,
				null => Technique.Esp
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			if (IsDoublyLinked is null)
			{
				// Extended subset principle.
				if (ZDigitsMask == 0)
				{
					string cellsStr = (Als1.Map | Als2.Map).ToString();
					return $"{Name}: All digits can't be duplicate in cells {cellsStr} => {elimStr}";
				}
				else
				{
					string digitStr = (TrailingZeroCount(ZDigitsMask) + 1).ToString();
					string cellsStr = (Als1.Map | Als2.Map).ToString();
					return $"{Name}: Only the digit {digitStr} can be duplicate in cells {cellsStr} => {elimStr}";
				}
			}
			else
			{
				// ALS-XZ.
				string xStr = new DigitCollection(XDigitsMask).ToString();
				string zResultStr =
					ZDigitsMask != 0
					? $", z = {new DigitCollection(ZDigitsMask).ToString()}"
					: string.Empty;
				return $"{Name}: ALS 1: {Als1.ToString()}, ALS 2: {Als2.ToString()}, x = {xStr}{zResultStr} => {elimStr}";
			}
		}
	}
}
