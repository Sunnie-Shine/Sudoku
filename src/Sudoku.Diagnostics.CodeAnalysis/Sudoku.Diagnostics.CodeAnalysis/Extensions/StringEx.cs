﻿using System;
using System.Text;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="string"/>.
	/// </summary>
	/// <seealso cref="string"/>
	public static class StringEx
	{
		/// <summary>
		/// Converts the current name into the camel case.
		/// </summary>
		/// <param name="this">The name.</param>
		/// <param name="caseConvertingOption">The option that decides the result.</param>
		/// <returns>The result name.</returns>
		public static unsafe string ToCamelCase(
			this string @this, CaseConvertingOption caseConvertingOption = CaseConvertingOption.None)
		{
			switch (@this[0])
			{
				case >= 'A' and <= 'Z':
				{
					var sb = new StringBuilder(@this);
					sb[0] += ' ';

					return caseConvertingOption switch
					{
						CaseConvertingOption.None => sb.ToString(),
						CaseConvertingOption.ReserveLeadingUnderscore => $"_{sb}"
					};
				}
				case >= 'a' and <= 'z':
				{
					return caseConvertingOption switch
					{
						CaseConvertingOption.None => @this,
						CaseConvertingOption.ReserveLeadingUnderscore => $"_{@this}"
					};
				}
				/*length-pattern*/
				/*slice-pattern*/
				case '_' when @this.Length >= 2 && @this[1] is >= 'A' and <= 'Z' or >= 'a' and <= 'z':
				{
					if (@this[1] is >= 'A' and <= 'Z')
					{
						var sb = new StringBuilder(@this.Substring(1));
						sb[0] += ' ';

						return sb.ToString();
					}
					else
					{
						return caseConvertingOption switch
						{
							CaseConvertingOption.None => @this.Substring(1),
							CaseConvertingOption.ReserveLeadingUnderscore => @this
						};
					}
				}
				default:
				{
					throw new ArgumentException(
						"The specified argument is invalid name to convert.",
						nameof(@this)
					);
				}
			}
		}

		/// <summary>
		/// Converts the current name into the pascal case.
		/// </summary>
		/// <param name="this">The name.</param>
		/// <returns>The result name.</returns>
		public static unsafe string ToPascalCase(this string @this)
		{
			switch (@this[0])
			{
				case >= 'A' and <= 'Z':
				{
					return @this;
				}
				case >= 'a' and <= 'z':
				{
					var sb = new StringBuilder(@this);
					sb[0] -= ' ';

					return sb.ToString();
				}
				case '_' when @this.Length >= 2 && @this[1] is >= 'A' and <= 'Z' or >= 'a' and <= 'z':
				{
					var sb = new StringBuilder(@this.Substring(1));
					if (sb[0] is >= 'a' and <= 'z')
					{
						sb[0] -= ' ';
					}

					return sb.ToString();
				}
				default:
				{
					throw new ArgumentException(
						"The specified argument is invalid name to convert.",
						nameof(@this)
					);
				}
			}
		}
	}
}
