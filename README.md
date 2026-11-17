# Sistema de Procesamiento y Auditoría de Facturación (Logística y Entregas)

Aplicación de escritorio en **C# / .NET 8 / Windows Forms** para procesar los lotes de facturas de flete
de las sucursales de una empresa de envíos express: valida cada línea capturada, calcula el desglose
financiero (subtotal, recargo por zona, sobrepeso, seguro, IVA y total), guarda el historial en una base
de datos SQLite, exporta la auditoría a CSV y permite cambiar el idioma de la interfaz (Español / English).

Repositorio: https://github.com/josueglzzz/Mini-Proyectyo-2-para-Programacion-CSharp

## Equipo

| Integrante | ID | Rol | Partes del proyecto |
|-----------|----|-----|---------------------|
| Josué González Gutiérrez | 261893 | Líder técnico / lógica de negocio | Solución, dominio, Retos 1, 2 y 3, pruebas de lógica |
| Román González Mata | 552992 | Desarrollador de persistencia | appsettings.json (lectura), CSV, SQLite, pruebas de persistencia |
| Isaac Alejandro Rubalcaba Dávila | 553586 | Desarrollador de interfaz y despliegue | WinForms, multi-idioma, eventos, instalador |

## Arquitectura en capas

```
LogisticaFacturacion.sln
├── src/
│   ├── Logistica.Dominio/        Capa de dominio: entidades y excepciones
│   │     Paquete, DesgloseCosto, ConfiguracionTarifas, ResultadoLote, ErrorLinea, ResumenLote...
│   ├── Logistica.Logica/         Servicios de lógica (sin dependencias de UI ni archivos)
│   │     ParserLotes        → Reto 1: sanitización y parseo tolerante a fallos
│   │     ArregloPaquetes    → Reto 2: arreglo nativo con redimensionamiento, burbuja e inversión
│   │     CalculadoraTarifas → Reto 3: tarifas escalonadas por zona, sobrepeso y seguro
│   │     ProcesadorLotes    → orquesta el procesamiento completo
│   ├── Logistica.Persistencia/   Servicios de persistencia
│   │     ConfiguracionRepositorio → lectura de appsettings.json (System.Text.Json)
│   │     ExportadorCsv            → exportación CSV con bloques using
│   │     RepositorioEnvios        → base de datos SQLite (envíos y recepciones)
│   └── Logistica.UI/             Capa de presentación (Windows Forms)
│         FormPrincipal, Idiomas/Localizador, appsettings.json
├── tests/Logistica.Pruebas/      Pruebas automáticas (consola, sin dependencias externas)
├── database/esquema.sql          Esquema de referencia de la base de datos
├── ejemplos/lote_ejemplo.txt     Lote de ejemplo con casos válidos e inválidos
├── instalador/                   Script de Inno Setup e instrucciones de instalación
└── publicar.bat                  Genera el paquete de instalación (Entregable 2)
```

Dependencias entre proyectos: `UI → Lógica, Persistencia → Dominio`. La interfaz no hace cálculos ni lee archivos directamente.

## Requisitos

- Windows 10/11 de 64 bits.
- Para compilar: **Visual Studio 2022** (carga de trabajo "Desarrollo de escritorio de .NET") o el **SDK de .NET 8**.
- Paquete NuGet: `Microsoft.Data.Sqlite 8.0.10` (se restaura automáticamente).

## Clonar, compilar y ejecutar

```bash
git clone https://github.com/josueglzzz/Mini-Proyectyo-2-para-Programacion-CSharp.git
cd Mini-Proyectyo-2-para-Programacion-CSharp
dotnet build
dotnet run --project src/Logistica.UI
```

En Visual Studio: abrir `LogisticaFacturacion.sln`, establecer **Logistica.UI** como proyecto de inicio y presionar F5.

## Pruebas automáticas

```bash
dotnet run --project tests/Logistica.Pruebas
```

Ejecuta 102 verificaciones de los tres retos, la lectura del JSON, la exportación CSV y la base de datos.

## Generar el instalador (Entregable 2)

Doble clic en `publicar.bat`. El script ejecuta las pruebas, publica la aplicación autocontenida
para `win-x64` (no requiere instalar .NET en la computadora destino) y genera:

- `publicacion/LogisticaFacturacion/` → carpeta lista para usar (`LogisticaFacturacion.exe`).
- `publicacion/Entregable2_Instalador_LogisticaFacturacion.zip` → paquete para entregar.
- Opcional: si tiene **Inno Setup 6** instalado, compile `instalador/LogisticaFacturacion.iss` para obtener `SetupLogisticaFacturacion.exe`.

## Formato del lote

Una factura por línea, campos separados por `;`:

```
CODIGO;CLIENTE;ZONA;PESO_KG;VALOR_DECLARADO
PACK-001;Juan Perez;Zona A;12.5;150.0
```

| Campo | Regla |
|-------|-------|
| Código | 2 a 6 letras, guion y 1 a 6 dígitos (`PACK-001`, `COD-101`). No puede repetirse en el lote. |
| Cliente | Letras, espacios, punto, apóstrofo o guion. Máximo 80 caracteres. |
| Zona | `Zona A`, `Zona B` o `Zona C` (también acepta `A`, `b`, `zona c`). |
| Peso | Número con punto decimal, mayor a cero. |
| Valor declarado | Número mayor o igual a cero. Acepta `$` y separador de miles (`$1,500.00`). |

## Reglas de cálculo (Reto 3)

Con los valores de `appsettings.json`:

1. Subtotal por peso = peso × `PrecioBasePorKg`.
2. Recargo por zona = subtotal × (`RecargoZonaA` 5% · `RecargoZonaB` 10% · `RecargoZonaC` 18%).
3. Si el peso supera `PesoLimiteKg` (20 kg): penalización = kg extra × precio base × `PenalizacionSobrepeso` (25%).
4. Si el valor declarado supera `UmbralSeguro` ($1,000.00): seguro = valor × `TasaSeguro` (2%).
5. Base imponible = subtotal + recargo + penalización + seguro + `CostoFijoEnvio`.
6. IVA = base × `TasaIva` (16%). Total = base + IVA.

## Configuración (`appsettings.json`)

Se encuentra junto al ejecutable. Si falta o tiene un error, la aplicación lo informa y deshabilita
el procesamiento hasta que se corrija y se presione **Recargar configuración**.

La base de datos se crea automáticamente en `%LOCALAPPDATA%\LogisticaFacturacion\logistica_envios.db`.

## Convenciones de código

PascalCase para clases, métodos y propiedades; camelCase para variables locales y parámetros;
`_camelCase` para campos privados (incluidos los controles del formulario).
