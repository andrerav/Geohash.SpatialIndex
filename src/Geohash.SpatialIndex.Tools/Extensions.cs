using Geohash.SpatialIndex.Core;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Geohash.SpatialIndex.Tools
{
	public static class Extensions
	{
		/// <summary>
		/// Returns a list of index keys and their associated bounding boxes as a geometry.
		/// This extension is useful for analyzing an index after it has been built.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="index"></param>
		/// <returns></returns>
		public static IEnumerable<(string, Polygon)> DumpIndex<T>(this GeohashSpatialIndex<T> index)
		{
			var keys = index.TrieMap.Keys().Distinct().ToList();
			foreach(var key in keys.ToList())
			{
				var hash = key;
				while(true)
				{
					if (hash.Length == 1)
					{
						break;
					}
					hash = index.Geohasher.Reduce(hash);
					keys.Add(hash);
				}
			}
			return keys.Distinct().OrderBy(k => k).Select(key => (key, index.Geohasher.BoundingBox(key))).ToList();
		}

		/// <summary>
		/// Serializes this index to CSV and writes the result to a file
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="index"></param>
		/// <returns></returns>
		public static void ToCsv<T>(this GeohashSpatialIndex<T> index, string path, string delimiter = ";")
		{
			var indexDump = index.DumpIndex();
			using (var sw = new StreamWriter(path, false))
			{
				sw.WriteLine($"geohash{delimiter}geometry");
				foreach(var entry in indexDump)
				{
					sw.WriteLine(entry.Item1 + delimiter + entry.Item2.AsText());
				}
			}
		}

		/// <summary>
		/// Analyzes and rebalances the index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static GeohashSpatialIndex<T> Balance<T>(this GeohashSpatialIndex<T> index)
		{
			return index;
		}
	}
}
