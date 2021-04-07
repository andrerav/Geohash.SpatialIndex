using NetTopologySuite.Geometries;

namespace Geohash.SpatialIndex.Core
{
	public class GeohashIndexEntry<T>
	{
		public T Value;
		public Geometry Geom;
	}
}