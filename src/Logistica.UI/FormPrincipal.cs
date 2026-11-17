using System;
using System.Drawing;
using System.Windows.Forms;
using Logistica.Dominio;
using Logistica.Logica;
using Logistica.Persistencia;
using Logistica.UI.Idiomas;

namespace Logistica.UI
{
    /// <summary>
    /// Capa de presentación. Sólo captura eventos y muestra resultados:
    /// los cálculos los hace la capa de lógica y el acceso a archivos y a la
    /// base de datos lo hace la capa de persistencia.
    /// </summary>
    public partial class FormPrincipal : Form
    {
        private static readonly string[] _columnasPaquete =
        {
            "Codigo", "Cliente", "Zona", "Peso", "Valor", "Subtotal",
            "Recargo", "Sobrepeso", "Seguro", "CostoFijo", "Iva", "Total"
        };

        private static readonly string[] _columnasPaqueteHistorial =
        {
            "Codigo", "Cliente", "Zona", "Peso", "Valor", "Subtotal",
            "Recargo", "Sobrepeso", "Seguro", "CostoFijo", "Iva", "Total", "Estado"
        };

        private static readonly string[] _columnasErrores = { "Linea", "TipoError", "Detalle", "Contenido" };

        private static readonly string[] _columnasLotes = { "Lote", "Fecha", "TotalLineas", "Validos", "ConError", "Entregados", "Total" };

        private static readonly string[] _clavesInspeccion =
        {
            "Insp.Codigo", "Insp.Cliente", "Insp.Zona", "Insp.Linea", "Insp.Peso", "Insp.Valor",
            "Insp.Subtotal", "Insp.Recargo", "Insp.Excedente", "Insp.Penalizacion", "Insp.Seguro",
            "Insp.CostoFijo", "Insp.Base", "Insp.Iva", "Insp.Total", "Insp.Estado", "Insp.Recepcion"
        };

        private readonly Localizador _localizador;
        private readonly ConfiguracionRepositorio _configuracionRepositorio;
        private readonly ExportadorCsv _exportadorCsv;

        private ConfiguracionAplicacion _configuracion;
        private ProcesadorLotes _procesador;
        private RepositorioEnvios _repositorioEnvios;

        private ResultadoLote _resultadoActual;
        private ResumenLote[] _historial;
        private Paquete[] _paquetesHistorial;
        private Paquete _paqueteInspeccionado;
        private bool _ordenDescendente;

        private Label[] _lblTitulosInspeccion;
        private Label[] _lblValoresInspeccion;

        public FormPrincipal()
        {
            InitializeComponent();

            _localizador = new Localizador();
            _configuracionRepositorio = new ConfiguracionRepositorio();
            _exportadorCsv = new ExportadorCsv();
            _historial = new ResumenLote[0];
            _paquetesHistorial = new Paquete[0];
            _ordenDescendente = true;

            // Suscripción al evento personalizado del localizador (RF2).
            _localizador.IdiomaCambiado += Localizador_IdiomaCambiado;

            ConfigurarTabla(_dgvPaquetes, _columnasPaquete);
            ConfigurarTabla(_dgvPaquetesHistorial, _columnasPaqueteHistorial);
            ConfigurarTabla(_dgvErrores, _columnasErrores);
            ConfigurarTabla(_dgvLotes, _columnasLotes);
            _dgvErrores.Columns["Contenido"].FillWeight = 220;
            _dgvErrores.Columns["TipoError"].FillWeight = 180;

            // Las tablas de paquetes tienen muchas columnas: se ajustan al contenido con barra horizontal.
            _dgvPaquetes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            _dgvPaquetesHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            ConstruirPanelInspeccion();
        }

        // ------------------------------------------------------------------
        // Eventos del formulario
        // ------------------------------------------------------------------

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            _splitPrincipal.SplitterDistance = 380;
            _splitResultados.SplitterDistance = Math.Max(300, _splitResultados.Width - 330);
            _splitHistorial.SplitterDistance = 170;

            _cboIdioma.SelectedIndex = (int)Idioma.Espanol;
            AplicarTextos();

            CargarConfiguracion(false);
            InicializarBaseDatos();
            CargarHistorial();
        }

        private void CboIdioma_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cboIdioma.SelectedIndex < 0)
            {
                return;
            }
            _localizador.IdiomaActual = (Idioma)_cboIdioma.SelectedIndex;
        }

        private void Localizador_IdiomaCambiado(object sender, EventArgs e)
        {
            AplicarTextos();
        }

        private void BtnRecargarConfiguracion_Click(object sender, EventArgs e)
        {
            if (CargarConfiguracion(true) && _repositorioEnvios == null)
            {
                InicializarBaseDatos();
                CargarHistorial();
            }
        }

        private void TxtLote_TextChanged(object sender, EventArgs e)
        {
            ActualizarContadorLineas();
        }

        private void BtnCargarEjemplo_Click(object sender, EventArgs e)
        {
            _txtLote.Text = DatosEjemplo.LoteDemostracion;
            _txtLote.SelectionStart = 0;
            _txtLote.Focus();
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            _txtLote.Clear();
            _resultadoActual = null;
            _paqueteInspeccionado = null;
            _ordenDescendente = true;
            _dgvPaquetes.Rows.Clear();
            _dgvErrores.Rows.Clear();
            MostrarInspeccion(null);
            ActualizarResumen();
            MostrarEstado(_localizador.Texto("Estado.Listo"));
        }

        private void BtnProcesar_Click(object sender, EventArgs e)
        {
            if (_procesador == null)
            {
                MostrarAviso(_localizador.Texto("Msg.ConfigNoCargada"));
                return;
            }
            if (string.IsNullOrWhiteSpace(_txtLote.Text))
            {
                MostrarAviso(_localizador.Texto("Msg.LoteVacio"));
                _txtLote.Focus();
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                _resultadoActual = _procesador.Procesar(_txtLote.Text);
                _ordenDescendente = true;
                _paqueteInspeccionado = null;
            }
            catch (Exception ex)
            {
                MostrarError(_localizador.Formatear("Msg.ErrorInesperado", ex.Message));
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            MostrarResultadoActual();
            GuardarLoteEnBaseDatos();

            string mensaje = _localizador.Formatear(
                "Msg.LoteProcesado",
                _resultadoActual.TotalLineas,
                _resultadoActual.CantidadValidos,
                _resultadoActual.CantidadErrores,
                _localizador.Moneda(_resultadoActual.SumaTotal));
            if (_resultadoActual.CantidadErrores > 0)
            {
                mensaje += _localizador.Texto("Msg.LoteConErrores");
            }
            MessageBox.Show(this, mensaje, _localizador.Texto("Msg.TituloInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnInvertirOrden_Click(object sender, EventArgs e)
        {
            if (_resultadoActual == null || _resultadoActual.CantidadValidos == 0)
            {
                MostrarAviso(_localizador.Texto("Msg.SinDatosExportar"));
                return;
            }

            _resultadoActual.ReemplazarPaquetes(_procesador.InvertirOrden(_resultadoActual.Paquetes));
            _ordenDescendente = !_ordenDescendente;
            LlenarTablaPaquetes(_dgvPaquetes, _resultadoActual.Paquetes, false);
            ActualizarResumen();
        }

        private void BtnExportarCsv_Click(object sender, EventArgs e)
        {
            if (_resultadoActual == null || _resultadoActual.CantidadValidos == 0)
            {
                MostrarAviso(_localizador.Texto("Msg.SinDatosExportar"));
                return;
            }

            _dlgGuardarCsv.Title = _localizador.Texto("Dialogo.CsvTitulo");
            _dlgGuardarCsv.Filter = _localizador.Texto("Dialogo.CsvFiltro");
            _dlgGuardarCsv.FileName = "auditoria_lote_" + _resultadoActual.FechaProceso.ToString("yyyyMMdd_HHmmss") + ".csv";

            if (_dlgGuardarCsv.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            string ruta = _dlgGuardarCsv.FileName;
            try
            {
                _exportadorCsv.Exportar(_resultadoActual, ruta, ObtenerEncabezados(_columnasPaquete), _localizador.Texto("Csv.TotalLote"));
                MostrarEstado(_localizador.Formatear("Estado.CsvExportado", ruta));
                MessageBox.Show(this, _localizador.Formatear("Msg.ExportacionExitosa", ruta), _localizador.Texto("Msg.TituloInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (PersistenciaException ex)
            {
                MostrarError(TraducirFalloPersistencia(ex, ruta));
            }
        }

        private void DgvPaquetes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            MostrarInspeccion(ObtenerPaqueteDeFila(_dgvPaquetes, e.RowIndex));
        }

        private void DgvPaquetesHistorial_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            MostrarInspeccion(ObtenerPaqueteDeFila(_dgvPaquetesHistorial, e.RowIndex));
        }

        private void DgvLotes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _dgvLotes.Rows.Count)
            {
                return;
            }

            ResumenLote lote = _dgvLotes.Rows[e.RowIndex].Tag as ResumenLote;
            if (lote != null)
            {
                CargarPaquetesDeLote(lote.Id);
            }
        }

        private void BtnActualizarHistorial_Click(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void BtnRegistrarRecepcion_Click(object sender, EventArgs e)
        {
            if (_repositorioEnvios == null)
            {
                MostrarAviso(_localizador.Texto("Msg.SinBaseDatos"));
                return;
            }

            Paquete paquete = null;
            if (_dgvPaquetesHistorial.CurrentRow != null)
            {
                paquete = _dgvPaquetesHistorial.CurrentRow.Tag as Paquete;
            }
            if (paquete == null)
            {
                MostrarAviso(_localizador.Texto("Msg.SeleccionePaquete"));
                return;
            }
            if (paquete.Estado == EstadoEnvio.Entregado)
            {
                string fecha = paquete.FechaRecepcion.HasValue ? _localizador.Fecha(paquete.FechaRecepcion.Value) : "-";
                MostrarAviso(_localizador.Formatear("Msg.YaEntregado", paquete.Codigo, fecha));
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                this,
                _localizador.Formatear("Msg.ConfirmarRecepcion", paquete.Codigo, paquete.Cliente),
                _localizador.Texto("Msg.TituloConfirmar"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _repositorioEnvios.RegistrarRecepcion(paquete, DateTime.Now);
                long loteId = paquete.LoteId;
                CargarHistorial();
                CargarPaquetesDeLote(loteId);
                MostrarInspeccion(paquete);
                MostrarEstado(_localizador.Formatear("Msg.RecepcionRegistrada", paquete.Codigo));
            }
            catch (InvalidOperationException ex)
            {
                MostrarAviso(ex.Message);
            }
            catch (PersistenciaException ex)
            {
                MostrarError(_localizador.Formatear("Msg.BdError", ex.Message));
            }
        }

        // ------------------------------------------------------------------
        // Configuración y base de datos
        // ------------------------------------------------------------------

        /// <summary>RF1: carga appsettings.json y crea los servicios de cálculo.</summary>
        private bool CargarConfiguracion(bool informarExito)
        {
            try
            {
                _configuracion = _configuracionRepositorio.Cargar();
                _procesador = new ProcesadorLotes(new CalculadoraTarifas(_configuracion.Tarifas));
                _btnProcesar.Enabled = true;

                Idioma idiomaConfigurado = Localizador.DesdeCodigo(_configuracion.IdiomaPredeterminado);
                if (!informarExito && _cboIdioma.SelectedIndex != (int)idiomaConfigurado)
                {
                    _cboIdioma.SelectedIndex = (int)idiomaConfigurado;
                }

                ActualizarBarraEstado();
                if (informarExito)
                {
                    ConfiguracionTarifas t = _configuracion.Tarifas;
                    MessageBox.Show(
                        this,
                        _localizador.Formatear("Msg.ConfigCargada", _localizador.Moneda(t.PrecioBasePorKg), _localizador.Porcentaje(t.TasaIva), _localizador.Moneda(t.CostoFijoEnvio)),
                        _localizador.Texto("Msg.TituloInfo"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                return true;
            }
            catch (ConfiguracionException ex)
            {
                _configuracion = null;
                _procesador = null;
                _btnProcesar.Enabled = false;
                ActualizarBarraEstado();
                MostrarError(TraducirErrorConfiguracion(ex));
                return false;
            }
        }

        private void InicializarBaseDatos()
        {
            if (_configuracion == null)
            {
                _repositorioEnvios = null;
                ActualizarBarraEstado();
                return;
            }

            try
            {
                RepositorioEnvios repositorio = new RepositorioEnvios(_configuracion.RutaBaseDatos);
                repositorio.InicializarBaseDatos();
                _repositorioEnvios = repositorio;
            }
            catch (PersistenciaException ex)
            {
                _repositorioEnvios = null;
                MostrarAviso(_localizador.Formatear("Msg.BdNoDisponible", ex.Message));
            }
            ActualizarBarraEstado();
        }

        private void GuardarLoteEnBaseDatos()
        {
            if (_repositorioEnvios == null || _resultadoActual == null)
            {
                MostrarEstado(_localizador.Texto("Estado.LoteNoGuardado"));
                return;
            }

            try
            {
                long loteId = _repositorioEnvios.GuardarLote(_resultadoActual);
                MostrarEstado(_localizador.Formatear("Estado.LoteGuardado", loteId));
                CargarHistorial();
            }
            catch (PersistenciaException ex)
            {
                MostrarEstado(_localizador.Texto("Estado.LoteNoGuardado"));
                MostrarAviso(_localizador.Formatear("Msg.LoteNoGuardado", ex.Message));
            }
        }

        private void CargarHistorial()
        {
            if (_repositorioEnvios == null)
            {
                _historial = new ResumenLote[0];
                LlenarTablaLotes();
                return;
            }

            try
            {
                _historial = _repositorioEnvios.ObtenerHistorial();
            }
            catch (PersistenciaException ex)
            {
                _historial = new ResumenLote[0];
                MostrarError(_localizador.Formatear("Msg.BdError", ex.Message));
            }
            LlenarTablaLotes();
        }

        private void CargarPaquetesDeLote(long loteId)
        {
            if (_repositorioEnvios == null)
            {
                return;
            }

            try
            {
                _paquetesHistorial = _repositorioEnvios.ObtenerPaquetesDeLote(loteId);
            }
            catch (PersistenciaException ex)
            {
                _paquetesHistorial = new Paquete[0];
                MostrarError(_localizador.Formatear("Msg.BdError", ex.Message));
            }
            LlenarTablaPaquetes(_dgvPaquetesHistorial, _paquetesHistorial, true);
        }

        // ------------------------------------------------------------------
        // Presentación de datos
        // ------------------------------------------------------------------

        private static void ConfigurarTabla(DataGridView tabla, string[] columnas)
        {
            tabla.AllowUserToAddRows = false;
            tabla.AllowUserToDeleteRows = false;
            tabla.AllowUserToResizeRows = false;
            tabla.ReadOnly = true;
            tabla.MultiSelect = false;
            tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tabla.RowHeadersVisible = false;
            tabla.BackgroundColor = Color.White;
            tabla.BorderStyle = BorderStyle.FixedSingle;
            tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 246, 252);
            tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            tabla.Columns.Clear();
            for (int i = 0; i < columnas.Length; i++)
            {
                DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
                columna.Name = columnas[i];
                columna.HeaderText = columnas[i];
                columna.SortMode = DataGridViewColumnSortMode.NotSortable;
                columna.MinimumWidth = 50;

                switch (columnas[i])
                {
                    case "Codigo":
                    case "Cliente":
                    case "Zona":
                    case "TipoError":
                    case "Detalle":
                    case "Contenido":
                    case "Fecha":
                    case "Estado":
                        columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        break;
                    default:
                        columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        break;
                }

                if (columnas[i] == "Total")
                {
                    columna.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                }
                tabla.Columns.Add(columna);
            }
        }

        private void ConstruirPanelInspeccion()
        {
            int filas = _clavesInspeccion.Length;
            _lblTitulosInspeccion = new Label[filas];
            _lblValoresInspeccion = new Label[filas];

            _tblInspeccion.SuspendLayout();
            _tblInspeccion.RowStyles.Clear();
            _tblInspeccion.RowCount = filas + 1;

            for (int i = 0; i < filas; i++)
            {
                bool esTotal = _clavesInspeccion[i] == "Insp.Total";

                Label titulo = new Label();
                titulo.AutoSize = true;
                titulo.Margin = new Padding(3, 5, 3, 5);
                titulo.ForeColor = Color.FromArgb(64, 64, 64);
                titulo.Font = new Font("Segoe UI", 9F, esTotal ? FontStyle.Bold : FontStyle.Regular);

                Label valor = new Label();
                valor.AutoSize = true;
                valor.Margin = new Padding(3, 5, 3, 5);
                valor.Font = new Font("Segoe UI", esTotal ? 11F : 9F, esTotal ? FontStyle.Bold : FontStyle.Regular);
                valor.ForeColor = esTotal ? Color.FromArgb(0, 110, 60) : Color.Black;

                _lblTitulosInspeccion[i] = titulo;
                _lblValoresInspeccion[i] = valor;
                _tblInspeccion.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                _tblInspeccion.Controls.Add(titulo, 0, i);
                _tblInspeccion.Controls.Add(valor, 1, i);
            }

            _tblInspeccion.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _tblInspeccion.ResumeLayout(true);
        }

        /// <summary>RF2: actualiza en pantalla todos los textos del idioma activo.</summary>
        private void AplicarTextos()
        {
            Text = _localizador.Texto("Form.Titulo");
            _lblTitulo.Text = _localizador.Texto("Form.Titulo");
            _lblIdioma.Text = _localizador.Texto("Etiqueta.Idioma");
            _btnRecargarConfiguracion.Text = _localizador.Texto("Boton.RecargarConfig");
            _grpCaptura.Text = _localizador.Texto("Grupo.Captura");
            _lblInstrucciones.Text = _localizador.Texto("Etiqueta.Instrucciones");
            _btnProcesar.Text = _localizador.Texto("Boton.Procesar");
            _btnCargarEjemplo.Text = _localizador.Texto("Boton.Ejemplo");
            _btnLimpiar.Text = _localizador.Texto("Boton.Limpiar");
            _tabHistorial.Text = _localizador.Texto("Tab.Historial");
            _btnExportarCsv.Text = _localizador.Texto("Boton.Exportar");
            _btnInvertirOrden.Text = _localizador.Texto("Boton.Invertir");
            _btnActualizarHistorial.Text = _localizador.Texto("Boton.ActualizarHistorial");
            _btnRegistrarRecepcion.Text = _localizador.Texto("Boton.RegistrarRecepcion");
            _lblHistorialInfo.Text = _localizador.Texto("Etiqueta.HistorialInfo");
            _grpInspeccion.Text = _localizador.Texto("Grupo.Inspeccion");
            _lblSinSeleccion.Text = _localizador.Texto("Inspeccion.SinSeleccion");

            TraducirEncabezados(_dgvPaquetes);
            TraducirEncabezados(_dgvPaquetesHistorial);
            TraducirEncabezados(_dgvErrores);
            TraducirEncabezados(_dgvLotes);

            for (int i = 0; i < _clavesInspeccion.Length; i++)
            {
                _lblTitulosInspeccion[i].Text = _localizador.Texto(_clavesInspeccion[i]);
            }

            // Vuelve a dibujar los datos para que montos, fechas y motivos usen el nuevo idioma.
            if (_resultadoActual != null)
            {
                LlenarTablaPaquetes(_dgvPaquetes, _resultadoActual.Paquetes, false);
                LlenarTablaErrores(_resultadoActual.Errores);
            }
            LlenarTablaPaquetes(_dgvPaquetesHistorial, _paquetesHistorial, true);
            LlenarTablaLotes();
            MostrarInspeccion(_paqueteInspeccionado);
            ActualizarResumen();
            ActualizarContadorLineas();
            ActualizarBarraEstado();
            MostrarEstado(_localizador.Texto("Estado.Listo"));
        }

        private void TraducirEncabezados(DataGridView tabla)
        {
            foreach (DataGridViewColumn columna in tabla.Columns)
            {
                columna.HeaderText = _localizador.Texto("Col." + columna.Name);
            }
        }

        private string[] ObtenerEncabezados(string[] columnas)
        {
            string[] encabezados = new string[columnas.Length];
            for (int i = 0; i < columnas.Length; i++)
            {
                encabezados[i] = _localizador.Texto("Col." + columnas[i]);
            }
            return encabezados;
        }

        private void MostrarResultadoActual()
        {
            LlenarTablaPaquetes(_dgvPaquetes, _resultadoActual.Paquetes, false);
            LlenarTablaErrores(_resultadoActual.Errores);
            MostrarInspeccion(null);
            ActualizarResumen();
            _tabResultados.SelectedTab = _tabValidos;
        }

        /// <summary>Recorre el arreglo de paquetes y agrega una fila por cada uno.</summary>
        private void LlenarTablaPaquetes(DataGridView tabla, Paquete[] paquetes, bool incluirEstado)
        {
            tabla.Rows.Clear();
            if (paquetes == null)
            {
                return;
            }

            for (int i = 0; i < paquetes.Length; i++)
            {
                Paquete p = paquetes[i];
                DesgloseCosto d = p.Desglose;
                object[] valores = new object[incluirEstado ? 13 : 12];
                valores[0] = p.Codigo;
                valores[1] = p.Cliente;
                valores[2] = "Zona " + p.Zona;
                valores[3] = _localizador.Numero(p.PesoKg);
                valores[4] = _localizador.Moneda(p.ValorDeclarado);
                valores[5] = d == null ? "-" : _localizador.Moneda(d.SubtotalPeso);
                valores[6] = d == null ? "-" : _localizador.Moneda(d.RecargoZona);
                valores[7] = d == null ? "-" : _localizador.Moneda(d.PenalizacionSobrepeso);
                valores[8] = d == null ? "-" : _localizador.Moneda(d.Seguro);
                valores[9] = d == null ? "-" : _localizador.Moneda(d.CostoFijoEnvio);
                valores[10] = d == null ? "-" : _localizador.Moneda(d.Iva);
                valores[11] = d == null ? "-" : _localizador.Moneda(d.Total);
                if (incluirEstado)
                {
                    valores[12] = _localizador.TextoEstado(p.Estado);
                }

                int fila = tabla.Rows.Add(valores);
                tabla.Rows[fila].Tag = p;
                if (incluirEstado && p.Estado == EstadoEnvio.Entregado)
                {
                    tabla.Rows[fila].DefaultCellStyle.ForeColor = Color.FromArgb(0, 110, 60);
                }
            }
            tabla.ClearSelection();
        }

        private void LlenarTablaErrores(ErrorLinea[] errores)
        {
            _dgvErrores.Rows.Clear();
            int i = 0;
            while (i < errores.Length)
            {
                ErrorLinea error = errores[i];
                int fila = _dgvErrores.Rows.Add(
                    error.NumeroLinea.ToString(),
                    _localizador.TextoError(error.Tipo),
                    error.Detalle,
                    error.ContenidoOriginal.Trim());
                _dgvErrores.Rows[fila].DefaultCellStyle.ForeColor = Color.FromArgb(170, 20, 20);
                i++;
            }
            _dgvErrores.ClearSelection();
        }

        private void LlenarTablaLotes()
        {
            _dgvLotes.Rows.Clear();
            for (int i = 0; i < _historial.Length; i++)
            {
                ResumenLote lote = _historial[i];
                int fila = _dgvLotes.Rows.Add(
                    "#" + lote.Id,
                    _localizador.Fecha(lote.Fecha),
                    lote.TotalLineas.ToString(),
                    lote.PaquetesValidos.ToString(),
                    lote.PaquetesConError.ToString(),
                    lote.PaquetesEntregados + " / " + lote.PaquetesValidos,
                    _localizador.Moneda(lote.Total));
                _dgvLotes.Rows[fila].Tag = lote;
            }
            _dgvLotes.ClearSelection();
        }

        /// <summary>RF5: proyecta los metadatos y el desglose del paquete seleccionado.</summary>
        private void MostrarInspeccion(Paquete paquete)
        {
            _paqueteInspeccionado = paquete;
            _lblSinSeleccion.Visible = paquete == null;
            _tblInspeccion.Visible = paquete != null;
            if (paquete == null)
            {
                return;
            }

            DesgloseCosto d = paquete.Desglose;
            string noAplica = _localizador.Texto("Insp.NoAplica");
            string[] valores = new string[_clavesInspeccion.Length];

            valores[0] = paquete.Codigo;
            valores[1] = paquete.Cliente;
            valores[2] = "Zona " + paquete.Zona;
            valores[3] = paquete.NumeroLinea > 0 ? paquete.NumeroLinea.ToString() : noAplica;
            valores[4] = _localizador.Numero(paquete.PesoKg) + " kg";
            valores[5] = _localizador.Moneda(paquete.ValorDeclarado);
            if (d != null)
            {
                valores[6] = _localizador.Moneda(d.SubtotalPeso);
                valores[7] = _localizador.Moneda(d.RecargoZona) + "  (" + _localizador.Porcentaje(d.PorcentajeRecargoZona) + ")";
                valores[8] = d.KilosExcedentes > 0 ? _localizador.Numero(d.KilosExcedentes) + " kg" : noAplica;
                valores[9] = _localizador.Moneda(d.PenalizacionSobrepeso);
                valores[10] = _localizador.Moneda(d.Seguro);
                valores[11] = _localizador.Moneda(d.CostoFijoEnvio);
                valores[12] = _localizador.Moneda(d.BaseImponible);
                valores[13] = _localizador.Moneda(d.Iva);
                valores[14] = _localizador.Moneda(d.Total);
            }
            valores[15] = _localizador.TextoEstado(paquete.Estado);
            valores[16] = paquete.FechaRecepcion.HasValue ? _localizador.Fecha(paquete.FechaRecepcion.Value) : noAplica;

            for (int i = 0; i < valores.Length; i++)
            {
                _lblValoresInspeccion[i].Text = valores[i] ?? noAplica;
            }
        }

        private void ActualizarResumen()
        {
            int validos = _resultadoActual == null ? 0 : _resultadoActual.CantidadValidos;
            int errores = _resultadoActual == null ? 0 : _resultadoActual.CantidadErrores;
            _tabValidos.Text = _localizador.Formatear("Tab.Validos", validos);
            _tabErrores.Text = _localizador.Formatear("Tab.Errores", errores);

            bool hayLote = _resultadoActual != null;
            _btnExportarCsv.Enabled = hayLote && validos > 0;
            _btnInvertirOrden.Enabled = hayLote && validos > 1;

            if (!hayLote)
            {
                _lblOrden.Text = string.Empty;
                _lblResumenLote.Text = _localizador.Texto("Etiqueta.SinLote");
                _lblResumenArreglo.Text = string.Empty;
                return;
            }

            _lblOrden.Text = _localizador.Texto(_ordenDescendente ? "Etiqueta.OrdenDescendente" : "Etiqueta.OrdenAscendente");
            _lblResumenLote.Text = _localizador.Formatear(
                "Etiqueta.ResumenLote",
                validos,
                errores,
                _localizador.Moneda(_resultadoActual.SumaBaseImponible),
                _localizador.Moneda(_resultadoActual.SumaIva),
                _localizador.Moneda(_resultadoActual.SumaTotal));
            _lblResumenArreglo.Text = _localizador.Formatear("Etiqueta.ResumenArreglo", _resultadoActual.CapacidadArreglo, _resultadoActual.Redimensionamientos);
        }

        private void ActualizarContadorLineas()
        {
            string texto = _txtLote.Text.TrimEnd();
            int lineas = texto.Length == 0 ? 0 : texto.Split('\n').Length;
            _lblContadorLineas.Text = _localizador.Formatear("Etiqueta.ContadorLineas", lineas);
        }

        private void ActualizarBarraEstado()
        {
            if (_configuracion != null)
            {
                ConfiguracionTarifas t = _configuracion.Tarifas;
                _lblEstadoConfiguracion.Text = _localizador.Formatear("Estado.ConfigOk", _localizador.Moneda(t.PrecioBasePorKg), _localizador.Porcentaje(t.TasaIva), _localizador.Moneda(t.CostoFijoEnvio));
                _lblEstadoConfiguracion.ForeColor = Color.Black;
            }
            else
            {
                _lblEstadoConfiguracion.Text = _localizador.Texto("Estado.ConfigError");
                _lblEstadoConfiguracion.ForeColor = Color.Firebrick;
            }

            if (_repositorioEnvios != null)
            {
                _lblEstadoBaseDatos.Text = _localizador.Texto("Estado.BdOk");
                _lblEstadoBaseDatos.ToolTipText = _repositorioEnvios.RutaArchivo;
                _lblEstadoBaseDatos.ForeColor = Color.Black;
            }
            else
            {
                _lblEstadoBaseDatos.Text = _localizador.Texto("Estado.BdError");
                _lblEstadoBaseDatos.ToolTipText = string.Empty;
                _lblEstadoBaseDatos.ForeColor = Color.Firebrick;
            }
        }

        // ------------------------------------------------------------------
        // Utilidades
        // ------------------------------------------------------------------

        private static Paquete ObtenerPaqueteDeFila(DataGridView tabla, int indiceFila)
        {
            if (indiceFila < 0 || indiceFila >= tabla.Rows.Count)
            {
                return null;
            }
            return tabla.Rows[indiceFila].Tag as Paquete;
        }

        private string TraducirErrorConfiguracion(ConfiguracionException ex)
        {
            switch (ex.Motivo)
            {
                case MotivoErrorConfiguracion.ArchivoNoEncontrado:
                    return _localizador.Formatear("Msg.ConfigNoEncontrada", ex.Ruta);
                case MotivoErrorConfiguracion.FormatoInvalido:
                    return _localizador.Formatear("Msg.ConfigFormato", ex.Message);
                case MotivoErrorConfiguracion.ValorInvalido:
                    return _localizador.Formatear("Msg.ConfigValor", ex.Message);
                default:
                    return _localizador.Formatear("Msg.ConfigAcceso", ex.Message);
            }
        }

        private string TraducirFalloPersistencia(PersistenciaException ex, string ruta)
        {
            switch (ex.Tipo)
            {
                case TipoFalloPersistencia.RutaInvalida:
                    return _localizador.Formatear("Msg.ExportRuta", ruta);
                case TipoFalloPersistencia.AccesoDenegado:
                    return _localizador.Formatear("Msg.ExportAcceso", ruta);
                case TipoFalloPersistencia.ArchivoEnUso:
                    return _localizador.Formatear("Msg.ExportEnUso", ruta);
                case TipoFalloPersistencia.SinDatos:
                    return _localizador.Texto("Msg.SinDatosExportar");
                default:
                    return _localizador.Formatear("Msg.BdError", ex.Message);
            }
        }

        private void MostrarEstado(string mensaje)
        {
            _lblEstadoGeneral.Text = mensaje;
        }

        private void MostrarAviso(string mensaje)
        {
            MessageBox.Show(this, mensaje, _localizador.Texto("Msg.TituloAviso"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void MostrarError(string mensaje)
        {
            MessageBox.Show(this, mensaje, _localizador.Texto("Msg.TituloError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
