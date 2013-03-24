@echo off
pushd %~dp0

set BUILD_CONFIG=Release
call build.bat

echo Creating NuGet package...
.nuget\NuGet.exe pack ConsoleAppLauncher\ConsoleAppLauncher.csproj -OutputDirectory PublishedPackages -Prop Configuration=%BUILD_CONFIG% -Symbols -Verbosity detailed

echo Done.
popd

