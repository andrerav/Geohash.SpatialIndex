using Geohash.SpatialIndex.Core;
using Geohash.SpatialIndex.SpatialRelations;
using Geohash.SpatialIndex.Tests.Models;
using Geohash.SpatialIndex.Tools;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geohash.SpatialIndex.Tests
{
	public class AbstractGeohashIndexTests
	{
		protected List<HarbourPolygon> polygons;
		protected List<VesselPosition> vesselPositions;
		protected GeohashSpatialIndex<int> polygonIndex;
		protected GeohashSpatialIndex<int> pointIndex;
		protected SpatialRelations<int> polygonRelations;
		protected SpatialRelations<int> pointRelations;

		[OneTimeSetUp]
		public void Setup()
		{
			polygons = TestUtil.ReadCsvFile<HarbourPolygon, HarbourCsvClassMap>("./Resources/harbours_polygons.csv");
			vesselPositions = TestUtil.ReadCsvFile<VesselPosition, VesselPositionCsvClassMap>("./Resources/vessel_points.csv")
									.Where(position => !(position.Geom.Coordinate.Y > 90 || position.Geom.Coordinate.Y < -90
															|| position.Geom.Coordinate.X > 180 || position.Geom.Coordinate.X < -180
															|| position.Sog > 100 || position.Cog == 360)).ToList();
			polygonIndex = new GeohashSpatialIndex<int>(new DefaultGeohasher(), new DefaultTrieMap<int>(), 9);
			pointIndex = new GeohashSpatialIndex<int>(new DefaultGeohasher(), new DefaultTrieMap<int>(), 8);
			polygonRelations = new SpatialRelations<int>(polygonIndex);
			pointRelations = new SpatialRelations<int>(pointIndex);
			foreach (var polygon in polygons)
			{
				polygonIndex.Insert(polygon.Geom, polygon.HarbourId);
			}

			foreach (var position in vesselPositions.ToList())
			{
				pointIndex.Insert(position.Geom, position.Mmsi);
			}

		}
	}
}