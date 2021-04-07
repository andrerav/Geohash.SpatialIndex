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

		public static List<TModel> ReadCsvFile<TModel, TClassMap>(string file) where TClassMap : ClassMap<TModel>
		{
			var reader = File.OpenText(file);
			var csv = new CsvReader(reader, DefaultConfig);
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