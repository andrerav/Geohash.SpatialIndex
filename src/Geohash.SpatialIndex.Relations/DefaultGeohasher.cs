using Geohash.SpatialIndex.Core;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace Geohash.SpatialIndex.Relations
{
	/// <summary>
	/// A default geohasher implementation using the geohash-dotnet nuget package from https://www.nuget.org/packages/geohash-dotnet/
	/// </summary>
	public class DefaultGeohasher : IGeohasher
	{
		private Geohasher hasher = new Geohasher();

		public Point Decode(string geohash)
		{
			var result = hasher.Decode(geohash);
			return new Point(result.Item2, result.Item1);
		}

		public string Encode(Geometry geom, int precision)
		{
			var hash = GeohashIndexUtil.LongestCommonPrefix(geom.Envelope.Coordinates.Select(coord => hasher.Encode(coord.Y, coord.X, precision)).ToArray());
			return hash;
		}

		public Polygon BoundingBox(string geohash)
		{
			var bb = hasher.GetBoundingBox(geohash);

			var coordinates = new List<Coordinate>();
			coordinates.Add(new Coordinate(bb[3], bb[0]));
			coordinates.Add(new Coordinate(bb[3], bb[1]));
			coordinates.Add(new Coordinate(bb[2], bb[1]));
			coordinates.Add(new Coordinate(bb[2], bb[0]));
			coordinates.Add(new Coordinate(bb[3], bb[0]));

			var linearRing = new LinearRing(coordinates.ToArray());
			var polygon = new Polygon(linearRing);
			return polygon;
		}

		public List<string> Neighbours(string geohash)
		{
			return hasher.GetNeighbors(geohash).Select(d => d.Value).ToList();
		}

		public List<string> Subhashes(string geohash)
		{
			return hasher.GetSubhashes(geohash).ToList();
		}

		public string Reduce(string hash)
		{
			if (string.IsNullOrWhiteSpace(hash))
			{
				return hash;
			}
			return hash.Remove(hash.Length - 1);
		}

		public string Reduce(string hash, int precision)
		{
			if (string.IsNullOrWhiteSpace(hash))
			{
				return hash;
			}
			return hash.Remove(precision);
		}

		public List<string> Parents(string hash, int precision = 1)
		{
			var result = new List<string>();
			while (hash.Length > precision)
			{
				hash = hasher.GetParent(hash);
				result.Add(hash);
			}

			return result;
		}
	}
}