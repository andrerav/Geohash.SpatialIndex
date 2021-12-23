# Changelog

## Version 0.2.0
* Added Remove(T value) method to remove entries by value
* Added InsertOrUpdate(geohash, value) method. Note that the current implementation does a simple Remove(value) and then Insert(geohash, value), which is slow. Avoid using this when building the index from bulk data.
* Updated NuGet package dependencies

## Version 0.1.1-pre
* Added license expression to NuGet packages
* Updated Authors field and added link to release notes
* Debug symbols are now included when building packages

## Version 0.1.0-pre
* Added logo image files.
* Added Directory.build.props file which contains common project properties such as version number.
* Added build targets for .NET 5, .NET 6, and .NET Standard 2.0.
* Added build script to pack NuGet packages.
* Fixed a minor name conflict in the DefaultTrieMap class.
