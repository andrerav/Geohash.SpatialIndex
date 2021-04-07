# Geohash.SpatialIndex
A spatial index backed by geohashing and Trie (prefix tree) maps. Geometries are encoded as geohashes and stored in a prefix tree, and reverse lookups are performing by encoding the queried geometry and using the prefix tree as an inverted lookup index. The index size can be tuned with the precision parameter. The geohasher and prefix tree map providers can be injected by implementing the `IGeohasher` and/or `IGeohashTrieMap<TEntryList, T>` interfaces. This library ships with default implementations that uses [rm.Trie](https://github.com/rmandvikar/csharp-trie) and [geohash-dotnet](https://github.com/postlagerkarte/geohash-dotnet).

Some examples. Here is an index covering a set of polygons along the norwegian coastline:
![image](https://user-images.githubusercontent.com/3635018/113932204-a1109e00-97f3-11eb-8549-7464224445d3.png)

Another example, an index covering some polygons in inner Oslo Fjord:
![image](https://user-images.githubusercontent.com/3635018/113932377-d0270f80-97f3-11eb-9514-ab1a5bf1df8a.png)

## Quickstart
Build the index:
```csharp
var index = new GeohashSpatialIndex<int>(
                  new DefaultGeohasher(), 
                  new DefaultTrieMap<int>(), 
                  precision: 9);
```  

Fill the index with geometries:
```csharp
foreach(var obj in gisObjects)
    index.Insert(obj.Geometry, obj);
```

Query the index:
```csharp
var result = index.Query(geom)
```

You can also use one of the provided spatial relation functions provided which is backed by NetTopologySuite:
```csharp
var relations = new GeohashSpatialIndexRelations<MyGisObjType>(index);
var result = relations.STIntersects(myGeom);
```
Here is a full list of currently supported spatial relations:
* STContains()
* STContainsProperly()
* STCrosses()
* STEquals()
* STIntersects()
* STOverlaps()
* STTouches()
* STWithin()

Addtionally, nearest neighbour and KNN searches are supported with custom distance metrics using the STNearestNeighbour() and KNN() methods.
