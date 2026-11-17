@echo off
REM ==========================================================================
REM  Genera el paquete de instalacion (Entregable 2) del Sistema de Facturacion
REM  Requisito: SDK de .NET 8 instalado (viene con Visual Studio 2022).
REM ==========================================================================
setlocal
cd /d "%~dp0"

echo.
echo === 1/4 Verificando el SDK de .NET ===
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo No se encontro el SDK de .NET 8.
    echo Descarguelo de https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo.
echo === 2/4 Ejecutando pruebas automaticas ===
dotnet run --project tests\Logistica.Pruebas -c Release
if errorlevel 1 (
    echo Las pruebas fallaron. Corrija los errores antes de publicar.
    pause
    exit /b 1
)

set SALIDA=%~dp0publicacion\LogisticaFacturacion
if exist "%~dp0publicacion" rmdir /s /q "%~dp0publicacion"

echo.
echo === 3/4 Publicando aplicacion autocontenida win-x64 ===
dotnet publish src\Logistica.UI\Logistica.UI.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=none -o "%SALIDA%"
if errorlevel 1 (
    echo Error al publicar la aplicacion.
    pause
    exit /b 1
)

copy /y "ejemplos\lote_ejemplo.txt" "%SALIDA%\" >nul
copy /y "instalador\LEEME_INSTALACION.txt" "%SALIDA%\" >nul
copy /y "database\esquema.sql" "%SALIDA%\" >nul

echo.
echo === 4/4 Comprimiendo paquete ===
powershell -NoProfile -Command "Compress-Archive -Path '%SALIDA%' -DestinationPath '%~dp0publicacion\Entregable2_Instalador_LogisticaFacturacion.zip' -Force"

echo.
echo Listo. Archivos generados en la carpeta "publicacion":
echo   - LogisticaFacturacion\LogisticaFacturacion.exe
echo   - Entregable2_Instalador_LogisticaFacturacion.zip
echo.
echo Opcional: abra instalador\LogisticaFacturacion.iss con Inno Setup 6 y compile
echo para obtener SetupLogisticaFacturacion.exe
pause
