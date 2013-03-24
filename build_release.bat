@echo off
pushd %~dp0

set BUILD_CONFIG=Release
call build.bat

popd

