@echo off
pushd %~dp0

echo Pushing NuGet package to nuget.org...
dotnet nuget push "ConsoleAppLauncher\bin\Release\ConsoleAppLauncher.2.0.0.nupkg" --source https://api.nuget.org/v3/index.json --api-key %1

:exit
echo Done.
popd

