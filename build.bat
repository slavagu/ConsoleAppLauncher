@echo off
pushd %~dp0

set BUILD_CONFIG=Release

echo Building %BUILD_CONFIG%...
dotnet build -c %BUILD_CONFIG% ConsoleAppLauncher

echo Running tests...
dotnet test -c %BUILD_CONFIG% -f net45 ConsoleAppLauncher.Tests

echo Building Samples...
dotnet build -c %BUILD_CONFIG% ConsoleAppLauncher.Samples 

echo Done.
popd

