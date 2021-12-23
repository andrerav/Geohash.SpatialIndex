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

	public class GeohashIndexQueryTests : AbstractGeohashIndexTests
	{
		[Test]
		public void TestKnnSearch()
		{
			const int K = 5;
			var success = 0;
			Parallel.ForEach(polygons, polygon =>
			{
				var distanceMetric = new Func<IndexEntry<int>, double>(entry => entry.Geom.Envelope.Distance(polygon.Geom.Envelope));
				var result = polygonRelations.KNN(K, polygon.Geom, distanceMetric, exclude: polygon.HarbourId).ToList();
				if (result.Count() == K)
				{
					success++;
				}
			});
			Assert.IsTrue(success > 0);
		}

		[Test]
		public void TestIntersection()
		{
			var sum = 0;

			Parallel.ForEach(polygons, polygon =>
			{
				var result = polygonRelations.STIntersects(polygon.Geom, exclude: polygon.HarbourId);
				sum += result.Count();
				Assert.IsTrue(!result.Any(r => r.Value == polygon.HarbourId));
			});
			Assert.IsTrue(sum > 0);
		}

		[Test]
		public void TestPositionIntersection()
		{
			var sum = 0;

			Parallel.ForEach(vesselPositions, position =>
			{
				var result = polygonRelations.STIntersects(position.Geom);
				sum += result.Count();
			});
			Assert.IsTrue(sum > 0);
		}

		[Test]
		public void TestContains()
		{
			var sum = 0;
			Parallel.ForEach(polygons, polygon =>
			{
				var result = polygonRelations.STContains(polygon.Geom, exclude: polygon.HarbourId);
				sum += result.Count();
				Assert.IsTrue(!result.Any(r => r.Value == polygon.HarbourId));
			});
			Assert.IsTrue(sum > 0);
		}

		[Test]
		public void TestNearestNeighbour()
		{
			Parallel.ForEach(polygons, polygon =>
			{
				var distanceMetric = new Func<IndexEntry<int>, double>(entry => entry.Geom.Envelope.Distance(polygon.Geom.Envelope));
				var result = polygonRelations.STNearestNeighbour(polygon.Geom.Envelope, distanceMetric, exclude: polygon.HarbourId);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Value != polygon.HarbourId);
			});
		}

		[Test]
		public void TestPositionNearestNeighbour()
		{
			Parallel.ForEach(vesselPositions, position =>
			{
				var distanceMetric = new Func<IndexEntry<int>, double>(entry => entry.Geom.Envelope.Distance(position.Geom));
				var result = polygonRelations.STNearestNeighbour(position.Geom, distanceMetric);
				Assert.IsNotNull(result);
			});
		}

		[Test]
		public void TestPositionsWithinPolygon()
		{
			int sum = 0;
			Parallel.ForEach(polygons, polygon =>
			{
				var result = pointRelations.STWithin(polygon.Geom);
				sum += result.Count();
				Assert.IsNotNull(result);
			});
			Assert.IsTrue(sum > 0);
		}

		[Test]
		public void TestDumpIndex()
		{
			var dump = pointIndex.DumpIndex();
			Assert.IsTrue(dump.Count() > 0);
		}

		[Test]
		public void TestToCsv()
		{
			pointIndex.ToCsv("./../../../PointIndex.csv");
			polygonIndex.ToCsv("./../../../PolygonIndex.csv");
		}

		[Test]
		public void TestSpecificCellCounts()
		{
			// 5: 3622
			// 6: 20
			// 7: 
			var cell = "u4et7z";
			var result = pointIndex.Query(cell).Item1.ToList();
			Assert.AreEqual(result.Count(), 20);
		}
	}
}