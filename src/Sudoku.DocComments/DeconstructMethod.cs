﻿using System;

namespace Sudoku.DocComments
{
	/// <summary>
	/// (Deconstruct method) Deconstruct the instance to multiple values.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The method should be declared manually when the type is a normal <see langword="struct"/>
	/// or <see langword="class"/>. If the method is in a <see langword="record"/>,
	/// the deconstruction method will be generated by the compiler automatically and returns
	/// <b>all properties</b> to those <see langword="out"/> parameters.
	/// </para>
	/// <para>
	/// Please note that the deconstruct methods can take <b>more than</b> 8 <see langword="out"/> parameters,
	/// although a normal <see cref="ValueTuple"/> can only contain at most 8 parameters.
	/// </para>
	/// </remarks>
	/// <seealso cref="ValueTuple"/>
	public sealed class DeconstructMethod
	{
	}
}