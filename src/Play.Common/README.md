# Common library
Library with common reusable code across microservices in Play project

## Building app
dotnet build

## Pack library and export to output folder
dotnet pack -o ../../../packages/
dotnet pack -o ../../../packages/ -p:PackageVersion=1.0.14

## Specify dotnet local Nuget Package source path
dotnet nuget add source "<Absolute_path_to_package_folder>" -n PlayEconomy

## Publish package to Github
```powershell
$version="1.0.14"
$owner="Play-Microservices"
$gh_pat="[PAT HERE]"

dotnet pack src/Play.Common/ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=http://github.com/$owner/play.common -o ../packages

dotnet nuget push ../packages/Play.Common.$version.nupkg --api-key $gh_pat --source "github"
```

```bash
version="1.0.14"
owner="Play-Microservices"
gh_pat="[PAT HERE]"

dotnet pack src/Play.Common/ \
  --configuration Release \
  -p:PackageVersion="$version" \
  -p:RepositoryUrl="http://github.com/$owner/play.common" \
  -o ../packages

dotnet nuget push "../packages/Play.Common.$version.nupkg" \
  --api-key "$gh_pat" \
  --source "github"
```