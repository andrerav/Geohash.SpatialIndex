using System.Collections.Generic;

namespace Geohash.SpatialIndex.Core
{
	public class GeohashIndexEntryList<T>
	{
		public List<GeohashIndexEntry<T>> IndexEntries;

		public GeohashIndexEntryList()
		{
			IndexEntries = new List<GeohashIndexEntry<T>>();
		}
	}
}