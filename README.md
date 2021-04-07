# Geohash.SpatialIndex
A spatial index backed by geohashing and trie maps.

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

You can also use one of the provided spatial relations which is backed by NetTopolgySuite:
```csharp
var relations = new GeohashSpatialIndexRelations<MyGisObjType>(index);
var intersects_result = relations.STIntersects(myGeom);
var contains_result = relations.STContains(myGeom);
var overlaps_result = relations.STOverlaps(myGeom);
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
