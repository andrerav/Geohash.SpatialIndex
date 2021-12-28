using NetTopologySuite.Geometries;
using System;
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
		private Dictionary<T, IndexEntry<T>> _reverseIndex = new Dictionary<T, IndexEntry<T>>();

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
			if (_reverseIndex.ContainsKey(value))
			{
				throw new InvalidOperationException($"This value already exists in the index. Use {nameof(InsertOrUpdate)} instead");
			}

			var key = _geohasher.Encode(geom, _precision);
			var entry = new IndexEntry<T>
			{
				Geom = geom,
				Value = value
			};

			_trieMap.Add(key, entry);
			_reverseIndex[value] = entry;
			return (key, entry);
		}

		///<inheritdoc/>
		public (string, IndexEntry<T>) InsertOrUpdate(Geometry geom, T value)
		{
			if (_reverseIndex.ContainsKey(value))
			{
				var entry = _reverseIndex[value];
				var existingKey = _geohasher.Encode(entry.Geom, _precision);
				var newKey = _geohasher.Encode(geom, _precision);
				if (newKey != existingKey)
				{
					_trieMap.Remove(existingKey, value);
					_reverseIndex.Remove(value);
					return Insert(geom, value);
				}
				else
				{
					return (existingKey, entry);
				}
			}
			else
			{
				// Just add it
				return Insert(geom, value);
			}
		}

		///<inheritdoc/>
		public void Remove(Geometry geom, T value)
		{
			var geohash = _geohasher.Encode(geom, _precision);
			Remove(geohash, value);
		}

		///<inheritdoc/>
		public void Remove(string geohash, T value)
		{
			_trieMap.Remove(geohash, value);
			_reverseIndex.Remove(value);
		}


		///<inheritdoc/>
		public void Remove(T value)
		{
			if (_reverseIndex.ContainsKey(value))
			{
				var entry = _reverseIndex[value];
				var geohash = _geohasher.Encode(entry.Geom, _precision);
				Remove(geohash, value);
			}

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
		public (IEnumerable<IndexEntry<T>> Entries, string Geohash) Query(string geohash, int minimumHits = 1, bool lockPrecision = false, T exclude = default)
		{
			IEnumerable<IndexEntry<T>> entries;
			while (true)
			{
				entries = Search(geohash);
				if (exclude != null && !exclude.Equals(default(T)))
				{
					entries = entries.Where(entry => !entry.Value.Equals(exclude));
				}
				if (entries.Count() >= minimumHits)
				{
					break;
				}
				else if (lockPrecision)
				{
					break;
				}
				else if (geohash.Length == 1)
				{
					break;
				}
				geohash = _geohasher.Reduce(geohash);
			}

			return (entries, geohash);
		}

		private IEnumerable<IndexEntry<T>> Search(string geohash)
		{
			IEnumerable<IndexEntry<T>> entries;
			var trieMapSearchResult = _trieMap.Search(geohash);
			entries = trieMapSearchResult.SelectMany(sr => sr.IndexEntries); // Each entry in the map is a list, so we flatten the result
			return entries;
		}
	}
}