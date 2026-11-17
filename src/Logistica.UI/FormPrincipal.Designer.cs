namespace Logistica.UI
{
    partial class FormPrincipal
    {
        /// <summary>Variable del diseñador necesaria.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Limpia los recursos que se estén usando.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this._tlpSuperior = new System.Windows.Forms.TableLayoutPanel();
            this._lblTitulo = new System.Windows.Forms.Label();
            this._lblIdioma = new System.Windows.Forms.Label();
            this._cboIdioma = new System.Windows.Forms.ComboBox();
            this._btnRecargarConfiguracion = new System.Windows.Forms.Button();
            this._barraEstado = new System.Windows.Forms.StatusStrip();
            this._lblEstadoConfiguracion = new System.Windows.Forms.ToolStripStatusLabel();
            this._lblEstadoBaseDatos = new System.Windows.Forms.ToolStripStatusLabel();
            this._lblEstadoGeneral = new System.Windows.Forms.ToolStripStatusLabel();
            this._splitPrincipal = new System.Windows.Forms.SplitContainer();
            this._grpCaptura = new System.Windows.Forms.GroupBox();
            this._txtLote = new System.Windows.Forms.TextBox();
            this._panelBotonesCaptura = new System.Windows.Forms.FlowLayoutPanel();
            this._btnProcesar = new System.Windows.Forms.Button();
            this._btnCargarEjemplo = new System.Windows.Forms.Button();
            this._btnLimpiar = new System.Windows.Forms.Button();
            this._lblContadorLineas = new System.Windows.Forms.Label();
            this._lblInstrucciones = new System.Windows.Forms.Label();
            this._splitResultados = new System.Windows.Forms.SplitContainer();
            this._tabResultados = new System.Windows.Forms.TabControl();
            this._tabValidos = new System.Windows.Forms.TabPage();
            this._dgvPaquetes = new System.Windows.Forms.DataGridView();
            this._panelAccionesValidos = new System.Windows.Forms.FlowLayoutPanel();
            this._btnExportarCsv = new System.Windows.Forms.Button();
            this._btnInvertirOrden = new System.Windows.Forms.Button();
            this._lblOrden = new System.Windows.Forms.Label();
            this._lblResumenLote = new System.Windows.Forms.Label();
            this._lblResumenArreglo = new System.Windows.Forms.Label();
            this._tabErrores = new System.Windows.Forms.TabPage();
            this._dgvErrores = new System.Windows.Forms.DataGridView();
            this._tabHistorial = new System.Windows.Forms.TabPage();
            this._splitHistorial = new System.Windows.Forms.SplitContainer();
            this._dgvLotes = new System.Windows.Forms.DataGridView();
            this._dgvPaquetesHistorial = new System.Windows.Forms.DataGridView();
            this._panelAccionesHistorial = new System.Windows.Forms.FlowLayoutPanel();
            this._btnActualizarHistorial = new System.Windows.Forms.Button();
            this._btnRegistrarRecepcion = new System.Windows.Forms.Button();
            this._lblHistorialInfo = new System.Windows.Forms.Label();
            this._grpInspeccion = new System.Windows.Forms.GroupBox();
            this._tblInspeccion = new System.Windows.Forms.TableLayoutPanel();
            this._lblSinSeleccion = new System.Windows.Forms.Label();
            this._dlgGuardarCsv = new System.Windows.Forms.SaveFileDialog();
            this._tlpSuperior.SuspendLayout();
            this._barraEstado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitPrincipal)).BeginInit();
            this._splitPrincipal.Panel1.SuspendLayout();
            this._splitPrincipal.Panel2.SuspendLayout();
            this._splitPrincipal.SuspendLayout();
            this._grpCaptura.SuspendLayout();
            this._panelBotonesCaptura.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitResultados)).BeginInit();
            this._splitResultados.Panel1.SuspendLayout();
            this._splitResultados.Panel2.SuspendLayout();
            this._splitResultados.SuspendLayout();
            this._tabResultados.SuspendLayout();
            this._tabValidos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPaquetes)).BeginInit();
            this._panelAccionesValidos.SuspendLayout();
            this._tabErrores.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvErrores)).BeginInit();
            this._tabHistorial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitHistorial)).BeginInit();
            this._splitHistorial.Panel1.SuspendLayout();
            this._splitHistorial.Panel2.SuspendLayout();
            this._splitHistorial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvLotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPaquetesHistorial)).BeginInit();
            this._panelAccionesHistorial.SuspendLayout();
            this._grpInspeccion.SuspendLayout();
            this.SuspendLayout();
            //
            // _tlpSuperior
            //
            this._tlpSuperior.BackColor = System.Drawing.Color.FromArgb(32, 56, 100);
            this._tlpSuperior.ColumnCount = 4;
            this._tlpSuperior.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tlpSuperior.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this._tlpSuperior.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this._tlpSuperior.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this._tlpSuperior.Controls.Add(this._lblTitulo, 0, 0);
            this._tlpSuperior.Controls.Add(this._lblIdioma, 1, 0);
            this._tlpSuperior.Controls.Add(this._cboIdioma, 2, 0);
            this._tlpSuperior.Controls.Add(this._btnRecargarConfiguracion, 3, 0);
            this._tlpSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this._tlpSuperior.Location = new System.Drawing.Point(0, 0);
            this._tlpSuperior.Name = "_tlpSuperior";
            this._tlpSuperior.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this._tlpSuperior.RowCount = 1;
            this._tlpSuperior.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tlpSuperior.Size = new System.Drawing.Size(1264, 46);
            this._tlpSuperior.TabIndex = 0;
            //
            // _lblTitulo
            //
            this._lblTitulo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblTitulo.AutoSize = true;
            this._lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this._lblTitulo.ForeColor = System.Drawing.Color.White;
            this._lblTitulo.Name = "_lblTitulo";
            this._lblTitulo.TabIndex = 0;
            this._lblTitulo.Text = "Sistema de Procesamiento y Auditoría de Facturación";
            //
            // _lblIdioma
            //
            this._lblIdioma.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._lblIdioma.AutoSize = true;
            this._lblIdioma.ForeColor = System.Drawing.Color.White;
            this._lblIdioma.Name = "_lblIdioma";
            this._lblIdioma.TabIndex = 1;
            this._lblIdioma.Text = "Idioma:";
            //
            // _cboIdioma
            //
            this._cboIdioma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._cboIdioma.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboIdioma.FormattingEnabled = true;
            this._cboIdioma.Items.AddRange(new object[] { "Español", "English" });
            this._cboIdioma.Name = "_cboIdioma";
            this._cboIdioma.TabIndex = 2;
            this._cboIdioma.SelectedIndexChanged += new System.EventHandler(this.CboIdioma_SelectedIndexChanged);
            //
            // _btnRecargarConfiguracion
            //
            this._btnRecargarConfiguracion.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._btnRecargarConfiguracion.AutoSize = true;
            this._btnRecargarConfiguracion.Name = "_btnRecargarConfiguracion";
            this._btnRecargarConfiguracion.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this._btnRecargarConfiguracion.TabIndex = 3;
            this._btnRecargarConfiguracion.Text = "Recargar configuración";
            this._btnRecargarConfiguracion.UseVisualStyleBackColor = true;
            this._btnRecargarConfiguracion.Click += new System.EventHandler(this.BtnRecargarConfiguracion_Click);
            //
            // _barraEstado
            //
            this._barraEstado.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._lblEstadoConfiguracion,
                this._lblEstadoBaseDatos,
                this._lblEstadoGeneral});
            this._barraEstado.Name = "_barraEstado";
            this._barraEstado.TabIndex = 2;
            //
            // _lblEstadoConfiguracion
            //
            this._lblEstadoConfiguracion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this._lblEstadoConfiguracion.Name = "_lblEstadoConfiguracion";
            this._lblEstadoConfiguracion.Text = "-";
            //
            // _lblEstadoBaseDatos
            //
            this._lblEstadoBaseDatos.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this._lblEstadoBaseDatos.Name = "_lblEstadoBaseDatos";
            this._lblEstadoBaseDatos.Text = "-";
            //
            // _lblEstadoGeneral
            //
            this._lblEstadoGeneral.Name = "_lblEstadoGeneral";
            this._lblEstadoGeneral.Spring = true;
            this._lblEstadoGeneral.Text = "-";
            this._lblEstadoGeneral.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _splitPrincipal
            //
            this._splitPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitPrincipal.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this._splitPrincipal.Name = "_splitPrincipal";
            this._splitPrincipal.Panel1.Controls.Add(this._grpCaptura);
            this._splitPrincipal.Panel1.Padding = new System.Windows.Forms.Padding(6);
            this._splitPrincipal.Panel2.Controls.Add(this._splitResultados);
            this._splitPrincipal.Panel2.Padding = new System.Windows.Forms.Padding(0, 6, 6, 6);
            this._splitPrincipal.TabIndex = 1;
            //
            // _grpCaptura
            //
            this._grpCaptura.Controls.Add(this._txtLote);
            this._grpCaptura.Controls.Add(this._panelBotonesCaptura);
            this._grpCaptura.Controls.Add(this._lblInstrucciones);
            this._grpCaptura.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grpCaptura.Name = "_grpCaptura";
            this._grpCaptura.Padding = new System.Windows.Forms.Padding(8);
            this._grpCaptura.TabIndex = 0;
            this._grpCaptura.TabStop = false;
            this._grpCaptura.Text = "Captura del lote de envíos";
            //
            // _txtLote
            //
            this._txtLote.AcceptsReturn = true;
            this._txtLote.Dock = System.Windows.Forms.DockStyle.Fill;
            this._txtLote.Font = new System.Drawing.Font("Consolas", 9.75F);
            this._txtLote.Multiline = true;
            this._txtLote.Name = "_txtLote";
            this._txtLote.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._txtLote.TabIndex = 1;
            this._txtLote.WordWrap = false;
            this._txtLote.TextChanged += new System.EventHandler(this.TxtLote_TextChanged);
            //
            // _panelBotonesCaptura
            //
            this._panelBotonesCaptura.Controls.Add(this._btnProcesar);
            this._panelBotonesCaptura.Controls.Add(this._btnCargarEjemplo);
            this._panelBotonesCaptura.Controls.Add(this._btnLimpiar);
            this._panelBotonesCaptura.Controls.Add(this._lblContadorLineas);
            this._panelBotonesCaptura.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._panelBotonesCaptura.Name = "_panelBotonesCaptura";
            this._panelBotonesCaptura.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this._panelBotonesCaptura.Size = new System.Drawing.Size(360, 78);
            this._panelBotonesCaptura.TabIndex = 2;
            //
            // _btnProcesar
            //
            this._btnProcesar.BackColor = System.Drawing.Color.FromArgb(0, 120, 70);
            this._btnProcesar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnProcesar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this._btnProcesar.ForeColor = System.Drawing.Color.White;
            this._btnProcesar.Name = "_btnProcesar";
            this._btnProcesar.Size = new System.Drawing.Size(130, 32);
            this._btnProcesar.TabIndex = 0;
            this._btnProcesar.Text = "Procesar lote";
            this._btnProcesar.UseVisualStyleBackColor = false;
            this._btnProcesar.Click += new System.EventHandler(this.BtnProcesar_Click);
            //
            // _btnCargarEjemplo
            //
            this._btnCargarEjemplo.Name = "_btnCargarEjemplo";
            this._btnCargarEjemplo.Size = new System.Drawing.Size(115, 32);
            this._btnCargarEjemplo.TabIndex = 1;
            this._btnCargarEjemplo.Text = "Cargar ejemplo";
            this._btnCargarEjemplo.UseVisualStyleBackColor = true;
            this._btnCargarEjemplo.Click += new System.EventHandler(this.BtnCargarEjemplo_Click);
            //
            // _btnLimpiar
            //
            this._btnLimpiar.Name = "_btnLimpiar";
            this._btnLimpiar.Size = new System.Drawing.Size(90, 32);
            this._btnLimpiar.TabIndex = 2;
            this._btnLimpiar.Text = "Limpiar";
            this._btnLimpiar.UseVisualStyleBackColor = true;
            this._btnLimpiar.Click += new System.EventHandler(this.BtnLimpiar_Click);
            //
            // _lblContadorLineas
            //
            this._lblContadorLineas.AutoSize = true;
            this._lblContadorLineas.ForeColor = System.Drawing.Color.DimGray;
            this._lblContadorLineas.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this._lblContadorLineas.Name = "_lblContadorLineas";
            this._lblContadorLineas.TabIndex = 3;
            this._lblContadorLineas.Text = "Líneas capturadas: 0";
            //
            // _lblInstrucciones
            //
            this._lblInstrucciones.Dock = System.Windows.Forms.DockStyle.Top;
            this._lblInstrucciones.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this._lblInstrucciones.Name = "_lblInstrucciones";
            this._lblInstrucciones.Size = new System.Drawing.Size(360, 62);
            this._lblInstrucciones.TabIndex = 0;
            this._lblInstrucciones.Text = "CÓDIGO;CLIENTE;ZONA;PESO_KG;VALOR_DECLARADO";
            //
            // _splitResultados
            //
            this._splitResultados.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitResultados.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this._splitResultados.Name = "_splitResultados";
            this._splitResultados.Panel1.Controls.Add(this._tabResultados);
            this._splitResultados.Panel2.Controls.Add(this._grpInspeccion);
            this._splitResultados.TabIndex = 0;
            //
            // _tabResultados
            //
            this._tabResultados.Controls.Add(this._tabValidos);
            this._tabResultados.Controls.Add(this._tabErrores);
            this._tabResultados.Controls.Add(this._tabHistorial);
            this._tabResultados.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabResultados.Name = "_tabResultados";
            this._tabResultados.SelectedIndex = 0;
            this._tabResultados.TabIndex = 0;
            //
            // _tabValidos
            //
            this._tabValidos.Controls.Add(this._dgvPaquetes);
            this._tabValidos.Controls.Add(this._panelAccionesValidos);
            this._tabValidos.Name = "_tabValidos";
            this._tabValidos.Padding = new System.Windows.Forms.Padding(4);
            this._tabValidos.TabIndex = 0;
            this._tabValidos.Text = "Paquetes válidos";
            this._tabValidos.UseVisualStyleBackColor = true;
            //
            // _dgvPaquetes
            //
            this._dgvPaquetes.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvPaquetes.Name = "_dgvPaquetes";
            this._dgvPaquetes.TabIndex = 0;
            this._dgvPaquetes.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvPaquetes_CellClick);
            //
            // _panelAccionesValidos
            //
            this._panelAccionesValidos.Controls.Add(this._btnExportarCsv);
            this._panelAccionesValidos.Controls.Add(this._btnInvertirOrden);
            this._panelAccionesValidos.Controls.Add(this._lblOrden);
            this._panelAccionesValidos.Controls.Add(this._lblResumenLote);
            this._panelAccionesValidos.Controls.Add(this._lblResumenArreglo);
            this._panelAccionesValidos.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._panelAccionesValidos.Name = "_panelAccionesValidos";
            this._panelAccionesValidos.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this._panelAccionesValidos.Size = new System.Drawing.Size(600, 84);
            this._panelAccionesValidos.TabIndex = 1;
            this._panelAccionesValidos.SetFlowBreak(this._lblOrden, true);
            this._panelAccionesValidos.SetFlowBreak(this._lblResumenLote, true);
            //
            // _btnExportarCsv
            //
            this._btnExportarCsv.Name = "_btnExportarCsv";
            this._btnExportarCsv.Size = new System.Drawing.Size(120, 30);
            this._btnExportarCsv.TabIndex = 0;
            this._btnExportarCsv.Text = "Exportar CSV...";
            this._btnExportarCsv.UseVisualStyleBackColor = true;
            this._btnExportarCsv.Click += new System.EventHandler(this.BtnExportarCsv_Click);
            //
            // _btnInvertirOrden
            //
            this._btnInvertirOrden.Name = "_btnInvertirOrden";
            this._btnInvertirOrden.Size = new System.Drawing.Size(120, 30);
            this._btnInvertirOrden.TabIndex = 1;
            this._btnInvertirOrden.Text = "Invertir orden";
            this._btnInvertirOrden.UseVisualStyleBackColor = true;
            this._btnInvertirOrden.Click += new System.EventHandler(this.BtnInvertirOrden_Click);
            //
            // _lblOrden
            //
            this._lblOrden.AutoSize = true;
            this._lblOrden.ForeColor = System.Drawing.Color.DimGray;
            this._lblOrden.Margin = new System.Windows.Forms.Padding(6, 9, 3, 0);
            this._lblOrden.Name = "_lblOrden";
            this._lblOrden.TabIndex = 2;
            this._lblOrden.Text = "Orden";
            //
            // _lblResumenLote
            //
            this._lblResumenLote.AutoSize = true;
            this._lblResumenLote.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this._lblResumenLote.ForeColor = System.Drawing.Color.FromArgb(32, 56, 100);
            this._lblResumenLote.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this._lblResumenLote.Name = "_lblResumenLote";
            this._lblResumenLote.TabIndex = 3;
            this._lblResumenLote.Text = "-";
            //
            // _lblResumenArreglo
            //
            this._lblResumenArreglo.AutoSize = true;
            this._lblResumenArreglo.ForeColor = System.Drawing.Color.DimGray;
            this._lblResumenArreglo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this._lblResumenArreglo.Name = "_lblResumenArreglo";
            this._lblResumenArreglo.TabIndex = 4;
            this._lblResumenArreglo.Text = "-";
            //
            // _tabErrores
            //
            this._tabErrores.Controls.Add(this._dgvErrores);
            this._tabErrores.Name = "_tabErrores";
            this._tabErrores.Padding = new System.Windows.Forms.Padding(4);
            this._tabErrores.TabIndex = 1;
            this._tabErrores.Text = "Errores";
            this._tabErrores.UseVisualStyleBackColor = true;
            //
            // _dgvErrores
            //
            this._dgvErrores.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvErrores.Name = "_dgvErrores";
            this._dgvErrores.TabIndex = 0;
            //
            // _tabHistorial
            //
            this._tabHistorial.Controls.Add(this._splitHistorial);
            this._tabHistorial.Controls.Add(this._panelAccionesHistorial);
            this._tabHistorial.Name = "_tabHistorial";
            this._tabHistorial.Padding = new System.Windows.Forms.Padding(4);
            this._tabHistorial.TabIndex = 2;
            this._tabHistorial.Text = "Historial";
            this._tabHistorial.UseVisualStyleBackColor = true;
            //
            // _splitHistorial
            //
            this._splitHistorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitHistorial.Name = "_splitHistorial";
            this._splitHistorial.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this._splitHistorial.Panel1.Controls.Add(this._dgvLotes);
            this._splitHistorial.Panel2.Controls.Add(this._dgvPaquetesHistorial);
            this._splitHistorial.TabIndex = 0;
            //
            // _dgvLotes
            //
            this._dgvLotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvLotes.Name = "_dgvLotes";
            this._dgvLotes.TabIndex = 0;
            this._dgvLotes.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvLotes_CellClick);
            //
            // _dgvPaquetesHistorial
            //
            this._dgvPaquetesHistorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvPaquetesHistorial.Name = "_dgvPaquetesHistorial";
            this._dgvPaquetesHistorial.TabIndex = 0;
            this._dgvPaquetesHistorial.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvPaquetesHistorial_CellClick);
            //
            // _panelAccionesHistorial
            //
            this._panelAccionesHistorial.Controls.Add(this._btnActualizarHistorial);
            this._panelAccionesHistorial.Controls.Add(this._btnRegistrarRecepcion);
            this._panelAccionesHistorial.Controls.Add(this._lblHistorialInfo);
            this._panelAccionesHistorial.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._panelAccionesHistorial.Name = "_panelAccionesHistorial";
            this._panelAccionesHistorial.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this._panelAccionesHistorial.Size = new System.Drawing.Size(600, 64);
            this._panelAccionesHistorial.TabIndex = 1;
            //
            // _btnActualizarHistorial
            //
            this._btnActualizarHistorial.Name = "_btnActualizarHistorial";
            this._btnActualizarHistorial.Size = new System.Drawing.Size(140, 30);
            this._btnActualizarHistorial.TabIndex = 0;
            this._btnActualizarHistorial.Text = "Actualizar historial";
            this._btnActualizarHistorial.UseVisualStyleBackColor = true;
            this._btnActualizarHistorial.Click += new System.EventHandler(this.BtnActualizarHistorial_Click);
            //
            // _btnRegistrarRecepcion
            //
            this._btnRegistrarRecepcion.Name = "_btnRegistrarRecepcion";
            this._btnRegistrarRecepcion.Size = new System.Drawing.Size(140, 30);
            this._btnRegistrarRecepcion.TabIndex = 1;
            this._btnRegistrarRecepcion.Text = "Registrar recepción";
            this._btnRegistrarRecepcion.UseVisualStyleBackColor = true;
            this._btnRegistrarRecepcion.Click += new System.EventHandler(this.BtnRegistrarRecepcion_Click);
            //
            // _lblHistorialInfo
            //
            this._lblHistorialInfo.AutoSize = true;
            this._lblHistorialInfo.ForeColor = System.Drawing.Color.DimGray;
            this._lblHistorialInfo.Margin = new System.Windows.Forms.Padding(6, 9, 3, 0);
            this._lblHistorialInfo.MaximumSize = new System.Drawing.Size(520, 0);
            this._lblHistorialInfo.Name = "_lblHistorialInfo";
            this._lblHistorialInfo.TabIndex = 2;
            this._lblHistorialInfo.Text = "-";
            //
            // _grpInspeccion
            //
            this._grpInspeccion.Controls.Add(this._tblInspeccion);
            this._grpInspeccion.Controls.Add(this._lblSinSeleccion);
            this._grpInspeccion.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grpInspeccion.Name = "_grpInspeccion";
            this._grpInspeccion.Padding = new System.Windows.Forms.Padding(8);
            this._grpInspeccion.TabIndex = 0;
            this._grpInspeccion.TabStop = false;
            this._grpInspeccion.Text = "Inspección del paquete";
            //
            // _tblInspeccion
            //
            this._tblInspeccion.AutoScroll = true;
            this._tblInspeccion.ColumnCount = 2;
            this._tblInspeccion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52F));
            this._tblInspeccion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this._tblInspeccion.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tblInspeccion.Name = "_tblInspeccion";
            this._tblInspeccion.TabIndex = 1;
            //
            // _lblSinSeleccion
            //
            this._lblSinSeleccion.Dock = System.Windows.Forms.DockStyle.Top;
            this._lblSinSeleccion.ForeColor = System.Drawing.Color.DimGray;
            this._lblSinSeleccion.Name = "_lblSinSeleccion";
            this._lblSinSeleccion.Size = new System.Drawing.Size(300, 40);
            this._lblSinSeleccion.TabIndex = 0;
            this._lblSinSeleccion.Text = "-";
            //
            // _dlgGuardarCsv
            //
            this._dlgGuardarCsv.DefaultExt = "csv";
            this._dlgGuardarCsv.AddExtension = true;
            this._dlgGuardarCsv.OverwritePrompt = true;
            //
            // FormPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 761);
            this.Controls.Add(this._splitPrincipal);
            this.Controls.Add(this._tlpSuperior);
            this.Controls.Add(this._barraEstado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 680);
            this.Name = "FormPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Logística - Facturación de Fletes";
            this.Load += new System.EventHandler(this.FormPrincipal_Load);
            this._tlpSuperior.ResumeLayout(false);
            this._tlpSuperior.PerformLayout();
            this._barraEstado.ResumeLayout(false);
            this._barraEstado.PerformLayout();
            this._splitPrincipal.Panel1.ResumeLayout(false);
            this._splitPrincipal.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitPrincipal)).EndInit();
            this._splitPrincipal.ResumeLayout(false);
            this._grpCaptura.ResumeLayout(false);
            this._grpCaptura.PerformLayout();
            this._panelBotonesCaptura.ResumeLayout(false);
            this._panelBotonesCaptura.PerformLayout();
            this._splitResultados.Panel1.ResumeLayout(false);
            this._splitResultados.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitResultados)).EndInit();
            this._splitResultados.ResumeLayout(false);
            this._tabResultados.ResumeLayout(false);
            this._tabValidos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgvPaquetes)).EndInit();
            this._panelAccionesValidos.ResumeLayout(false);
            this._panelAccionesValidos.PerformLayout();
            this._tabErrores.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgvErrores)).EndInit();
            this._tabHistorial.ResumeLayout(false);
            this._splitHistorial.Panel1.ResumeLayout(false);
            this._splitHistorial.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitHistorial)).EndInit();
            this._splitHistorial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgvLotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPaquetesHistorial)).EndInit();
            this._panelAccionesHistorial.ResumeLayout(false);
            this._panelAccionesHistorial.PerformLayout();
            this._grpInspeccion.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _tlpSuperior;
        private System.Windows.Forms.Label _lblTitulo;
        private System.Windows.Forms.Label _lblIdioma;
        private System.Windows.Forms.ComboBox _cboIdioma;
        private System.Windows.Forms.Button _btnRecargarConfiguracion;
        private System.Windows.Forms.StatusStrip _barraEstado;
        private System.Windows.Forms.ToolStripStatusLabel _lblEstadoConfiguracion;
        private System.Windows.Forms.ToolStripStatusLabel _lblEstadoBaseDatos;
        private System.Windows.Forms.ToolStripStatusLabel _lblEstadoGeneral;
        private System.Windows.Forms.SplitContainer _splitPrincipal;
        private System.Windows.Forms.GroupBox _grpCaptura;
        private System.Windows.Forms.TextBox _txtLote;
        private System.Windows.Forms.FlowLayoutPanel _panelBotonesCaptura;
        private System.Windows.Forms.Button _btnProcesar;
        private System.Windows.Forms.Button _btnCargarEjemplo;
        private System.Windows.Forms.Button _btnLimpiar;
        private System.Windows.Forms.Label _lblContadorLineas;
        private System.Windows.Forms.Label _lblInstrucciones;
        private System.Windows.Forms.SplitContainer _splitResultados;
        private System.Windows.Forms.TabControl _tabResultados;
        private System.Windows.Forms.TabPage _tabValidos;
        private System.Windows.Forms.DataGridView _dgvPaquetes;
        private System.Windows.Forms.FlowLayoutPanel _panelAccionesValidos;
        private System.Windows.Forms.Button _btnExportarCsv;
        private System.Windows.Forms.Button _btnInvertirOrden;
        private System.Windows.Forms.Label _lblOrden;
        private System.Windows.Forms.Label _lblResumenLote;
        private System.Windows.Forms.Label _lblResumenArreglo;
        private System.Windows.Forms.TabPage _tabErrores;
        private System.Windows.Forms.DataGridView _dgvErrores;
        private System.Windows.Forms.TabPage _tabHistorial;
        private System.Windows.Forms.SplitContainer _splitHistorial;
        private System.Windows.Forms.DataGridView _dgvLotes;
        private System.Windows.Forms.DataGridView _dgvPaquetesHistorial;
        private System.Windows.Forms.FlowLayoutPanel _panelAccionesHistorial;
        private System.Windows.Forms.Button _btnActualizarHistorial;
        private System.Windows.Forms.Button _btnRegistrarRecepcion;
        private System.Windows.Forms.Label _lblHistorialInfo;
        private System.Windows.Forms.GroupBox _grpInspeccion;
        private System.Windows.Forms.TableLayoutPanel _tblInspeccion;
        private System.Windows.Forms.Label _lblSinSeleccion;
        private System.Windows.Forms.SaveFileDialog _dlgGuardarCsv;
    }
}
