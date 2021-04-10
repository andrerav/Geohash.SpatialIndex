using NetTopologySuite.Geometries;

namespace Geohash.SpatialIndex.Core
{
	public class IndexEntry<T>
	{
		public T Value;
		public Geometry Geom;
	}
}
