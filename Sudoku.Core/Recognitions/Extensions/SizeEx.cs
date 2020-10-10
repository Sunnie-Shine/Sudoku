﻿using System.Drawing;
using Sudoku.DocComments;

namespace Sudoku.Recognitions.Extensions
{
	/// <summary>
	/// Provides extension methods for <see cref="Size"/> and <see cref="SizeF"/>.
	/// </summary>
	/// <seealso cref="Size"/>
	/// <seealso cref="SizeF"/>
	public static class SizeEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The size instance.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		public static void Deconstruct(this SizeF @this, out float width, out float height) =>
			(width, height) = (@this.Width, @this.Height);
	}
}