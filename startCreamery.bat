@echo off
setlocal EnableDelayedExpansion

:: Configuration variables
set "API_PROJECT_PATH=.\api\api.csproj"
set "ANGULAR_PROJECT_PATH=.\client"
set "BROWSER_URL=https://localhost:4200"
set "API_PORT=5001"

:: Validate paths
if not exist "%API_PROJECT_PATH%" (
    echo Error: API project not found at %API_PROJECT_PATH%
    pause
    exit /b 1
)

if not exist "%ANGULAR_PROJECT_PATH%\package.json" (
    echo Error: Angular project not found at %ANGULAR_PROJECT_PATH%
    pause
    exit /b 1
)

:: Start ASP.NET Core Web API
echo Starting ASP.NET Core Web API...
cd /d "%~dp0"
start "Web API" cmd /c "dotnet run --project %API_PROJECT_PATH% --urls=https://localhost:%API_PORT%"

:: Wait briefly to ensure API starts
timeout /t 5

:: Start Angular frontend
echo Starting Angular frontend...
cd /d "%ANGULAR_PROJECT_PATH%"
start "Angular" cmd /c "npm start"

:: Wait for Angular to compile
echo Waiting for Angular to compile...
timeout /t 10

:: Open browser
echo Opening browser at %BROWSER_URL%...
start "" "%BROWSER_URL%"

echo Applications started. Press any key to exit...
pause >nul