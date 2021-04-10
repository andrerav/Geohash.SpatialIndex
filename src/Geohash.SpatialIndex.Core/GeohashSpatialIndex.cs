﻿using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace Geohash.SpatialIndex.Core
{
	///<inheritdoc/>
	public class GeohashSpatialIndex<T> : IGeohashSpatialIndex<T>
	{
		private int _precision;
		private IGeohasher _geohasher;
		private IGeohashTrieMap<GeohashIndexEntryList<T>, T> _trieMap;

		///<inheritdoc/>
		public IGeohasher Geohasher => _geohasher;

		///<inheritdoc/>
		public IGeohashTrieMap<GeohashIndexEntryList<T>, T> TrieMap => _trieMap;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="geohasher">A geohasher implementation</param>
		/// <param name="trieMap">A trie map implementation</param>
		/// <param name="precision">The default search precision. Depending on your data </param>
		public GeohashSpatialIndex(IGeohasher geohasher, IGeohashTrieMap<GeohashIndexEntryList<T>, T> trieMap, int precision = 9)
		{
			_geohasher = geohasher;
			_trieMap = trieMap;
			_precision = precision;
		}

		///<inheritdoc/>
		public (string, IndexEntry<T>) Insert(Geometry geom, T value)
		{
			var key = _geohasher.Encode(geom, _precision);
			var entry = new IndexEntry<T>
			{
				Geom = geom,
				Value = value
			};
			_trieMap.Add(key, entry);
			return (key, entry);
		}

		///<inheritdoc/>
		public void Remove(Geometry geom, T value)
		{
			var key = _geohasher.Encode(geom, _precision);
			_trieMap.Remove(key, value);
		}

		///<inheritdoc/>
		public IEnumerable<IndexEntry<T>> Query(Geometry geom)
		{
			return Query(_geohasher.Encode(geom, _precision)).Item1;
		}

		///<inheritdoc/>
		public (IEnumerable<IndexEntry<T>>, string) Query(Geometry geom, int minimumHits = 1, bool lockPrecision = false, T exclude = default)
		{
			var result = Query(_geohasher.Encode(geom, _precision), minimumHits, lockPrecision, exclude);
			return result;
		}

		///<inheritdoc/>
		public (IEnumerable<IndexEntry<T>>, string) Query(string hash, int minimumHits = 1, bool lockPrecision = false, T exclude = default)
		{
			IEnumerable<IndexEntry<T>> entries;
			while (true)
			{
				var trieMapSearchResult = _trieMap.Search(hash);
				entries = trieMapSearchResult.SelectMany(sr => sr.IndexEntries); // Each entry in the map is a list, so we flatten the result
				int hitCount;
				if (exclude != null && !exclude.Equals(default(T)))
				{
					entries = entries.Where(entry => !entry.Value.Equals(exclude));
				}
				hitCount = entries.Count();
				if (hitCount >= minimumHits)
				{
					break;
				}
				else if (lockPrecision)
				{
					break;
				}
				else if (hash.Length == 1)
				{
					break;
				}
				hash = _geohasher.Reduce(hash);
			}

			return (entries, hash);
		}
	}
}