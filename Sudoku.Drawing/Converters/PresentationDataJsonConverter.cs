using System;
using System.Collections.Generic;
using System.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Data;

namespace Sudoku.Drawing.Converters
{
	/// <summary>
	/// Provides a <see cref="PresentationData"/> JSON converter.
	/// </summary>
	/// <seealso cref="PresentationData"/>
	[JsonConverter(typeof(PresentationData))]
	public sealed class PresentationDataJsonConverter : JsonConverter<PresentationData>
	{
		/// <inheritdoc/>
		public override PresentationData? Read(
			ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var cells = new List<PaintingPair<int>>();
			var candidates = new List<PaintingPair<int>>();
			var regions = new List<PaintingPair<int>>();
			var links = new List<PaintingPair<Link>>();
			var directLines = new List<PaintingPair<(Cells, Cells)>>();

			dynamic? inst = null;
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName:
					{
						inst = reader.GetString() switch
						{
							nameof(PresentationData.Cells) => cells,
							nameof(PresentationData.Candidates) => candidates,
							nameof(PresentationData.Regions) => regions,
							nameof(PresentationData.Links) => links,
							nameof(PresentationData.DirectLines) => directLines
						};
						break;
					}
					case JsonTokenType.StartObject:
					{
						Type type = inst!.GetType().GenericTypeArguments[0];
						var converter = options.GetConverter(typeof(PaintingPairJsonConverter));
						switch (inst)
						{
							case List<PaintingPair<int>>:
							{
								reader.ReadObject(
									converter as JsonConverter<PaintingPair<int>>,
									type,
									options);
								break;
							}
							case List<PaintingPair<Link>>:
							{
								reader.ReadObject(
									converter as JsonConverter<PaintingPair<Link>>,
									type,
									options);
								break;
							}
							case List<PaintingPair<(Cells, Cells)>>:
							{
								reader.ReadObject(
									converter as JsonConverter<PaintingPair<(Cells, Cells)>>,
									type,
									options);
								break;
							}
						}
						break;
					}
				}
			}

			return new()
			{
				Cells = assign(cells),
				Candidates = assign(candidates),
				Regions = assign(regions),
				Links = assign(links),
				DirectLines = assign(directLines)
			};

			static IList<T>? assign<T>(List<T> z) => z.Count == 0 ? null : z;
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, PresentationData value, JsonSerializerOptions options)
		{
			var converter = options.GetConverter(typeof(PaintingPairJsonConverter));

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(PresentationData.Cells));
			writer.WriteStartArray();
			writer.WriteObjects(value.Cells, converter as JsonConverter<PaintingPair<int>>, options);
			writer.WriteEndArray();

			writer.WritePropertyName(nameof(PresentationData.Candidates));
			writer.WriteStartArray();
			writer.WriteObjects(value.Candidates, converter as JsonConverter<PaintingPair<int>>, options);
			writer.WriteEndArray();

			writer.WritePropertyName(nameof(PresentationData.Regions));
			writer.WriteStartArray();
			writer.WriteObjects(value.Regions, converter as JsonConverter<PaintingPair<int>>, options);
			writer.WriteEndArray();

			writer.WritePropertyName(nameof(PresentationData.Links));
			writer.WriteStartArray();
			writer.WriteObjects(value.Links, converter as JsonConverter<PaintingPair<Link>>, options);
			writer.WriteEndArray();

			writer.WritePropertyName(nameof(PresentationData.DirectLines));
			writer.WriteStartArray();
			writer.WriteObjects(value.DirectLines, converter as JsonConverter<PaintingPair<(Cells, Cells)>>, options);
			writer.WriteEndArray();

			writer.WriteEndObject();
		}
	}
}
