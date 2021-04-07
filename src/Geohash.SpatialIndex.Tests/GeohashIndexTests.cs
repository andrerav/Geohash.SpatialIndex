using Geohash.SpatialIndex.Core;
using Geohash.SpatialIndex.Relations;
using Geohash.SpatialIndex.Tests.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geohash.SpatialIndex.Tests
{
	public class GeohashIndexTests
	{
		private List<HarbourPolygon> polygons;
		private GeohashSpatialIndex<int> index;
		private GeohashSpatialIndexRelations<int> relations;

		[OneTimeSetUp]
		public void Setup()
		{
			polygons = TestUtil.ReadCsvFile<HarbourPolygon, HarbourCsvClassMap>("./Resources/harbours_polygons.csv");
			index = new GeohashSpatialIndex<int>(new DefaultGeohasher(), new DefaultTrieMap<int>(), 9);
			relations = new GeohashSpatialIndexRelations<int>(index);
			foreach (var polygon in polygons)
			{
				index.Insert(polygon.Geom, polygon.HarbourId);
			}
		}

		[Test]
		public void TestKnnSearch()
		{
			const int K = 5;
			var success = 0;
			Parallel.ForEach(polygons, polygon =>
			{
				var distanceMetric = new Func<GeohashIndexEntry<int>, double>(entry => entry.Geom.Envelope.Distance(polygon.Geom.Envelope));
				var result = relations.KNN(K, polygon.Geom, distanceMetric, exclude: polygon.HarbourId).ToList();
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
				var result = relations.STIntersects(polygon.Geom, exclude: polygon.HarbourId);
				sum += result.Count();
				Assert.IsTrue(!result.Any(r => r.Value == polygon.HarbourId));
			});
			Assert.IsTrue(sum > 0);
		}

		[Test]
		public void TestContains()
		{
			var sum = 0;
			Parallel.ForEach(polygons, polygon =>
			{
				var result = relations.STContains(polygon.Geom, exclude: polygon.HarbourId);
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
				var distanceMetric = new Func<GeohashIndexEntry<int>, double>(entry => entry.Geom.Envelope.Distance(polygon.Geom.Envelope));
				var result = relations.STNearestNeighbour(polygon.Geom.Envelope, distanceMetric, exclude: polygon.HarbourId);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Value != polygon.HarbourId);
			});
		}
	}
}