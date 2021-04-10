using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Geohash.SpatialIndex.Core
{
	/// <summary>
	/// A spatial index for arbitrary geometries backed by a trie map for fast
	/// insertion and lookup. This implementation is not thread safe. This index
	/// provides geometric range queries based on geohash cells. Point geometries
	/// are encoded as-is, while more complex geometries are encoded based on
	/// their envelopes and the minimum geohash precision their corresponding
	/// envelope will fit within.
	/// <para />
	/// This index should be used as a <b>primary filter</b>. The query methods
	/// will return multiple geometries
	/// that may or may not intersect with the query geometry. A secondary filter
	/// is required to test for actual intersection
	/// between the query rectangle and the envelope of each candidate item.
	/// <para />
	/// This implementation does not require specifying the extent of the inserted
	/// items beforehand.  It will automatically expand to accommodate any extent
	/// of dataset that fits within the geohash space.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IGeohashSpatialIndex<T>
	{
		/// <summary>
		/// Add a new entry to the index
		/// </summary>
		/// <param name="geom"></param>
		/// <param name="value"></param>
		(string, IndexEntry<T>) Insert(Geometry geom, T value);

		/// <summary>
		/// Remove the given item from the index
		/// </summary>
		/// <param name="value"></param>
		void Remove(Geometry geom, T value);

		/// <summary>
		/// Search the trie map while gradually reducing the geohash resolution for the given geometry until we find a match.
		/// Default settings are used and only the result from the query is returned.
		/// </summary>
		/// <param name="geom"></param>
		/// <returns>A list of matching geometries that may or may not intersect, contain or overlap the queried geometry within the same geohash cell</returns>
		IEnumerable<IndexEntry<T>> Query(Geometry geom);

		/// <summary>
		/// Search the trie map while gradually reducing the geohash resolution for the given geometry until we find a match.
		/// The reduced hash is returned to allow the caller to perform nearest neighbour searches.
		/// </summary>
		/// <param name="geom">The geometry to search for in the index</param>
		/// <param name="minimumHits">A minimum number of hits before the search is satisfied. Default value is 1.</param>
		/// <param name="lockPrecision">Set this to true to disable precision reduction in the search procedure. This will improve results but may give zero or few results. Default value is false.</param>
		/// <returns>A list of matching geometries that may or may not intersect, contain or overlap the queried geometry</returns>

		(IEnumerable<IndexEntry<T>>, string) Query(Geometry geom, int minimumHits = 1, bool lockPrecision = false, T exclude = default);

		/// <summary>
		/// Search the trie map while gradually reducing the geohash resolution for the given geohash until we find a match.
		/// The reduced hash is returned to allow the caller to perform nearest neighbour searches.
		/// </summary>
		/// <param name="geom">The geometry to search for in the index</param>
		/// <param name="minimumHits">A minimum number of hits before the search is satisfied. Default value is 1.</param>
		/// <param name="lockPrecision">Set this to true to disable precision reduction in the search procedure. This will improve results but may give zero or few results. Default value is false.</param>
		/// <returns>A list of matching geometries that may or may not intersect, contain or overlap the queried geometry</returns>
		(IEnumerable<IndexEntry<T>>, string) Query(string hash, int minimumHits = 1, bool lockPrecision = false, T exclude = default);

		/// <summary>
		/// The geohash implementation for this index
		/// </summary>
		IGeohasher Geohasher { get; }

		/// <summary>
		/// The trie map implementation for this index
		/// </summary>
		IGeohashTrieMap<GeohashIndexEntryList<T>, T> TrieMap { get; }
	}
}