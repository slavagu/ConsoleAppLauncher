@echo off
pushd %~dp0

echo Pushing NuGet package to nuget.org...
dotnet nuget push %1 --api-key %2 --source https://api.nuget.org/v3/index.json

:exit
echo Done.
popd

