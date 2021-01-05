﻿using System.Numerics;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="BitOperations"/>.
	/// </summary>
	/// <seealso cref="BitOperations"/>
	public static class BitOperationsEx
	{
		/// <inheritdoc cref="Integer.IsPowerOfTwo(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(this byte @this) => @this != 0 && (@this & (@this - 1)) == 0;

		/// <inheritdoc cref="Integer.IsPowerOfTwo(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(this short @this) => @this != 0 && (@this & (@this - 1)) == 0;

		/// <inheritdoc cref="Integer.IsPowerOfTwo(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(this int @this) => @this != 0 && (@this & (@this - 1)) == 0;

		/// <inheritdoc cref="Integer.IsPowerOfTwo(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(this long @this) => @this != 0 && (@this & (@this - 1L)) == 0;

		/// <inheritdoc cref="Integer.IsOdd(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOdd(this byte @this) => (@this & 1) != 0;

		/// <inheritdoc cref="Integer.IsOdd(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOdd(this short @this) => (@this & 1) != 0;

		/// <inheritdoc cref="Integer.IsOdd(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOdd(this int @this) => (@this & 1) != 0;

		/// <inheritdoc cref="Integer.IsOdd(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOdd(this long @this) => (@this & 1) != 0;

		/// <inheritdoc cref="Integer.IsEven(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEven(this byte @this) => (@this & 1) == 0;

		/// <inheritdoc cref="Integer.IsEven(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEven(this short @this) => (@this & 1) == 0;

		/// <inheritdoc cref="Integer.IsEven(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEven(this int @this) => (@this & 1) == 0;

		/// <inheritdoc cref="Integer.IsEven(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEven(this long @this) => (@this & 1) == 0;

		/// <inheritdoc cref="Integer.ContainsBit(Integer, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContainsBit(this byte @this, int bitPosition) => (@this >> bitPosition & 1) != 0;

		/// <inheritdoc cref="Integer.ContainsBit(Integer, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContainsBit(this short @this, int bitPosition) => (@this >> bitPosition & 1) != 0;

		/// <inheritdoc cref="Integer.ContainsBit(Integer, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContainsBit(this int @this, int bitPosition) => (@this >> bitPosition & 1) != 0;

		/// <inheritdoc cref="Integer.ContainsBit(Integer, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContainsBit(this long @this, int bitPosition) => (@this >> bitPosition & 1) != 0;

		/// <inheritdoc cref="Integer.Overlaps(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Overlaps(this byte @this, byte other) => (@this & other) != 0;

		/// <inheritdoc cref="Integer.Overlaps(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Overlaps(this short @this, short other) => (@this & other) != 0;

		/// <inheritdoc cref="Integer.Overlaps(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Overlaps(this int @this, int other) => (@this & other) != 0;

		/// <inheritdoc cref="Integer.Overlaps(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Overlaps(this long @this, long other) => (@this & other) != 0;

		/// <inheritdoc cref="Integer.ExceptOverlaps(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ExceptOverlaps(this byte @this, byte other) => (@this & ~other) != 0;

		/// <inheritdoc cref="Integer.ExceptOverlaps(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ExceptOverlaps(this short @this, short other) => (@this & ~other) != 0;

		/// <inheritdoc cref="Integer.ExceptOverlaps(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ExceptOverlaps(this int @this, int other) => (@this & ~other) != 0;

		/// <inheritdoc cref="Integer.ExceptOverlaps(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ExceptOverlaps(this long @this, long other) => (@this & ~other) != 0;

		/// <inheritdoc cref="Integer.Covers(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Covers(this byte @this, byte other) => (@this & other) == other;

		/// <inheritdoc cref="Integer.Covers(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Covers(this short @this, short other) => (@this & other) == other;

		/// <inheritdoc cref="Integer.Covers(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Covers(this int @this, int other) => (@this & other) == other;

		/// <inheritdoc cref="Integer.Covers(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Covers(this long @this, long other) => (@this & other) == other;

		/// <inheritdoc cref="Integer.FindFirstSet(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this byte @this) => BitOperations.TrailingZeroCount(@this);

		/// <inheritdoc cref="Integer.FindFirstSet(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this short @this) => BitOperations.TrailingZeroCount(@this);

		/// <inheritdoc cref="Integer.FindFirstSet(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this int @this) => BitOperations.TrailingZeroCount(@this);

		/// <inheritdoc cref="Integer.FindFirstSet(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this long @this) => BitOperations.TrailingZeroCount(@this);

		/// <inheritdoc cref="Integer.PopCount(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(this byte @this) => BitOperations.PopCount(@this);

		/// <inheritdoc cref="Integer.PopCount(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(this short @this) => BitOperations.PopCount((uint)@this);

		/// <inheritdoc cref="Integer.PopCount(Integer)"/>
		public static int PopCount(this int @this) => BitOperations.PopCount((uint)@this);

		/// <inheritdoc cref="Integer.PopCount(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(this long @this) => BitOperations.PopCount((ulong)@this);

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
		public static int GetNextSet(this byte @this, int index)
		{
			for (int i = index + 1; i < 8; i++)
			{
				if (@this.ContainsBit(i))
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
		public static int GetNextSet(this short @this, int index)
		{
			for (int i = index + 1; i < 16; i++)
			{
				if (@this.ContainsBit(i))
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
		public static int GetNextSet(this int @this, int index)
		{
			for (int i = index + 1; i < 32; i++)
			{
				if (@this.ContainsBit(i))
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
		public static int GetNextSet(this long @this, int index)
		{
			for (int i = index + 1; i < 64; i++)
			{
				if (@this.ContainsBit(i))
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
		public static int SetAt(this byte @this, int order)
		{
			for (int i = 0, count = -1; i < 8; i++, @this >>= 1)
			{
				if (@this.IsOdd() && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
		public static int SetAt(this short @this, int order)
		{
			for (int i = 0, count = -1; i < 16; i++, @this >>= 1)
			{
				if (@this.IsOdd() && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
		public static int SetAt(this int @this, int order)
		{
			for (int i = 0, count = -1; i < 32; i++, @this >>= 1)
			{
				if (@this.IsOdd() && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
		public static int SetAt(this long @this, int order)
		{
			for (int i = 0, count = -1; i < 64; i++, @this >>= 1)
			{
				if (@this.IsOdd() && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int> GetAllSets(this byte @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = @this.PopCount();
			var resultSpan = (stackalloc int[length]);
			for (byte i = 0, p = 0; i < 8; i++, @this >>= 1)
			{
				if (@this.IsOdd())
				{
					resultSpan[p++] = i;
				}
			}

			return new(resultSpan.ToArray());
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this short @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = @this.PopCount();
			var resultSpan = (stackalloc int[length]);
			for (byte i = 0, p = 0; i < 16; i++, @this >>= 1)
			{
				if (@this.IsOdd())
				{
					resultSpan[p++] = i;
				}
			}

			return new(resultSpan.ToArray());
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this int @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = @this.PopCount();
			var resultSpan = (stackalloc int[length]);
			for (byte i = 0, p = 0; i < 32; i++, @this >>= 1)
			{
				if (@this.IsOdd())
				{
					resultSpan[p++] = i;
				}
			}

			return new(resultSpan.ToArray());
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this long @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = @this.PopCount();
			var resultSpan = (stackalloc int[length]);
			for (byte i = 0, p = 0; i < 64; i++, @this >>= 1)
			{
				if (@this.IsOdd())
				{
					resultSpan[p++] = i;
				}
			}

			return new(resultSpan.ToArray());
		}

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this byte @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this short @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this int @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this long @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.ReverseBits(ref Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReverseBits(this ref byte @this)
		{
			@this = (byte)(@this >> 1 & 0x55 | (@this & 0x55) << 1);
			@this = (byte)(@this >> 2 & 0x33 | (@this & 0x33) << 2);
			@this = (byte)(@this >> 4 | @this << 4);
		}

		/// <inheritdoc cref="Integer.ReverseBits(ref Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReverseBits(this ref short @this)
		{
			@this = (short)(@this >> 1 & 0x5555 | (@this & 0x5555) << 1);
			@this = (short)(@this >> 2 & 0x3333 | (@this & 0x3333) << 2);
			@this = (short)(@this >> 4 & 0x0F0F | (@this & 0x0F0F) << 4);
			@this = (short)(@this >> 8 | @this << 8);
		}

		/// <inheritdoc cref="Integer.ReverseBits(ref Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReverseBits(this ref int @this)
		{
			@this = @this >> 1 & 0x55555555 | (@this & 0x55555555) << 1;
			@this = @this >> 2 & 0x33333333 | (@this & 0x33333333) << 2;
			@this = @this >> 4 & 0x0F0F0F0F | (@this & 0x0F0F0F0F) << 4;
			@this = @this >> 8 & 0x00FF00FF | (@this & 0x00FF00FF) << 8;
			@this = @this >> 16 | @this << 16;
		}

		/// <inheritdoc cref="Integer.ReverseBits(ref Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReverseBits(this ref long @this)
		{
			@this = @this >> 1 & 0x55555555_55555555L | (@this & 0x55555555_55555555L) << 1;
			@this = @this >> 2 & 0x33333333_33333333L | (@this & 0x33333333_33333333L) << 2;
			@this = @this >> 4 & 0x0F0F0F0F_0F0F0F0FL | (@this & 0x0F0F0F0F_0F0F0F0FL) << 4;
			@this = @this >> 8 & 0x00FF00FF_00FF00FFL | (@this & 0x00FF00FF_00FF00FFL) << 8;
			@this = @this >> 16 & 0x0000FFFF_0000FFFFL | (@this & 0x0000FFFF_0000FFFFL) << 16;
			@this = @this >> 32 | @this << 32;
		}
	}
}