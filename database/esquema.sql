-- =====================================================================
-- Esquema de la base de datos SQLite del Sistema de Facturación Logística
-- La aplicación crea estas tablas automáticamente al iniciar
-- (RepositorioEnvios.InicializarBaseDatos). Este script es de referencia
-- y puede ejecutarse manualmente con:  sqlite3 logistica_envios.db < esquema.sql
-- Ubicación por defecto: %LOCALAPPDATA%\LogisticaFacturacion\logistica_envios.db
-- =====================================================================

PRAGMA foreign_keys = ON;

-- Un registro por cada lote procesado y guardado.
CREATE TABLE IF NOT EXISTS Lotes (
    Id                 INTEGER PRIMARY KEY AUTOINCREMENT,
    FechaProceso       TEXT    NOT NULL,          -- yyyy-MM-dd HH:mm:ss
    TotalLineas        INTEGER NOT NULL,
    PaquetesValidos    INTEGER NOT NULL,
    PaquetesConError   INTEGER NOT NULL,
    SumaBaseImponible  REAL    NOT NULL,
    SumaIva            REAL    NOT NULL,
    SumaTotal          REAL    NOT NULL
);

-- Transacciones de envío: cada paquete válido con su desglose.
-- La recepción se registra cambiando Estado a 'Entregado'.
CREATE TABLE IF NOT EXISTS Paquetes (
    Id                     INTEGER PRIMARY KEY AUTOINCREMENT,
    LoteId                 INTEGER NOT NULL REFERENCES Lotes(Id) ON DELETE CASCADE,
    NumeroLinea            INTEGER NOT NULL,
    Codigo                 TEXT    NOT NULL,
    Cliente                TEXT    NOT NULL,
    Zona                   TEXT    NOT NULL,      -- A, B o C
    PesoKg                 REAL    NOT NULL,
    ValorDeclarado         REAL    NOT NULL,
    SubtotalPeso           REAL    NOT NULL,
    PorcentajeRecargoZona  REAL    NOT NULL,
    RecargoZona            REAL    NOT NULL,
    KilosExcedentes        REAL    NOT NULL,
    PenalizacionSobrepeso  REAL    NOT NULL,
    Seguro                 REAL    NOT NULL,
    CostoFijoEnvio         REAL    NOT NULL,
    BaseImponible          REAL    NOT NULL,
    Iva                    REAL    NOT NULL,
    Total                  REAL    NOT NULL,
    Estado                 TEXT    NOT NULL DEFAULT 'EnTransito',  -- EnTransito | Entregado
    FechaRecepcion         TEXT    NULL
);

-- Líneas descartadas de cada lote (auditoría).
CREATE TABLE IF NOT EXISTS ErroresLote (
    Id                 INTEGER PRIMARY KEY AUTOINCREMENT,
    LoteId             INTEGER NOT NULL REFERENCES Lotes(Id) ON DELETE CASCADE,
    NumeroLinea        INTEGER NOT NULL,
    TipoError          TEXT    NOT NULL,
    Detalle            TEXT    NOT NULL,
    ContenidoOriginal  TEXT    NOT NULL
);

CREATE INDEX IF NOT EXISTS IX_Paquetes_LoteId ON Paquetes(LoteId);
CREATE INDEX IF NOT EXISTS IX_ErroresLote_LoteId ON ErroresLote(LoteId);

-- Consultas útiles de mantenimiento:
-- SELECT * FROM Lotes ORDER BY Id DESC;
-- SELECT Codigo, Cliente, Total, Estado FROM Paquetes WHERE LoteId = 1;
-- SELECT Estado, COUNT(*) FROM Paquetes GROUP BY Estado;
