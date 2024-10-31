@echo off
setlocal

:: Define the path to vswhere (adjust if vswhere is in a different location)
set VSPATH="%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"

:: Check if vswhere exists
if not exist %VSPATH% (
    echo vswhere.exe not found. Please ensure it is installed in the default location.
    exit /b 1
)

:: Use vswhere to find the latest MSBuild instance
for /f "usebackq tokens=*" %%i in (`%VSPATH% -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set MSBUILD_PATH=%%i

:: Check if msbuild was found
if not defined MSBUILD_PATH (
    echo MSBuild not found. Please ensure Visual Studio with MSBuild is installed.
    exit /b 1
)

:: Restore NuGet packages
echo Restoring NuGet packages with MSBuild at %MSBUILD_PATH%
"%MSBUILD_PATH%" -t:restore -p:RestorePackagesConfig=true

endlocal
