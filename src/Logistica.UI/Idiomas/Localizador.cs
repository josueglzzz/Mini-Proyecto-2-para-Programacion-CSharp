using System;
using System.Collections.Generic;
using System.Globalization;
using Logistica.Dominio;

namespace Logistica.UI.Idiomas
{
    /// <summary>
    /// RF2: diccionario de textos de la interfaz en Español e Inglés.
    /// Al cambiar el idioma dispara el evento IdiomaCambiado, al que se
    /// suscribe el formulario para refrescar todos sus textos.
    /// </summary>
    public class Localizador
    {
        private readonly Dictionary<string, string[]> _textos;
        private Idioma _idiomaActual;

        public event EventHandler IdiomaCambiado;

        public Localizador()
        {
            _textos = new Dictionary<string, string[]>();
            _idiomaActual = Idioma.Espanol;
            RegistrarTextos();
        }

        public Idioma IdiomaActual
        {
            get { return _idiomaActual; }
            set
            {
                if (_idiomaActual == value)
                {
                    return;
                }
                _idiomaActual = value;
                EventHandler manejador = IdiomaCambiado;
                if (manejador != null)
                {
                    manejador(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>Cultura usada para formatear montos y fechas.</summary>
        public CultureInfo Cultura
        {
            get { return _idiomaActual == Idioma.Ingles ? new CultureInfo("en-US") : new CultureInfo("es-MX"); }
        }

        /// <summary>Convierte el código "es"/"en" del JSON en un idioma.</summary>
        public static Idioma DesdeCodigo(string codigo)
        {
            string normalizado = (codigo ?? string.Empty).Trim().ToLowerInvariant();
            switch (normalizado)
            {
                case "en":
                case "en-us":
                case "ingles":
                case "english":
                    return Idioma.Ingles;
                default:
                    return Idioma.Espanol;
            }
        }

        public string Texto(string clave)
        {
            string[] traducciones;
            if (_textos.TryGetValue(clave, out traducciones))
            {
                return traducciones[(int)_idiomaActual];
            }
            return "[" + clave + "]";
        }

        public string Formatear(string clave, params object[] argumentos)
        {
            return string.Format(Cultura, Texto(clave), argumentos);
        }

        public string Moneda(decimal monto)
        {
            return monto.ToString("C2", Cultura);
        }

        public string Porcentaje(decimal fraccion)
        {
            return (fraccion * 100m).ToString("0.##", Cultura) + "%";
        }

        public string Numero(decimal valor)
        {
            return valor.ToString("#,##0.###", Cultura);
        }

        public string Fecha(DateTime fecha)
        {
            return fecha.ToString("g", Cultura);
        }

        public string TextoError(TipoErrorLinea tipo)
        {
            return Texto("Error." + tipo);
        }

        public string TextoEstado(EstadoEnvio estado)
        {
            return Texto("Estado." + estado);
        }

        private void Agregar(string clave, string espanol, string ingles)
        {
            _textos[clave] = new string[] { espanol, ingles };
        }

        private void RegistrarTextos()
        {
            // Ventana y barra superior
            Agregar("Form.Titulo", "Sistema de Procesamiento y Auditoría de Facturación", "Freight Billing Processing and Audit System");
            Agregar("Etiqueta.Idioma", "Idioma:", "Language:");
            Agregar("Boton.RecargarConfig", "Recargar configuración", "Reload settings");

            // Captura
            Agregar("Grupo.Captura", "Captura del lote de envíos", "Shipment batch input");
            Agregar("Etiqueta.Instrucciones",
                "Una factura por línea, campos separados por punto y coma (;):\r\nCÓDIGO;CLIENTE;ZONA;PESO_KG;VALOR_DECLARADO\r\nEjemplo: PACK-001;Juan Perez;Zona A;12.5;150.0",
                "One invoice per line, fields separated by semicolons (;):\r\nCODE;CUSTOMER;ZONE;WEIGHT_KG;DECLARED_VALUE\r\nExample: PACK-001;Juan Perez;Zona A;12.5;150.0");
            Agregar("Boton.Procesar", "Procesar lote", "Process batch");
            Agregar("Boton.Ejemplo", "Cargar ejemplo", "Load sample");
            Agregar("Boton.Limpiar", "Limpiar", "Clear");
            Agregar("Etiqueta.ContadorLineas", "Líneas capturadas: {0}", "Lines entered: {0}");

            // Pestañas y acciones
            Agregar("Tab.Validos", "Paquetes válidos ({0})", "Valid packages ({0})");
            Agregar("Tab.Errores", "Líneas con error ({0})", "Lines with errors ({0})");
            Agregar("Tab.Historial", "Historial (base de datos)", "History (database)");
            Agregar("Boton.Exportar", "Exportar CSV...", "Export CSV...");
            Agregar("Boton.Invertir", "Invertir orden", "Reverse order");
            Agregar("Boton.ActualizarHistorial", "Actualizar historial", "Refresh history");
            Agregar("Boton.RegistrarRecepcion", "Registrar recepción", "Register delivery");
            Agregar("Etiqueta.OrdenDescendente", "Orden: mayor a menor total", "Order: highest to lowest total");
            Agregar("Etiqueta.OrdenAscendente", "Orden: menor a mayor total", "Order: lowest to highest total");
            Agregar("Etiqueta.ResumenLote", "Válidos: {0}   Errores: {1}   Base: {2}   IVA: {3}   TOTAL DEL LOTE: {4}",
                "Valid: {0}   Errors: {1}   Base: {2}   VAT: {3}   BATCH TOTAL: {4}");
            Agregar("Etiqueta.ResumenArreglo", "Arreglo de paquetes: capacidad {0}, redimensionado {1} vez(es)",
                "Package array: capacity {0}, resized {1} time(s)");
            Agregar("Etiqueta.SinLote", "Aún no se ha procesado ningún lote.", "No batch has been processed yet.");
            Agregar("Etiqueta.HistorialInfo", "Seleccione un lote para ver sus paquetes; seleccione un paquete para registrar su recepción.",
                "Select a batch to see its packages; select a package to register its delivery.");

            // Inspección
            Agregar("Grupo.Inspeccion", "Inspección del paquete", "Package inspection");
            Agregar("Inspeccion.SinSeleccion", "Haga clic en una fila para ver el desglose detallado.", "Click a row to see the detailed breakdown.");
            Agregar("Insp.Codigo", "Código:", "Code:");
            Agregar("Insp.Cliente", "Cliente:", "Customer:");
            Agregar("Insp.Zona", "Zona:", "Zone:");
            Agregar("Insp.Linea", "Línea de origen:", "Source line:");
            Agregar("Insp.Peso", "Peso:", "Weight:");
            Agregar("Insp.Valor", "Valor declarado:", "Declared value:");
            Agregar("Insp.Subtotal", "Subtotal por peso:", "Weight subtotal:");
            Agregar("Insp.Recargo", "Recargo por zona:", "Zone surcharge:");
            Agregar("Insp.Excedente", "Kg excedentes:", "Excess kg:");
            Agregar("Insp.Penalizacion", "Penalización sobrepeso:", "Overweight penalty:");
            Agregar("Insp.Seguro", "Seguro obligatorio:", "Mandatory insurance:");
            Agregar("Insp.CostoFijo", "Costo fijo de envío:", "Fixed shipping fee:");
            Agregar("Insp.Base", "Base imponible:", "Taxable base:");
            Agregar("Insp.Iva", "IVA:", "VAT:");
            Agregar("Insp.Total", "TOTAL A PAGAR:", "TOTAL DUE:");
            Agregar("Insp.Estado", "Estado:", "Status:");
            Agregar("Insp.Recepcion", "Fecha de recepción:", "Delivery date:");
            Agregar("Insp.NoAplica", "—", "—");

            // Columnas de las tablas
            Agregar("Col.Codigo", "Código", "Code");
            Agregar("Col.Cliente", "Cliente", "Customer");
            Agregar("Col.Zona", "Zona", "Zone");
            Agregar("Col.Peso", "Peso (kg)", "Weight (kg)");
            Agregar("Col.Valor", "Valor declarado", "Declared value");
            Agregar("Col.Subtotal", "Subtotal peso", "Weight subtotal");
            Agregar("Col.Recargo", "Recargo zona", "Zone surcharge");
            Agregar("Col.Sobrepeso", "Sobrepeso", "Overweight");
            Agregar("Col.Seguro", "Seguro", "Insurance");
            Agregar("Col.CostoFijo", "Costo fijo", "Fixed fee");
            Agregar("Col.Iva", "IVA", "VAT");
            Agregar("Col.Total", "Total", "Total");
            Agregar("Col.Estado", "Estado", "Status");
            Agregar("Col.Linea", "Línea", "Line");
            Agregar("Col.TipoError", "Motivo del rechazo", "Rejection reason");
            Agregar("Col.Detalle", "Valor detectado", "Detected value");
            Agregar("Col.Contenido", "Contenido original", "Original content");
            Agregar("Col.Lote", "Lote", "Batch");
            Agregar("Col.Fecha", "Fecha", "Date");
            Agregar("Col.TotalLineas", "Líneas", "Lines");
            Agregar("Col.Validos", "Válidos", "Valid");
            Agregar("Col.ConError", "Con error", "With errors");
            Agregar("Col.Entregados", "Entregados", "Delivered");

            // Motivos de error (Reto 1)
            Agregar("Error.LineaVacia", "Línea vacía", "Empty line");
            Agregar("Error.CamposIncompletos", "Número de campos incorrecto (se esperaban 5)", "Wrong number of fields (5 expected)");
            Agregar("Error.CodigoInvalido", "Código con formato inválido (ej. PACK-001)", "Invalid code format (e.g. PACK-001)");
            Agregar("Error.CodigoDuplicado", "Código duplicado en el lote", "Duplicated code in batch");
            Agregar("Error.ClienteInvalido", "Nombre de cliente inválido", "Invalid customer name");
            Agregar("Error.ZonaInvalida", "Zona inexistente (use A, B o C)", "Unknown zone (use A, B or C)");
            Agregar("Error.PesoNoNumerico", "El peso no es numérico", "Weight is not numeric");
            Agregar("Error.PesoNoPositivo", "El peso debe ser mayor a cero", "Weight must be greater than zero");
            Agregar("Error.ValorNoNumerico", "El valor declarado no es numérico", "Declared value is not numeric");
            Agregar("Error.ValorNegativo", "El valor declarado no puede ser negativo", "Declared value cannot be negative");
            Agregar("Error.ErrorFinanciero", "Error financiero: el monto excede la capacidad de cálculo", "Financial error: amount exceeds calculation capacity");

            // Estados de envío
            Agregar("Estado.EnTransito", "En tránsito", "In transit");
            Agregar("Estado.Entregado", "Entregado", "Delivered");

            // Barra de estado
            Agregar("Estado.ConfigOk", "Tarifas: {0}/kg · IVA {1} · Envío fijo {2}", "Rates: {0}/kg · VAT {1} · Fixed fee {2}");
            Agregar("Estado.ConfigError", "Configuración NO cargada", "Settings NOT loaded");
            Agregar("Estado.BdOk", "Base de datos lista", "Database ready");
            Agregar("Estado.BdError", "Base de datos no disponible", "Database unavailable");
            Agregar("Estado.Listo", "Listo", "Ready");
            Agregar("Estado.LoteGuardado", "Lote #{0} guardado en la base de datos", "Batch #{0} saved to the database");
            Agregar("Estado.LoteNoGuardado", "Lote procesado sin guardar en la base de datos", "Batch processed but not saved");
            Agregar("Estado.CsvExportado", "Reporte exportado: {0}", "Report exported: {0}");

            // Títulos de mensajes
            Agregar("Msg.TituloError", "Error", "Error");
            Agregar("Msg.TituloAviso", "Aviso", "Warning");
            Agregar("Msg.TituloInfo", "Información", "Information");
            Agregar("Msg.TituloConfirmar", "Confirmar", "Confirm");

            // Mensajes emergentes
            Agregar("Msg.ConfigNoEncontrada",
                "No se encontró el archivo de configuración:\r\n{0}\r\n\r\nEl procesamiento de lotes queda deshabilitado. Restaure el archivo y presione \"Recargar configuración\".",
                "The settings file was not found:\r\n{0}\r\n\r\nBatch processing is disabled. Restore the file and press \"Reload settings\".");
            Agregar("Msg.ConfigFormato", "El archivo de configuración no es un JSON válido:\r\n{0}", "The settings file is not valid JSON:\r\n{0}");
            Agregar("Msg.ConfigValor", "Hay un parámetro inválido en la configuración:\r\n{0}", "There is an invalid parameter in the settings:\r\n{0}");
            Agregar("Msg.ConfigAcceso", "No se pudo leer el archivo de configuración:\r\n{0}", "The settings file could not be read:\r\n{0}");
            Agregar("Msg.ConfigCargada", "Configuración cargada correctamente.\r\n\r\nPrecio base: {0}/kg\r\nIVA: {1}\r\nCosto fijo de envío: {2}",
                "Settings loaded successfully.\r\n\r\nBase price: {0}/kg\r\nVAT: {1}\r\nFixed shipping fee: {2}");
            Agregar("Msg.ConfigNoCargada", "No hay tarifas cargadas. Revise appsettings.json y presione \"Recargar configuración\".",
                "No rates are loaded. Check appsettings.json and press \"Reload settings\".");
            Agregar("Msg.LoteVacio", "Capture o pegue al menos una línea antes de procesar.", "Type or paste at least one line before processing.");
            Agregar("Msg.LoteProcesado", "Lote procesado.\r\n\r\nLíneas leídas: {0}\r\nPaquetes válidos: {1}\r\nLíneas con error: {2}\r\nTotal del lote: {3}",
                "Batch processed.\r\n\r\nLines read: {0}\r\nValid packages: {1}\r\nLines with errors: {2}\r\nBatch total: {3}");
            Agregar("Msg.LoteConErrores", "\r\n\r\nRevise la pestaña de errores para ver las líneas descartadas.", "\r\n\r\nCheck the errors tab to see the rejected lines.");
            Agregar("Msg.ErrorInesperado", "Ocurrió un error inesperado:\r\n{0}\r\n\r\nLa aplicación seguirá funcionando.", "An unexpected error occurred:\r\n{0}\r\n\r\nThe application will keep running.");
            Agregar("Msg.BdNoDisponible", "No se pudo preparar la base de datos:\r\n{0}\r\n\r\nPuede seguir procesando lotes, pero no se guardará el historial.",
                "The database could not be prepared:\r\n{0}\r\n\r\nYou can keep processing batches, but history will not be saved.");
            Agregar("Msg.LoteNoGuardado", "El lote se procesó, pero no se pudo guardar en la base de datos:\r\n{0}", "The batch was processed but could not be saved to the database:\r\n{0}");
            Agregar("Msg.SinDatosExportar", "Primero procese un lote con al menos un paquete válido.", "First process a batch with at least one valid package.");
            Agregar("Msg.ExportacionExitosa", "Reporte exportado correctamente en:\r\n{0}", "Report exported successfully to:\r\n{0}");
            Agregar("Msg.ExportRuta", "La ruta seleccionada no es válida:\r\n{0}", "The selected path is not valid:\r\n{0}");
            Agregar("Msg.ExportAcceso", "No tiene permisos para escribir en esa ubicación:\r\n{0}", "You do not have permission to write to that location:\r\n{0}");
            Agregar("Msg.ExportEnUso", "No se pudo escribir el archivo. ¿Está abierto en Excel u otro programa?\r\n{0}", "The file could not be written. Is it open in Excel or another program?\r\n{0}");
            Agregar("Msg.SeleccionePaquete", "Seleccione un paquete del lote en la tabla inferior del historial.", "Select a package from the lower table in the history tab.");
            Agregar("Msg.YaEntregado", "El paquete {0} ya fue entregado el {1}.", "Package {0} was already delivered on {1}.");
            Agregar("Msg.ConfirmarRecepcion", "¿Registrar la recepción (entrega) del paquete {0} de {1}?", "Register the delivery of package {0} for {1}?");
            Agregar("Msg.RecepcionRegistrada", "Recepción del paquete {0} registrada.", "Delivery of package {0} registered.");
            Agregar("Msg.BdError", "Error de base de datos:\r\n{0}", "Database error:\r\n{0}");
            Agregar("Msg.SinBaseDatos", "La base de datos no está disponible.", "The database is not available.");

            // Diálogo para guardar CSV
            Agregar("Dialogo.CsvTitulo", "Guardar reporte de auditoría", "Save audit report");
            Agregar("Dialogo.CsvFiltro", "Archivos CSV (*.csv)|*.csv|Todos los archivos (*.*)|*.*", "CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            Agregar("Csv.TotalLote", "TOTAL LOTE", "BATCH TOTAL");
        }
    }
}
