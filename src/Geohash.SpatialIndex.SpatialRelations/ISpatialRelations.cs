using Geohash.SpatialIndex.Core;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace Geohash.SpatialIndex.SpatialRelations
{
	public interface ISpatialRelations<T>
	{
		IEnumerable<IndexEntry<T>> STContains(Geometry geom, T exclude = default);
		IEnumerable<IndexEntry<T>> STContainsProperly(Geometry geom, T exclude = default);
		IEnumerable<IndexEntry<T>> STCrosses(Geometry geom, T exclude = default);
		IEnumerable<IndexEntry<T>> STEquals(Geometry geom, T exclude = default);
		IEnumerable<IndexEntry<T>> STIntersects(Geometry geom, T exclude = default);
		IEnumerable<IndexEntry<T>> STOverlaps(Geometry geom, T exclude = default);
		IEnumerable<IndexEntry<T>> STTouches(Geometry geom, T exclude = default);
		IEnumerable<IndexEntry<T>> STWithin(Geometry geom, T exclude = default);
		IndexEntry<T> STNearestNeighbour(Geometry geom, Func<IndexEntry<T>, double> distanceMetric, T exclude = default);
		IEnumerable<IndexEntry<T>> KNN(int k, Geometry geom, Func<IndexEntry<T>, double> distanceMetric, T exclude = default);

	}
}