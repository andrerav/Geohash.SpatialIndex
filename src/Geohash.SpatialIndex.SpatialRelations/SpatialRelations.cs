using Geohash.SpatialIndex.Core;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geohash.SpatialIndex.SpatialRelations
{
	public class SpatialRelations<T> : ISpatialRelations<T>
	{
		private IGeohashSpatialIndex<T> _geohashSpatialIndex;

		public SpatialRelations(IGeohashSpatialIndex<T> geohashSpatialIndex)
		{
			_geohashSpatialIndex = geohashSpatialIndex;
		}

		///<inheritdoc/>
		public IEnumerable<IndexEntry<T>> KNN(int k, Geometry geom,
			Func<IndexEntry<T>, double> distanceMetric,
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

		///<inheritdoc/>
		public IEnumerable<IndexEntry<T>> STContains(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, minimumHits:0, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Contains(ie.Geom)).ToList();
		}

		///<inheritdoc/>
		public IEnumerable<IndexEntry<T>> STContainsProperly(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, minimumHits: 0, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.ContainsProperly(ie.Geom)).ToList();
		}

		///<inheritdoc/>
		public IEnumerable<IndexEntry<T>> STCrosses(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Crosses(ie.Geom)).ToList();
		}

		///<inheritdoc/>
		public IEnumerable<IndexEntry<T>> STEquals(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, minimumHits: 0, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Equals(ie.Geom)).ToList();
		}

		///<inheritdoc/>
		public IEnumerable<IndexEntry<T>> STIntersects(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			if (geom is Point)
			{
				return trieMapSearchResult.Where(ie => geom.Intersects(ie.Geom)).ToList();
			}
			else
			{
				var preparedGeom = PreparedGeometryFactory.Prepare(geom);
				return trieMapSearchResult.Where(ie => preparedGeom.Intersects(ie.Geom)).ToList();
			}
		}

		///<inheritdoc/>
		public IndexEntry<T> STNearestNeighbour(Geometry geom,
			Func<IndexEntry<T>, double> distanceMetric,
			T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			return trieMapSearchResult.OrderBy(g => distanceMetric(g)).FirstOrDefault();
		}

		///<inheritdoc/>
		public IEnumerable<IndexEntry<T>> STOverlaps(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Overlaps(ie.Geom)).ToList();
		}

		///<inheritdoc/>
		public IEnumerable<IndexEntry<T>> STTouches(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, _) = _geohashSpatialIndex.Query(geom, exclude: exclude);
			var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult.Where(ie => preparedGeom.Touches(ie.Geom)).ToList();
		}

		///<inheritdoc/>
		public IEnumerable<IndexEntry<T>> STWithin(Geometry geom, T exclude = default)
		{
			var (trieMapSearchResult, hash) = _geohashSpatialIndex.Query(geom, minimumHits:0, exclude: exclude);
			//var preparedGeom = PreparedGeometryFactory.Prepare(geom);
			return trieMapSearchResult
				.Where(ie => ie.Geom.Within(geom)).ToList();
		}
	}
}