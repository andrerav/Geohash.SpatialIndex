using Geohash.SpatialIndex.Core;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace Geohash.SpatialIndex.Relations
{
	public interface IGeohashSpatialIndexRelations<T>
	{
		IEnumerable<GeohashIndexEntry<T>> KNN(int k, Geometry geom, Func<GeohashIndexEntry<T>, double> distanceMetric, T exclude = default);
		IEnumerable<GeohashIndexEntry<T>> STContains(Geometry geom, T exclude = default);
		IEnumerable<GeohashIndexEntry<T>> STContainsProperly(Geometry geom, T exclude = default);
		IEnumerable<GeohashIndexEntry<T>> STCrosses(Geometry geom, T exclude = default);
		IEnumerable<GeohashIndexEntry<T>> STEquals(Geometry geom, T exclude = default);
		IEnumerable<GeohashIndexEntry<T>> STIntersects(Geometry geom, T exclude = default);
		GeohashIndexEntry<T> STNearestNeighbour(Geometry geom, Func<GeohashIndexEntry<T>, double> distanceMetric, T exclude = default);
		IEnumerable<GeohashIndexEntry<T>> STOverlaps(Geometry geom, T exclude = default);
		IEnumerable<GeohashIndexEntry<T>> STTouches(Geometry geom, T exclude = default);
		IEnumerable<GeohashIndexEntry<T>> STWithin(Geometry geom, T exclude = default);
	}
}