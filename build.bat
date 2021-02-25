@echo off
pushd %~dp0
rem if "%WindowsSdkDir%" == "" call "%VS100COMNTOOLS%\vsvars32.bat"

if "%BUILD_CONFIG%" == "" set BUILD_CONFIG=Debug


rem msbuild.exe ConsoleAppLauncher.sln /t:Rebuild /p:Configuration=%BUILD_CONFIG%

echo Building %BUILD_CONFIG%...
dotnet build -c %BUILD_CONFIG% ConsoleAppLauncher/ConsoleAppLauncher.csproj

echo Running tests...
packages\NUnit.ConsoleRunner.3.12.0\tools\nunit3-console.exe "ConsoleAppLauncher.Tests\bin\%BUILD_CONFIG%\ConsoleAppLauncher.Tests.dll" /framework=net-4.0

echo Done.
popd

