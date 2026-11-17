; ==========================================================================
;  Script de Inno Setup 6 - Sistema de Procesamiento y Auditoria de Facturacion
;  1) Ejecute publicar.bat (genera ..\publicacion\LogisticaFacturacion)
;  2) Abra este archivo con Inno Setup Compiler y presione Compile (Ctrl+F9)
;  Resultado: instalador\Salida\SetupLogisticaFacturacion.exe
; ==========================================================================

#define MiApp "Sistema de Facturacion Logistica"
#define MiVersion "1.0.0"
#define MiEjecutable "LogisticaFacturacion.exe"

[Setup]
AppId={{6B1D3E52-7C1A-4F43-9A8B-2E5F0C8D4A11}
AppName={#MiApp}
AppVersion={#MiVersion}
AppPublisher=Equipo Mini Proyecto 2
DefaultDirName={autopf}\LogisticaFacturacion
DefaultGroupName={#MiApp}
PrivilegesRequired=lowest
OutputDir=Salida
OutputBaseFilename=SetupLogisticaFacturacion
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=modern

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "escritorio"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "..\publicacion\LogisticaFacturacion\{#MiEjecutable}"; DestDir: "{app}"; Flags: ignoreversion
; onlyifdoesntexist conserva la configuracion del usuario al reinstalar
Source: "..\publicacion\LogisticaFacturacion\appsettings.json"; DestDir: "{app}"; Flags: onlyifdoesntexist
Source: "..\publicacion\LogisticaFacturacion\lote_ejemplo.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\publicacion\LogisticaFacturacion\LEEME_INSTALACION.txt"; DestDir: "{app}"; Flags: ignoreversion isreadme
Source: "..\publicacion\LogisticaFacturacion\esquema.sql"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MiApp}"; Filename: "{app}\{#MiEjecutable}"
Name: "{group}\Desinstalar {#MiApp}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MiApp}"; Filename: "{app}\{#MiEjecutable}"; Tasks: escritorio

[Run]
Filename: "{app}\{#MiEjecutable}"; Description: "{cm:LaunchProgram,{#MiApp}}"; Flags: nowait postinstall skipifsilent
