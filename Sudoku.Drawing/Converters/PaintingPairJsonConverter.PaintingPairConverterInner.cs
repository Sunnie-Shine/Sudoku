using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sudoku.Drawing.Converters
{
	partial class PaintingPairJsonConverter
	{
		/// <summary>
		/// Indicates the inner converter.
		/// </summary>
		/// <typeparam name="T">The type of the value used.</typeparam>
		private sealed class PaintingPairConverterInner<T> : JsonConverter<PaintingPair<T>> where T : unmanaged
		{
			/// <summary>
			/// Indicates the string value representation of the property name <c>Color</c>.
			/// </summary>
			private const string ColorString = nameof(PaintingPair<T>.Color);

			/// <summary>
			/// Indicates the string value representation of the property name <c>Value</c>.
			/// </summary>
			private const string ValueString = nameof(PaintingPair<T>.Value);


			/// <summary>
			/// Indicates the JSON converter of the color (If need).
			/// </summary>
			private readonly JsonConverter<Color> _colorConverter;

			/// <summary>
			/// Indicates the JSON converter of the value (If need).
			/// </summary>
			private readonly JsonConverter<T>? _valueConverter;

			/// <summary>
			/// Indicates the real type token of the value.
			/// </summary>
			private readonly Type _valueType;


			/// <summary>
			/// Initializes an instance with the specified options.
			/// </summary>
			/// <param name="options">The options.</param>
			[DynamicDependency(DynamicallyAccessedMemberTypes.All, "Sudoku.JsonConverters.PaintingPairConverter", "Sudoku.Core")]
			public PaintingPairConverterInner(JsonSerializerOptions options)
			{
				// For performance, use the existing converter if available.
				_colorConverter = new ColorJsonConverter();
				_valueConverter = options.GetConverter(typeof(T)) as JsonConverter<T>;

				// Cache the key and value types.
				_valueType = typeof(T);
			}


			/// <inheritdoc/>
			/// <exception cref="JsonException">
			/// Throws when:
			/// <list type="number">
			/// <item>The converter reader doesn't locate at the start object token.</item>
			/// <item>The converter doesn't locate at the property name token while reading.</item>
			/// <item>The property name read is invalid.</item>
			/// <item>Failed to deserialize.</item>
			/// </list>
			/// </exception>
			public override unsafe PaintingPair<T> Read(
				ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType != JsonTokenType.StartObject)
				{
					throw new JsonException("The converter should locate at the start object token.");
				}

				Color color = default;
				T value;
				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndObject)
					{
						return new(color, *&value);
					}

					if (reader.TokenType != JsonTokenType.PropertyName)
					{
						throw new JsonException("The converter should locate at the property name token.");
					}

					string propertyName = reader.GetString()!;
					switch (propertyName)
					{
						case ColorString:
						{
							color = reader.ReadObject(_colorConverter, typeof(Color), options);
							break;
						}
						case ValueString:
						{
							value = reader.ReadObject(_valueConverter, _valueType, options);
							break;
						}
						default:
						{
							throw new JsonException("The property name is invalid.");
						}
					}
				}

				throw new JsonException("Failed to deserialize.");
			}

			/// <inheritdoc/>
			public override void Write(
				Utf8JsonWriter writer, PaintingPair<T> value, JsonSerializerOptions options)
			{
				writer.WriteStartObject();
				writer.WritePropertyName(ColorString);
				writer.WriteObject(value.Color, _colorConverter, options);
				writer.WritePropertyName(ValueString);
				writer.WriteObject(value.Value, _valueConverter, options);
				writer.WriteEndObject();
			}
		}
	}
}
