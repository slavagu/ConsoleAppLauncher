@echo off
pushd %~dp0
rem if "%WindowsSdkDir%" == "" call "%VS100COMNTOOLS%\vsvars32.bat"

if "%BUILD_CONFIG%" == "" set BUILD_CONFIG=Debug


rem msbuild.exe ConsoleAppLauncher.sln /t:Rebuild /p:Configuration=%BUILD_CONFIG%

echo Running tests...
dotnet test -c %BUILD_CONFIG% ConsoleAppLauncher.Tests/ConsoleAppLauncher.Tests.csproj

echo Building %BUILD_CONFIG%...
dotnet build -c %BUILD_CONFIG% ConsoleAppLauncher/ConsoleAppLauncher.csproj


rem packages\NUnit.Runners.2.6.2\tools\nunit-console.exe "ConsoleAppLauncher.Tests\bin\%BUILD_CONFIG%\ConsoleAppLauncher.Tests.dll" /framework=net-4.0 /labels

echo Done.
popd

