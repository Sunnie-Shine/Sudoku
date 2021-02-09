using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Chaining;

namespace Sudoku.Solving.Manual.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Node"/>.
	/// </summary>
	/// <seealso cref="Node"/>
	public static class NodeEx
	{
		/// <summary>
		/// Get highlight candidate offsets through the specified target node.
		/// </summary>
		/// <param name="target">(<see langword="this in"/> parameter) The target node.</param>
		/// <param name="simpleChain">
		/// Indicates whether the current caller is in <see cref="AicStepSearcher"/>. The
		/// default value is <see langword="false"/>.
		/// </param>
		/// <returns>The candidate offsets.</returns>
		public static IList<PaintingPair<int>> GetCandidateOffsets(this in Node target, bool simpleChain = false)
		{
			var result = new List<PaintingPair<int>>();
			var chain = target.Chain;
			var (pCandidate, _) = chain[0];
			if (!simpleChain)
			{
				result.Add(new(2, pCandidate));
			}

			for (int i = 0, count = chain.Count; i < count; i++)
			{
				var p = chain[i];
				if (p.Parents is { } parents)
				{
					foreach (var pr in parents)
					{
						var (prCandidate, prIsOn) = pr;
						if (simpleChain && i != count - 2 || !simpleChain)
						{
							result.AddIfDoesNotContain(new(prIsOn ? 1 : 0, prCandidate));
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Get the links through the specified target node.
		/// </summary>
		/// <param name="target">(<see langword="this in"/> parameter) The target node.</param>
		/// <param name="showAllLinks">
		/// Indicates whether the current chain will display all chains (even contains the weak links
		/// from the elimination node). The default value is <see langword="false"/>. If you want to
		/// draw the AIC, the elimination weak links don't need drawing.
		/// </param>
		/// <returns>The link.</returns>
		public static IReadOnlyList<PaintingPair<Link>> GetLinks(this in Node target, bool showAllLinks = false)
		{
			var result = new List<PaintingPair<Link>>();
			var chain = target.Chain;
			for (int i = showAllLinks ? 0 : 1, count = chain.Count - (showAllLinks ? 0 : 2); i < count; i++)
			{
				var p = chain[i];
				var (pCandidate, pIsOn) = p;
				for (int j = 0, parentsCount = p.Parents?.Count ?? 0; j < parentsCount; j++)
				{
					var (prCandidate, prIsOn) = p.Parents![j];
					result.Add(
						new(
							0,
							new(
								pCandidate,
								prCandidate,
								(prIsOn, pIsOn) switch
								{
									(false, true) => LinkType.Strong,
									(true, false) => LinkType.Weak,
									_ => LinkType.Default
								})));
				}
			}

			return result;
		}
	}
}
