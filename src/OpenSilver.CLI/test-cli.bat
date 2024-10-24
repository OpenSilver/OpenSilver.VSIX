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

:: Step 4: Install the new OpenSilver.Templates (exit if it fails)
echo Installing the new OpenSilver.Templates...
dotnet new install .\nupkg\OpenSilver.Templates.3.0.0.nupkg
if %errorlevel% neq 0 (
    echo Installation failed, exiting...
    exit /b %errorlevel%
)

echo Script completed successfully!
