using System.Collections.Generic;

namespace Geohash.SpatialIndex.Core
{
	public class GeohashIndexEntryList<T>
	{
		public List<IndexEntry<T>> IndexEntries;

		public GeohashIndexEntryList()
		{
			IndexEntries = new List<IndexEntry<T>>();
		}
	}
}