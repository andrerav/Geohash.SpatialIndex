using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Geohash.SpatialIndex.Core
{
	/// <summary>
	/// To inject a custom geohasher into the geohasher spatial index class, implement this interface in a new class.
	/// </summary>
	public interface IGeohasher
	{
		/// <summary>
		/// Return the bounding box for this geohash as a valid geometry
		/// </summary>
		/// <param name="geohash"></param>
		/// <returns></returns>
		Polygon BoundingBox(string geohash);

		/// <summary>
		/// Decode this geohash
		/// </summary>
		/// <param name="geohash"></param>
		/// <returns></returns>
		Point Decode(string geohash);

		/// <summary>
		/// Encode this geometry as a geohash. If the geometry is not a point, return the smallest possible geohash for the coordinates in this geometry.
		/// </summary>
		/// <param name="geom"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		string Encode(Geometry geom, int precision);

		/// <summary>
		/// A list of neighbours for the given geohash. Should always return 8 neighbours.
		/// </summary>
		/// <param name="geohash"></param>
		/// <returns></returns>
		List<string> Neighbours(string geohash);

		/// <summary>
		/// Return a list of parents for this geohash
		/// </summary>
		/// <param name="hash"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		List<string> Parents(string hash, int precision);

		/// <summary>
		/// Remove one character from the end of the geohash
		/// </summary>
		/// <param name="hash"></param>
		/// <returns></returns>
		string Reduce(string hash);

		/// <summary>
		/// Remove characters from the end of the geohash until @precision is met
		/// </summary>
		/// <param name="hash"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		string Reduce(string hash, int precision);

		/// <summary>
		/// Return a list of all subhashes within this geohash
		/// </summary>
		/// <param name="geohash"></param>
		/// <returns></returns>
		List<string> Subhashes(string geohash);
	}
}