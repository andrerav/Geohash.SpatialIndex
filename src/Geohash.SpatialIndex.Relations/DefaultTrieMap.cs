using Geohash.SpatialIndex.Core;
using rm.Trie;
using System.Collections.Generic;

namespace Geohash.SpatialIndex.Relations
{
	/// <summary>
	/// A default implementation of a trie map using the rm.Trie nuget package from https://www.nuget.org/packages/rm.Trie
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DefaultTrieMap<T> : IGeohashTrieMap<GeohashIndexEntryList<T>, T>
	{
		private TrieMap<GeohashIndexEntryList<T>> trieMap = new TrieMap<GeohashIndexEntryList<T>>();

		public void Add(string key, GeohashIndexEntry<T> entry)
		{
			GeohashIndexEntryList<T> trieMapEntryList;
			if (trieMap.HasKey(key))
			{
				trieMapEntryList = Get(key);
			}
			else
			{
				trieMapEntryList = new GeohashIndexEntryList<T>();
				trieMap.Add(key, trieMapEntryList);
			}
			trieMapEntryList.IndexEntries.Add(entry);
		}

		public GeohashIndexEntryList<T> Get(string key)
		{
			return trieMap.ValueBy(key);
		}

		public bool HasKey(string key)
		{
			return trieMap.HasKey(key);
		}

		public bool HasPrefix(string prefix)
		{
			return trieMap.HasKeyPrefix(prefix);
		}

		public void Remove(string key)
		{
			trieMap.Remove(key);
		}

		public void Remove(string key, T value)
		{
			if (trieMap.HasKey(key))
			{
				var entry = trieMap.ValueBy(key);
				if (entry != null)
				{
					entry.IndexEntries.RemoveAll(entry => entry.Value.Equals(value));
				}
			}
		}

		public void RemovePrefix(string prefix)
		{
			trieMap.RemoveKeyPrefix(prefix);
		}

		public IEnumerable<GeohashIndexEntryList<T>> Search(string prefix)
		{
			return trieMap.ValuesBy(prefix);
		}
	}
}