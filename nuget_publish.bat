@echo off
pushd %~dp0

set NUGET_PACKAGE=%1
if not exist %NUGET_PACKAGE% echo Could not find NuGet package %NUGET_PACKAGE% & goto exit

echo Pushing NuGet package to nuget.org...
.nuget\NuGet.exe push %NUGET_PACKAGE% -Verbosity detailed

:exit
echo Done.
popd

