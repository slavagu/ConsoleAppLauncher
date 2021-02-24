@echo off
pushd %~dp0

set BUILD_CONFIG=Release
rem call build.bat

echo Creating NuGet package...
rem .nuget\NuGet.exe pack ConsoleAppLauncher\ConsoleAppLauncher.csproj -OutputDirectory PublishedPackages -Prop Configuration=%BUILD_CONFIG% -Symbols -Verbosity detailed
dotnet pack -c %BUILD_CONFIG% ConsoleAppLauncher/ConsoleAppLauncher.csproj -o PublishedPackages

echo Done.
popd

