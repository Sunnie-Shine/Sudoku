using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern</b> (QDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pattern">The pattern.</param>
	public abstract record QdpStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<PresentationData> Views, in Pattern Pattern
	) : UniquenessStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.8M;

		/// <inheritdoc/>
		public abstract override Technique TechniqueCode { get; }

		/// <inheritdoc/>
		public sealed override string Name => base.Name;

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
