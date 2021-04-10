dotnet restore
REM dotnet msbuild /t:build /p:Configuration=Release /p:GeneratePackageOnBuild=True
dotnet pack -c Release -o Packages