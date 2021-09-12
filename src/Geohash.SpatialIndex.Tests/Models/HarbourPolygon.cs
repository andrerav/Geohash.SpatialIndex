using CsvHelper;
using CsvHelper.Configuration;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Geohash.SpatialIndex.Tests.Models
{
	public class HarbourPolygon
	{
		public virtual int Id { get; set; }
		public virtual Geometry Geom { get; set; }
		public virtual int HarbourId { get; set; }
		public virtual int Popularity { get; set; }
		public virtual bool AnchorPolygon { get; set; }
	}

	public class VesselPosition
	{
		public virtual int Mmsi { get; set; }
		public virtual string DateTimeUtc { get; set; }
		public virtual Geometry Geom { get; set; }
		public virtual double Sog { get; set; }
		public virtual double Cog { get; set; }
		public virtual double TrueHeading { get; set; }
		public virtual int NavStatus { get; set; }
		public virtual int MessageNr { get; set; }
	}

	public class VesselPositionCsvClassMap : ClassMap<VesselPosition>
	{
		public VesselPositionCsvClassMap()
		{
			Map(m => m.Mmsi).Name("mmsi");
			Map(m => m.DateTimeUtc).Name("timestamp");
			Map(m => m.Geom).Convert(args =>
			{
				var lon = Double.Parse(args.Row.GetField("lon"), CultureInfo.InvariantCulture);
				var lat = Double.Parse(args.Row.GetField("lat"), CultureInfo.InvariantCulture);
				var p = new Point(lon, lat);
				p.SRID = 4326;
				return p;
			});
			Map(m => m.Sog).Name("sog");
			Map(m => m.Cog).Name("cog");
		}
	}

	public class HarbourCsvClassMap : ClassMap<HarbourPolygon>
	{
		public HarbourCsvClassMap()
		{
			Map(m => m.Id).Name("id");
			Map(m => m.Geom).Convert(args =>
			{
				var wktReader = new WKTReader();
				var p = wktReader.Read(args.Row.GetField("geom"));
				p.SRID = 4326;
				return p;
			});
			Map(m => m.HarbourId).Name("harbour_id");
			Map(m => m.Popularity).Name("popularity");
			Map(m => m.AnchorPolygon).Name("anchor_polygon");
		}
	}

	public static class TestUtil
	{
		private static CsvConfiguration DefaultConfig => new CsvConfiguration(CultureInfo.InvariantCulture)
		{
			HeaderValidated = null,
			MissingFieldFound = null,
			Delimiter = ",",
			BufferSize = 1048576,
		};

		public static List<TModel> ReadCsvFile<TModel, TClassMap>(string file, string delimiter = ",") where TClassMap : ClassMap<TModel>
		{
			var config = DefaultConfig;
			config.Delimiter = delimiter;
			var reader = File.OpenText(file);
			var csv = new CsvReader(reader, config);
			csv.Context.RegisterClassMap<TClassMap>();
			var records = csv.GetRecords<TModel>().ToList();
			reader.Close();
			reader.Dispose();
			csv.Dispose();

			return records;
		}

		public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
		{
			return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
		}
	}
}