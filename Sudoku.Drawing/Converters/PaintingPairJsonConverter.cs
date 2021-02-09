using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sudoku.Drawing.Converters
{
	/// <summary>
	/// Indicates a <see cref="PaintingPair{T}"/> JSON converter.
	/// </summary>
	/// <seealso cref="PaintingPair{T}"/>
	public sealed partial class PaintingPairJsonConverter : JsonConverterFactory
	{
		/// <inheritdoc/>
		public override bool CanConvert(Type typeToConvert)
		{
			if (!typeToConvert.IsGenericType)
			{
				return false;
			}

			if (typeToConvert.GetGenericTypeDefinition() != typeof(PaintingPair<>))
			{
				return false;
			}

			return typeToConvert.GetGenericArguments()[0].IsEnum;
		}

		/// <inheritdoc/>
		public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
		{
			var valueType = typeToConvert.GetGenericArguments()[0];
			var converter = Activator.CreateInstance(
				type: typeof(PaintingPairConverterInner<>).MakeGenericType(new[] { valueType }),
				bindingAttr: BindingFlags.Instance | BindingFlags.Public,
				binder: null,
				args: new[] { options },
				culture: null
			) as JsonConverter;

			return converter;
		}
	}
}
