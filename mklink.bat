@echo off
SETLOCAL

set "SCRIPT_DIR=%~dp0"

set "CLIENT_PATH=SunshineMinerClient"
set "SERVER_PATH=SunshineMinerServer"
set "SHARED_PATH=SunshineMinerShared"
set "SHARED_CODE_RELATIVE=Shared"

:: 转换为绝对路径
call :MakeAbsolute "%SCRIPT_DIR%\%CLIENT_PATH%" CLIENT_ABS_PATH
call :MakeAbsolute "%SCRIPT_DIR%\%SERVER_PATH%" SERVER_ABS_PATH
call :MakeAbsolute "%SCRIPT_DIR%\%SHARED_PATH%\%SHARED_CODE_RELATIVE%" SHARED_CODE_ABS_PATH

echo CLIENT_ABS_PATH=%CLIENT_ABS_PATH%
echo SERVER_ABS_PATH=%SERVER_ABS_PATH%
echo SHARED_CODE_ABS_PATH=%SHARED_CODE_ABS_PATH%

if not exist "%CLIENT_ABS_PATH%\Assets\Scripts\Shared" (
    echo Create link for ClientShared...
    mklink /J "%CLIENT_ABS_PATH%\Assets\Scripts\Shared" "%SHARED_CODE_ABS_PATH%"
) else (
    echo ClientSharedExisted...
)

if not exist "%SERVER_ABS_PATH%\Shared" (
    echo Create link for ServerShared...
    mklink /J "%SERVER_ABS_PATH%\Shared" "%SHARED_CODE_ABS_PATH%"
) else (
    echo ServerSharedExisted...
)

echo.
echo Over...

pause
EXIT /B

:MakeAbsolute
SET "%~2=%~f1"
EXIT /B