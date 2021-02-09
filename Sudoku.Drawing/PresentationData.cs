using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;

namespace Sudoku.Drawing
{
	/// <summary>
	/// This is a data structure that stores the presentation data when drawing onto a picture.
	/// </summary>
	/// <remarks>
	/// In the code, a presentation data instance can be also named as a <c>view</c>.
	/// </remarks>
	public sealed class PresentationData
	{
		/// <summary>
		/// The back field of <see cref="Cells"/>.
		/// </summary>
		/// <seealso cref="Cells"/>
		private IList<PaintingPair<int>>? _cells;

		/// <summary>
		/// The back field of <see cref="Candidates"/>.
		/// </summary>
		/// <seealso cref="Candidates"/>
		private IList<PaintingPair<int>>? _candidates;

		/// <summary>
		/// The back field of <see cref="Regions"/>.
		/// </summary>
		/// <seealso cref="Regions"/>
		private IList<PaintingPair<int>>? _regions;

		/// <summary>
		/// The back field of <see cref="Links"/>.
		/// </summary>
		/// <seealso cref="Links"/>
		private IList<PaintingPair<Link>>? _links;

		/// <summary>
		/// The back field of <see cref="DirectLines"/>.
		/// </summary>
		/// <seealso cref="DirectLines"/>
		private IList<PaintingPair<(Cells Start, Cells End)>>? _directLines;


		/// <summary>
		/// The cell information.
		/// </summary>
		/// <value>The value you want to set.</value>
		public IList<PaintingPair<int>>? Cells
		{
			get => _cells;

			set
			{
				_cells = value;

				CellsChanged?.Invoke(value);
			}
		}

		/// <summary>
		/// The candidate information.
		/// </summary>
		/// <value>The value you want to set.</value>
		public IList<PaintingPair<int>>? Candidates
		{
			get => _candidates;

			set
			{
				_candidates = value;

				CandidatesChanged?.Invoke(value);
			}
		}

		/// <summary>
		/// The region information.
		/// </summary>
		/// <value>The value you want to set.</value>
		public IList<PaintingPair<int>>? Regions
		{
			get => _regions;

			set
			{
				_regions = value;

				RegionsChanged?.Invoke(value);
			}
		}

		/// <summary>
		/// The link information.
		/// </summary>
		/// <value>The value you want to set.</value>
		public IList<PaintingPair<Link>>? Links
		{
			get => _links;

			set
			{
				_links = value;

				LinksChanged?.Invoke(value);
			}
		}

		/// <summary>
		/// The direct line information.
		/// </summary>
		/// <value>The value you want to set.</value>
		public IList<PaintingPair<(Cells Start, Cells End)>>? DirectLines
		{
			get => _directLines;

			set
			{
				_directLines = value;

				DirectLinesChanged?.Invoke(value);
			}
		}


		/// <summary>
		/// Indicates the event triggered when the cell list is changed.
		/// </summary>
		public event PresentationDataChangedEventHandler<int>? CellsChanged;

		/// <summary>
		/// Indicates the event triggered when the candidate list is changed.
		/// </summary>
		public event PresentationDataChangedEventHandler<int>? CandidatesChanged;

		/// <summary>
		/// Indicates the event triggered when the region list is changed.
		/// </summary>
		public event PresentationDataChangedEventHandler<int>? RegionsChanged;

		/// <summary>
		/// Indicates the event triggered when the link list is changed.
		/// </summary>
		public event PresentationDataChangedEventHandler<Link>? LinksChanged;

		/// <summary>
		/// Indicates the event triggered when the direct line list is changed.
		/// </summary>
		public event PresentationDataChangedEventHandler<(Cells Start, Cells End)>? DirectLinesChanged;


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="cells">(<see langword="out"/> parameter) The cells.</param>
		/// <param name="candidates">(<see langword="out"/> parameter) The candidates.</param>
		/// <param name="regions">(<see langword="out"/> parameter) The regions.</param>
		/// <param name="links">(<see langword="out"/> parameter) The links.</param>
		/// <param name="directLines">(<see langword="out"/> parameter) The direct lines.</param>
		public void Deconstruct(
			out IList<PaintingPair<int>>? cells, out IList<PaintingPair<int>>? candidates,
			out IList<PaintingPair<int>>? regions, out IList<PaintingPair<Link>>? links,
			out IList<PaintingPair<(Cells Start, Cells End)>>? directLines)
		{
			cells = Cells;
			candidates = Candidates;
			regions = Regions;
			links = Links;
			directLines = DirectLines;
		}

		/// <summary>
		/// To clear the view.
		/// </summary>
		public void Clear()
		{
			Cells = null;
			Candidates = null;
			Regions = null;
			Links = null;
			DirectLines = null;
		}

		/// <summary>
		/// Add a new instance into the collection.
		/// </summary>
		/// <typeparam name="T">The type of the value to add into.</typeparam>
		/// <param name="propertyName">The property name.</param>
		/// <param name="value">The value to add into.</param>
		public bool Add<T>(string propertyName, in T value) where T : unmanaged
		{
			switch (propertyName)
			{
				case nameof(Cells) when value is PaintingPair<int> i:
				{
					(Cells ??= new List<PaintingPair<int>>()).Add(i);
					CellsChanged?.Invoke(Cells);
					return true;
				}
				case nameof(Candidates) when value is PaintingPair<int> i:
				{
					(Candidates ??= new List<PaintingPair<int>>()).Add(i);
					CandidatesChanged?.Invoke(Candidates);
					return true;
				}
				case nameof(Regions) when value is PaintingPair<int> i:
				{
					(Regions ??= new List<PaintingPair<int>>()).Add(i);
					RegionsChanged?.Invoke(Regions);
					return true;
				}
				case nameof(Links) when value is PaintingPair<Link> i:
				{
					(Links ??= new List<PaintingPair<Link>>()).Add(i);
					LinksChanged?.Invoke(Links);
					return true;
				}
				case nameof(DirectLines) when value is PaintingPair<(Cells, Cells)> i:
				{
					(DirectLines ??= new List<PaintingPair<(Cells, Cells)>>()).Add(i);
					DirectLinesChanged?.Invoke(DirectLines);
					return true;
				}
				default:
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Add a series of elements into the collection.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="propertyName">The name of the property you want to add into.</param>
		/// <param name="values">The values you want to add.</param>
		/// <returns>The number of elements that is successful to add.</returns>
		public int AddRange<T>(string propertyName, IEnumerable<T> values) where T : unmanaged
		{
			int result = 0;
			byte tag = 0;
			foreach (var value in values)
			{
				switch (propertyName)
				{
					case nameof(Cells) when value is PaintingPair<int> i:
					{
						(Cells ??= new List<PaintingPair<int>>()).Add(i);
						if (tag == 0)
						{
							tag = 1;
						}

						result++;

						break;
					}
					case nameof(Candidates) when value is PaintingPair<int> i:
					{
						(Candidates ??= new List<PaintingPair<int>>()).Add(i);
						if (tag == 0)
						{
							tag = 2;
						}

						result++;

						break;
					}
					case nameof(Regions) when value is PaintingPair<int> i:
					{
						(Regions ??= new List<PaintingPair<int>>()).Add(i);
						if (tag == 0)
						{
							tag = 3;
						}

						result++;

						break;
					}
					case nameof(Links) when value is PaintingPair<Link> i:
					{
						(Links ??= new List<PaintingPair<Link>>()).Add(i);
						if (tag == 0)
						{
							tag = 4;
						}

						result++;

						break;
					}
					case nameof(DirectLines) when value is PaintingPair<(Cells, Cells)> i:
					{
						(DirectLines ??= new List<PaintingPair<(Cells, Cells)>>()).Add(i);
						if (tag == 0)
						{
							tag = 5;
						}

						result++;

						break;
					}
				}
			}

			// Trigger the corresponding event.
			switch (tag)
			{
				case 1: CellsChanged?.Invoke(Cells); break;
				case 2: CandidatesChanged?.Invoke(Candidates); break;
				case 3: RegionsChanged?.Invoke(Regions); break;
				case 4: LinksChanged?.Invoke(Links); break;
				case 5: DirectLinesChanged?.Invoke(DirectLines); break;
			}

			return result;
		}

		/// <summary>
		/// Remove a new instance from the collection.
		/// </summary>
		/// <typeparam name="T">The type of the value to remove.</typeparam>
		/// <param name="propertyName">The property name.</param>
		/// <param name="value">The value to remove.</param>
		public bool Remove<T>(string propertyName, in T value) where T : unmanaged
		{
			switch (propertyName)
			{
				case nameof(Cells) when value is PaintingPair<int> i && Cells is not null:
				{
					Cells.Remove(i);
					CellsChanged?.Invoke(Cells);
					return true;
				}
				case nameof(Candidates) when value is PaintingPair<int> i && Candidates is not null:
				{
					Candidates.Remove(i);
					CandidatesChanged?.Invoke(Candidates);
					return true;
				}
				case nameof(Regions) when value is PaintingPair<int> i && Regions is not null:
				{
					Regions.Remove(i);
					RegionsChanged?.Invoke(Regions);
					return true;
				}
				case nameof(Links) when Links is not null:
				{
					switch (value)
					{
						case int i:
						{
							for (int z = 0; z < Links.Count; z++)
							{
								if (Links[z].Value.StartCandidate == i)
								{
									Links.RemoveAt(z);
									LinksChanged?.Invoke(Links);
									return true;
								}
							}

							return false;
						}
						case PaintingPair<Link> i:
						{
							Links.Remove(i);
							LinksChanged?.Invoke(Links);
							return true;
						}
					}

					return false;
				}
				case nameof(DirectLines) when value is PaintingPair<(Cells, Cells)> i && DirectLines is not null:
				{
					DirectLines.Remove(i);
					DirectLinesChanged?.Invoke(DirectLines);
					return true;
				}
				default:
				{
					return false;
				}
			}
		}
	}
}
