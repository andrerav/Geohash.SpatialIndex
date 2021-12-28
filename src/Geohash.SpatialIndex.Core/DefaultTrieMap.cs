using rm.Trie;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geohash.SpatialIndex.Core
{
	/// <summary>
	/// A default implementation of a trie map using the rm.Trie nuget package from https://www.nuget.org/packages/rm.Trie
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DefaultTrieMap<T> : IGeohashTrieMap<GeohashIndexEntryList<T>, T>
	{
		private TrieMap<GeohashIndexEntryList<T>> trieMap = new TrieMap<GeohashIndexEntryList<T>>();

		///<inheritdoc/>
		public void Add(string key, IndexEntry<T> entry)
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

		///<inheritdoc/>
		public void AddOrUpdate(string key, IndexEntry<T> entry)
		{
			Remove(entry.Value);
			Add(key, entry);
		}

		public int Count()
		{
			return trieMap.Keys().Count();
		}

		///<inheritdoc/>
		public GeohashIndexEntryList<T> Get(string key)
		{
			return trieMap.ValueBy(key);
		}

		///<inheritdoc/>
		public bool HasKey(string key)
		{
			return trieMap.HasKey(key);
		}

		///<inheritdoc/>
		public bool HasPrefix(string prefix)
		{
			return trieMap.HasKeyPrefix(prefix);
		}

		///<inheritdoc/>
		public IEnumerable<string> Keys()
		{
			return trieMap.Keys();
		}

		///<inheritdoc/>
		public void Remove(string key)
		{
			trieMap.Remove(key);
		}

		///<inheritdoc/>
		public void Remove(string key, T value)
		{
			if (trieMap.HasKey(key))
			{
				var entry = trieMap.ValueBy(key);
				if (entry != null)
				{
					entry.IndexEntries.RemoveAll(ie => ie.Value.Equals(value));
				}

				//Remove entire key if there are no entries anymore
				if (entry.IndexEntries.Count() == 0)
				{
					trieMap.Remove(key);
				}

			}
		}

		///<inheritdoc/>
		public void Remove(T value)
		{
			var indexEntryListList = trieMap.KeyValuePairs()
										.Where(v => v.Value.IndexEntries.Any(ie => ie.Value.Equals(value)))
										.ToList();

			foreach (var kvpList in indexEntryListList)
			{
				// If it's a single entry or all entries are equal to this value then we
				// can remove the entire key from the index
				if (kvpList.Value.IndexEntries.Count() == 1
					|| kvpList.Value.IndexEntries.All(ie => ie.Value.Equals(value)))
				{
					trieMap.Remove(kvpList.Key);
				}
				else
				{
					kvpList.Value.IndexEntries.RemoveAll(ie => ie.Value.Equals(value));
				}
			}
		}

		///<inheritdoc/>
		public void RemovePrefix(string prefix)
		{
			trieMap.RemoveKeyPrefix(prefix);
		}

		///<inheritdoc/>
		public IEnumerable<GeohashIndexEntryList<T>> Search(string prefix)
		{
			return trieMap.ValuesBy(prefix);
		}

	}
}