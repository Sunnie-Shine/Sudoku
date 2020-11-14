﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sudoku.Data
{
	public readonly ref partial struct BitSubsetsGenerator
	{
		/// <summary>
		/// Indicates the enumerator of the current instance.
		/// </summary>
		public struct Enumerator : IEnumerator<long>
		{
			/// <summary>
			/// The mask.
			/// </summary>
			private readonly long _mask;


			/// <summary>
			/// Indicates whether that the value is the last one.
			/// </summary>
			private bool _isLast;


			/// <summary>
			/// Initializes an instance with the specified number of bits
			/// and <see langword="true"/> bits.
			/// </summary>
			/// <param name="bitCount">The number of bits.</param>
			/// <param name="oneCount">The number of <see langword="true"/> bits.</param>
			public Enumerator(int bitCount, int oneCount) =>
				(Current, _mask, _isLast) = ((1 << oneCount) - 1, (1 << bitCount - oneCount) - 1, bitCount == 0);


			/// <inheritdoc/>
			[SkipLocalsInit]
			public long Current { readonly get; private set; }

			/// <inheritdoc/>
			readonly object IEnumerator.Current => Current;


			/// <inheritdoc/>
			/// <remarks>Here will do nothing.</remarks>
			public readonly void Dispose()
			{
			}

			/// <inheritdoc/>
			public bool MoveNext()
			{
				if (hasNext(ref this) is var result && result && !_isLast)
				{
					long smallest = Current & -Current;
					long ripple = Current + smallest;
					long ones = Current ^ ripple;
					ones = (ones >> 2) / smallest;
					Current = ripple | ones;
				}

				return result;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				static bool hasNext(ref Enumerator @this)
				{
					bool result = !@this._isLast;
					@this._isLast = (@this.Current & -@this.Current & @this._mask) == 0;
					return result;
				}
			}

			/// <inheritdoc/>
			/// <remarks>Here will do nothing.</remarks>
			public readonly void Reset()
			{
			}
		}
	}
}