using System.Collections.Generic;

namespace Geohash.SpatialIndex.Core
{
	/// <summary>
	/// Represents an inverted index based on a trie map. This index maps
	/// geohashes to lists of values contained by that geohash. This index can
	/// then be searched using geohash prefixes.
	/// </summary>
	/// <typeparam name="TEntryList"></typeparam>
	/// <typeparam name="T"></typeparam>
	public interface IGeohashTrieMap<TEntryList, T> where TEntryList : GeohashIndexEntryList<T>
	{
		void Add(string key, IndexEntry<T> entry);
		void AddOrUpdate(string key, IndexEntry<T> entry);

		bool HasKey(string key);

		bool HasPrefix(string prefix);

		void Remove(string key);

		void Remove(string key, T value);

		void Remove(T value);

		void RemovePrefix(string prefix);

		IEnumerable<string> Keys();

		TEntryList Get(string key);

		IEnumerable<TEntryList> Search(string prefix);
	}
}