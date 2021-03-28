@echo off
@setlocal

dotnet restore Src\Zeckoxe.sln

set UseStableVersions=true

dotnet pack -c Release Src\Zeckoxe.Core\Zeckoxe.Core.csproj
dotnet pack -c Release Src\Zeckoxe.Desktop\Zeckoxe.Desktop.csproj
dotnet pack -c Release Src\Zeckoxe.Engine\Zeckoxe.Engine.csproj
dotnet pack -c Release Src\Zeckoxe.Vulkan\Zeckoxe.Vulkan.csproj

