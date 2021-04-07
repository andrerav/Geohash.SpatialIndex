using Geohash.SpatialIndex.Core;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geohash.SpatialIndex.Relations
{
	public class GeohashSpatialIndexRelations<T> : IGeohashSpatialIndexRelations<T>
	{
		private IGeohashSpatialIndex<T> _geohashSpatialIndex;

		public GeohashSpatialIndexRelations(IGeohashSpatialIndex<T> geohashSpatialIndex)
		{
			_geohashSpatialIndex = geohashSpatialIndex;
		}

		/// <summary>
		/// A simple k-nearest-neighbour search
		/// </summary>
		/// <param name="geom"></param>
		/// <param name="k"></param>
		/// <returns></returns>
		public IEnumerable<GeohashIndexEntry<T>> KNN(int k, Geometry geom,
			Func<GeohashIndexEntry<T>, double> distanceMetric,
			T exclude = default)
		{
			var (trieMapSearchResult, reducedHash) = _geohashSpatialIndex.Query(geom, k, exclude: exclude);
			var result = trieMapSearchResult.ToList();
			foreach (var neighbour in _geohashSpatialIndex.Geohasher.Neighbours(reducedHash))
			{
				var (neighbourSearchResult, _) = _geohashSpatialIndex.Query(neighbour, lockPrecision: true);
				result.AddRange(neighbourSearchResult.ToList());
			}

			return result.OrderBy(g => distanceMetric(g)).Take(k).ToList();
		}

		/// <summary>
		/// Returns all geometries in the index that contains the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		public IEnumerable<GeohashIndexEntry<T>> STContains(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Contains(ie.Geom)).ToList();
		}

		/// <summary>
		/// Returns all geometries in the index that properly contains the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		public IEnumerable<GeohashIndexEntry<T>> STContainsProperly(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.ContainsProperly(ie.Geom)).ToList();
		}

		/// <summary>
		/// Returns all geometries in the index that crosses with the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		public IEnumerable<GeohashIndexEntry<T>> STCrosses(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Crosses(ie.Geom)).ToList();
		}

		/// <summary>
		/// Returns all geometries in the index have an exactly equal geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		public IEnumerable<GeohashIndexEntry<T>> STEquals(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Equals(ie.Geom)).ToList();
		}

		/// <summary>
		/// Returns all geometries in the index that intersects with the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		public IEnumerable<GeohashIndexEntry<T>> STIntersects(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Intersects(ie.Geom)).ToList();
		}

		/// <summary>
		/// Returns the closest geometry from the index
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		public GeohashIndexEntry<T> STNearestNeighbour(Geometry geom, 
			Func<GeohashIndexEntry<T>, double> distanceMetric,
			T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			return trieMapSearchResult.OrderBy(g => distanceMetric(g)).FirstOrDefault();
		}

		/// <summary>
		/// Returns all geometries in the index that overlaps with the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		public IEnumerable<GeohashIndexEntry<T>> STOverlaps(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Overlaps(ie.Geom)).ToList();
		}

		/// <summary>
		/// Returns all geometries in the index that touches the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		public IEnumerable<GeohashIndexEntry<T>> STTouches(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Touches(ie.Geom)).ToList();
		}

		/// <summary>
		/// Returns all geometries in the index that is within the given geometry
		/// </summary>
		/// <param name="geom"></param>
		/// <param name="exclude"></param>
		/// <returns></returns>
		public IEnumerable<GeohashIndexEntry<T>> STWithin(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Within(ie.Geom)).ToList();
		}
	}
}