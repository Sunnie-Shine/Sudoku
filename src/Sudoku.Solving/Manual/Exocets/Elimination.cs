﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.DocComments;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates an instance to represent a series of eliminations in JE or SE.
	/// </summary>
	[DisableParameterlessConstructor]
	public readonly struct Elimination : IValueEquatable<Elimination>
	{
		/// <summary>
		/// Initializes an instance with the eliminations and the reason.
		/// </summary>
		/// <param name="eliminations">(<see langword="in"/> parameter) The eliminations.</param>
		/// <param name="reason">The reason why those candidates can be eliminated.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Elimination(in Candidates eliminations, EliminatedReason reason)
		{
			Eliminations = eliminations;
			Reason = reason;
		}


		/// <summary>
		/// Indicates how many eliminations the instance contains.
		/// </summary>
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Eliminations.Count;
		}

		/// <summary>
		/// Indicates the eliminations.
		/// </summary>
		public Candidates Eliminations { get; }

		/// <summary>
		/// Indicates the reason why these candidates can be eliminated.
		/// </summary>
		public EliminatedReason Reason { get; }

		/// <summary>
		/// Indicates the header of the reason.
		/// </summary>
		private string Header => Reason switch
		{
			EliminatedReason.Basic => "Target",
			EliminatedReason.TargetInference => "Target inference",
			EliminatedReason.Mirror => "Mirror",
			EliminatedReason.BiBiPattern => "Bi-Bi pattern",
			EliminatedReason.TargetPair => "Target pair",
			EliminatedReason.GeneralizedSwordfish => "Generalized swordfish",
			EliminatedReason.TrueBase => "True base",
			EliminatedReason.CompatibilityTest => "Compatibility test"
		};


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="eliminations">(<see langword="out"/> parameter) The eliminations.</param>
		/// <param name="reason">
		/// (<see langword="out"/> parameter) The reason why the eliminations should be removed.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Deconstruct(out Candidates eliminations, out EliminatedReason reason)
		{
			eliminations = Eliminations;
			reason = Reason;
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? obj) => obj is Elimination comparer && Equals(comparer);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(in Elimination other) => Eliminations == other.Eliminations && Reason == other.Reason;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => Eliminations.GetHashCode() ^ (int)Reason << 17 & 0x135246;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override unsafe string ToString() =>
			$"* {Header} elimination: {new ConclusionCollection(AsSpan()).ToString()}";

		/// <summary>
		/// Converts all elements to <see cref="Conclusion"/>.
		/// </summary>
		/// <returns>The <see cref="ReadOnlySpan{T}"/> of type <see cref="Conclusion"/>.</returns>
		/// <seealso cref="ReadOnlySpan{T}"/>
		/// <seealso cref="Conclusion"/>
		public ReadOnlySpan<Conclusion> AsSpan()
		{
			var result = new Conclusion[Eliminations.Count];
			for (int i = 0; i < Eliminations.Count; i++)
			{
				result[i] = new(ConclusionType.Elimination, Eliminations[i]);
			}

			return result;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<Conclusion>.Enumerator GetEnumerator() => AsSpan().GetEnumerator();


		/// <inheritdoc cref="Operators.operator =="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in Elimination left, in Elimination right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in Elimination left, in Elimination right) => !(left == right);

		/// <summary>
		/// To merge two different instances, and return the merged result.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>
		/// The merged result. The result will contain all eliminations from two instances, and
		/// the reason should be same.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Throws when two instances contains different eliminated reason.
		/// </exception>
		public static unsafe Elimination operator |(in Elimination left, in Elimination right)
		{
			var (le, lr) = left;
			var (re, rr) = right;

			if (lr != rr)
			{
				throw new ArgumentException("Two arguments should contains same eliminated reason.");
			}

			int count = le.Count + re.Count;
			int* merged = stackalloc int[count];
			for (int i = 0; i < le.Count; i++)
			{
				merged[i] = left.Eliminations[i];
			}
			for (int i = 0; i < re.Count; i++)
			{
				merged[i + le.Count] = re[i];
			}

			return new(new(merged, count), lr);
		}
	}
}