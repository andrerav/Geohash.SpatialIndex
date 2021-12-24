using Geohash.SpatialIndex.Core;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace Geohash.SpatialIndex.SpatialRelations
{
	public interface ISpatialRelations<T>
	{
		/// <summary>
		/// Returns all geometries in the index that contains the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		IEnumerable<IndexEntry<T>> STContains(Geometry geom, T exclude = default);

		/// <summary>
		/// Returns all geometries in the index that properly contains the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		IEnumerable<IndexEntry<T>> STContainsProperly(Geometry geom, T exclude = default);

		/// <summary>
		/// Returns all geometries in the index that crosses with the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		IEnumerable<IndexEntry<T>> STCrosses(Geometry geom, T exclude = default);

		/// <summary>
		/// Returns all geometries in the index have an exactly equal geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		IEnumerable<IndexEntry<T>> STEquals(Geometry geom, T exclude = default);

		/// <summary>
		/// Returns all geometries in the index that intersects with the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		IEnumerable<IndexEntry<T>> STIntersects(Geometry geom, T exclude = default);

		/// <summary>
		/// Returns all geometries in the index that overlaps with the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		IEnumerable<IndexEntry<T>> STOverlaps(Geometry geom, T exclude = default);

		/// <summary>
		/// Returns all geometries in the index that touches the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		IEnumerable<IndexEntry<T>> STTouches(Geometry geom, T exclude = default);

		/// <summary>
		/// Returns all geometries in the index that is within the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <param name="exclude"></param>
		/// <returns></returns>
		IEnumerable<IndexEntry<T>> STWithin(Geometry geom, T exclude = default);

		/// <summary>
		/// Returns the closest geometry from the index
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		IndexEntry<T> STNearestNeighbour(Geometry geom, Func<IndexEntry<T>, double> distanceMetric, T exclude = default);

		/// <summary>
		/// A simple k-nearest-neighbour search
		/// </summary>
		/// <param name="geom"></param>
		/// <param name="k"></param>
		/// <returns></returns>
		IEnumerable<IndexEntry<T>> KNN(int k, Geometry geom, Func<IndexEntry<T>, double> distanceMetric, T exclude = default);

	}
}