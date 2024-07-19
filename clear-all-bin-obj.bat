@echo off
setlocal enabledelayedexpansion

set BIN_FOUND=0
set OBJ_FOUND=0

:: Define the root path if the script is not run from the root
set "ROOT_DIR=%~dp0"

:: Absolute paths for exclusions
set "EXCLUDE1=%ROOT_DIR%src\OpenSilverBusinessApplicationTemplate\OpenSilverBusinessApplication.Web\bin"
set "EXCLUDE2=%ROOT_DIR%src\OpenSilverBusinessVBApplicationTemplate\OpenSilverBusinessApplication.Web\bin"

echo Searching and deleting bin directories...
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin 2^>nul') DO (
    set "CURRENT_DIR=%%~fG"
    if "!CURRENT_DIR!"=="!EXCLUDE1!" (
        echo Skipping: %%G
    ) else if "!CURRENT_DIR!"=="!EXCLUDE2!" (
        echo Skipping: %%G
    ) else (
        set BIN_FOUND=1
        echo Deleting: %%G
        RMDIR /S /Q "%%G"
    )
)

echo Searching and deleting obj directories...
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj 2^>nul') DO (
    set OBJ_FOUND=1
    echo Deleting: %%G
    RMDIR /S /Q "%%G"
)

if !BIN_FOUND! EQU 0 (
    echo No bin directories found.
)

if !OBJ_FOUND! EQU 0 (
    echo No obj directories found.
)

endlocal
