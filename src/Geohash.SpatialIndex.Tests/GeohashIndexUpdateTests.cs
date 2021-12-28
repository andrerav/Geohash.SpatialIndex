using Geohash.SpatialIndex.Core;
using Geohash.SpatialIndex.SpatialRelations;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using System.Linq;

namespace Geohash.SpatialIndex.Tests
{
	public class GeohashIndexUpdateTests
	{
		protected GeohashSpatialIndex<int> emptyIndex;

		[SetUp]
		public void Setup()
		{
			emptyIndex = new GeohashSpatialIndex<int>(new DefaultGeohasher(), new DefaultTrieMap<int>(), 9);
		}

		[Test]
		public void TestRemoveSingleEntryByValue()
		{
			emptyIndex.Insert(new Point(1, 2), 1);
			emptyIndex.Insert(new Point(2, 1), 2);
			var firstResult = emptyIndex.Query(string.Empty).Entries;
			Assert.IsTrue(firstResult.Count() == 2);
			Assert.AreEqual(1, firstResult.First().Value);
			Assert.AreEqual(2, firstResult.Skip(1).Single().Value);


			emptyIndex.Remove(1);
			var secondResult = emptyIndex.Query(string.Empty).Entries;
			Assert.IsTrue(secondResult.Count() == 1);
			Assert.AreEqual(2, secondResult.Single().Value);
		}

		[Test]
		public void TestRemoveSingleEntryOnSameLocationByValue()
		{
			emptyIndex.Insert(new Point(1, 2), 1);
			emptyIndex.Insert(new Point(1, 2), 2);

			var firstResult = emptyIndex.Query(string.Empty).Entries;
			Assert.IsTrue(firstResult.Count() == 2);
			Assert.AreEqual(1, firstResult.First().Value);
			Assert.AreEqual(2, firstResult.Skip(1).First().Value);

			emptyIndex.Remove(1);

			var secondResult = emptyIndex.Query(string.Empty).Entries;
			Assert.IsTrue(secondResult.Count() == 1);
			Assert.AreEqual(2, secondResult.First().Value);
		}


		[Test]
		public void TestInsertOrUpdate()
		{
			emptyIndex.InsertOrUpdate(new Point(1, 2), 1);
			emptyIndex.InsertOrUpdate(new Point(2, 1), 1);
			emptyIndex.InsertOrUpdate(new Point(1, 2), 2);
			emptyIndex.InsertOrUpdate(new Point(2, 1), 2);

			var result = emptyIndex.Query(string.Empty).Entries;
			Assert.IsTrue(result.Count() == 2);
			Assert.AreEqual(1, result.First().Value);
			Assert.AreEqual(2, result.Skip(1).First().Value);
		}


		[Test]
		public void TestRemoveAllIndexEntriesAlsoRemovesKey()
		{
			emptyIndex.InsertOrUpdate(new Point(1, 2), 1);
			emptyIndex.InsertOrUpdate(new Point(2, 1), 2);

			var result = emptyIndex.Query(string.Empty).Entries;
			Assert.IsTrue(result.Count() == 2);
			Assert.IsTrue(emptyIndex.TrieMap.Count() == 2);
			emptyIndex.Remove(1);
			Assert.IsTrue(emptyIndex.TrieMap.Count() == 1);

		}

		[Test]
		public void TestInsertOrUpdateMovesValue()
		{
			emptyIndex.InsertOrUpdate(new Point(1, 1), 1);
			emptyIndex.InsertOrUpdate(new Point(1, 2), 2);

			var firstResult = emptyIndex.Query(string.Empty).Entries;
			Assert.IsTrue(firstResult.Count() == 2);
			Assert.IsTrue(emptyIndex.TrieMap.Count() == 2);
			Assert.AreEqual(1, firstResult.First().Value);
			Assert.AreEqual(2, firstResult.Skip(1).First().Value);

			emptyIndex.InsertOrUpdate(new Point(1, 1), 2);

			var secondResult = emptyIndex.Query(string.Empty).Entries;
			Assert.IsTrue(secondResult.Count() == 2);
			Assert.IsTrue(emptyIndex.TrieMap.Count() == 1);
			Assert.AreEqual(1, secondResult.First().Value);
			Assert.AreEqual(2, secondResult.Skip(1).First().Value);

		}

	}
}