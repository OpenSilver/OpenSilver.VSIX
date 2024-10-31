@echo off

:: Step 0: Remove the nupkg folder if it exists
echo Removing nupkg folder...
if exist "nupkg" (
    rmdir /s /q "nupkg"
    if %errorlevel% neq 0 (
        echo Failed to remove nupkg folder, exiting...
        exit /b %errorlevel%
    )
)

:: Step 1: Uninstall OpenSilver.Templates (continue if it fails)
echo Uninstalling OpenSilver.Templates...
dotnet new uninstall OpenSilver.Templates
if %errorlevel% neq 0 (
    echo Failed to uninstall OpenSilver.Templates
)

:: Step 2: Build the project (exit if it fails)
echo Building the project...
dotnet build
if %errorlevel% neq 0 (
    echo Build failed, exiting...
    exit /b %errorlevel%
)

:: Step 3: Pack the project (exit if it fails)
echo Packing the project...
dotnet pack
if %errorlevel% neq 0 (
    echo Pack failed, exiting...
    exit /b %errorlevel%
)

:: Step 4: Find the latest OpenSilver.Templates nupkg file
echo Finding the latest OpenSilver.Templates nupkg file...
for %%f in (.\nupkg\OpenSilver.Templates.*.nupkg) do set "nupkgFile=%%f"

if not defined nupkgFile (
    echo No OpenSilver.Templates nupkg file found, exiting...
    exit /b 1
)

:: Step 5: Install the found OpenSilver.Templates file
echo Installing the new OpenSilver.Templates from %nupkgFile%...
dotnet new install %nupkgFile%
if %errorlevel% neq 0 (
    echo Installation failed, exiting...
    exit /b %errorlevel%
)

echo Script completed successfully!
