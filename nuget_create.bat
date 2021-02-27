@echo off
pushd %~dp0

echo Creating NuGet package...
dotnet pack -c Release ConsoleAppLauncher -o PublishedPackages

echo Done.
popd

