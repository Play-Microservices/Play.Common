# Common library
Library with common reusable code across microservices in Play project

## Building app
dotnet build

## Pack library and export to output folder
dotnet pack -o ../../../packages/
dotnet pack -o ../../../packages/ -p PackageVersion=1.0.5

## Specify dotnet local Nuget Package source path
dotnet nuget add source "<Asbosule_path_to_package_folder>" -n PlayEconom