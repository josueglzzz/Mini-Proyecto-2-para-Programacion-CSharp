# Sistema de Procesamiento y Auditoría de Facturación (Logística y Entregas)

Aplicación de escritorio en **C# / .NET 8 / Windows Forms** para procesar lotes de facturas de flete,
validar su estructura, calcular el desglose financiero y auditar el historial de envíos.

## Estado del proyecto

| Parte | Responsable | Contenido | Estado |
|------|-------------|-----------|--------|
| 1 | Josué González Gutiérrez | Estructura de la solución, dominio y lógica de negocio (Retos 1, 2 y 3) | ✅ |
| 2 | Román González Mata | Persistencia: JSON, CSV y base de datos SQLite | ✅ |
| 3 | Isaac Alejandro Rubalcaba Dávila | Interfaz WinForms multi-idioma, eventos e instalador | ⏳ |

## Estructura (Partes 1 y 2)

```
LogisticaFacturacion.sln
src/
  Logistica.Dominio/    Entidades: Paquete, DesgloseCosto, ConfiguracionTarifas, ResultadoLote...
  Logistica.Logica/     ParserLotes (Reto 1), ArregloPaquetes (Reto 2), CalculadoraTarifas (Reto 3), ProcesadorLotes
  Logistica.Persistencia/  ConfiguracionRepositorio (JSON), ExportadorCsv (CSV), RepositorioEnvios (SQLite)
database/
  esquema.sql           Esquema de referencia de la base de datos
tests/
  Logistica.Pruebas/    Pruebas automáticas (aplicación de consola)
```

## Ejecutar las pruebas

```
dotnet run --project tests/Logistica.Pruebas
```
