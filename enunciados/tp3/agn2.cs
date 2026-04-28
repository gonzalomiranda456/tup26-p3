#:package Terminal.Gui@2.*

// Editor de agenda de contactos con Terminal.Gui v2 — programa file-based de C# 14.
//
//   dotnet run agenda.cs                 → busca agenda.json en el directorio actual,
//                                          y si no existe abre el selector.
//   dotnet run agenda.cs ./otra.json     → abre el archivo indicado (lo crea si no existe).

using System.Data;
using System.Text.Json;
using Terminal.Gui;

string? archivoInicial = args.Length > 0 ? args[0] : null;
new AgendaApp(archivoInicial).Ejecutar();

// =====================================================================
//  Modelo
// =====================================================================
public class Contacto
{
    public string Nombre    { get; set; } = "";
    public string Apellido  { get; set; } = "";
    public string Domicilio { get; set; } = "";
    public List<string> Telefonos { get; set; } = [];
}

// =====================================================================
//  Aplicación
// =====================================================================
public class AgendaApp(string? archivoInicial)
{
    private const int MaxTelefonos = 4;

    private string? _archivo = archivoInicial;
    private List<Contacto> _contactos = [];
    private List<Contacto> _vista     = [];   // contactos filtrados/ordenados que se muestran
    private bool _modificado;

    // Widgets
    private DataTable  _dt          = new();
    private TableView  _tabla       = null!;
    private TextField  _txtBusqueda = null!;
    private Label      _lblEstado   = null!;
    private Window     _win         = null!;

    // -----------------------------------------------------------------
    //  Punto de entrada
    // -----------------------------------------------------------------
    public void Ejecutar()
    {
        // Resolver archivo ANTES de inicializar Terminal.Gui:
        //  1) argumento explícito que existe  → cargar
        //  2) sin argumento + agenda.json existe → cargar
        //  3) caso contrario  → diálogo después de Application.Init
        if (_archivo is not null && File.Exists(_archivo))
        {
            CargarDesdeArchivo(_archivo);
        }
        else if (_archivo is null && File.Exists("agenda.json"))
        {
            _archivo = "agenda.json";
            CargarDesdeArchivo(_archivo);
        }

        Application.Init();
        try
        {
            if (_archivo is null)
            {
                var elegido = ElegirArchivoAbrir();
                if (elegido is null)
                {
                    var r = MessageBox.Query(
                        "Agenda",
                        "No se eligió ningún archivo.\n¿Crear una agenda nueva en 'agenda.json'?",
                        "Sí", "No");
                    if (r == 0)
                    {
                        _archivo   = Path.GetFullPath("agenda.json");
                        _contactos = [];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    _archivo = elegido;
                    if (File.Exists(elegido)) CargarDesdeArchivo(elegido);
                    else                      _contactos = [];
                }
            }

            _vista = OrdenarVista(_contactos);
            CorrerAplicacion();
        }
        finally
        {
            Application.Shutdown();
        }
    }

    // -----------------------------------------------------------------
    //  Persistencia
    // -----------------------------------------------------------------
    private void CargarDesdeArchivo(string ruta)
    {
        try
        {
            var json = File.ReadAllText(ruta);
            _contactos = string.IsNullOrWhiteSpace(json)
                ? []
                : (JsonSerializer.Deserialize<List<Contacto>>(json,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? []);
        }
        catch
        {
            _contactos = [];
        }
        _modificado = false;
    }

    private bool GuardarEn(string ruta)
    {
        try
        {
            var opts = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder       = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            File.WriteAllText(ruta, JsonSerializer.Serialize(_contactos, opts));
            _archivo    = ruta;
            _modificado = false;
            if (_win is not null) _win.Title = TituloVentana();
            ActualizarEstado();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error al guardar", ex.Message, "Aceptar");
            return false;
        }
    }

    // -----------------------------------------------------------------
    //  UI principal
    // -----------------------------------------------------------------
    private void CorrerAplicacion()
    {
        _dt = new DataTable();
        _dt.Columns.Add("Nombre");
        _dt.Columns.Add("Apellido");
        _dt.Columns.Add("Domicilio");
        _dt.Columns.Add("Teléfonos");

        var top = new Toplevel();

        var menu = new MenuBar
        {
            Menus = new[]
            {
                new MenuBarItem("_Archivo", new[]
                {
                    new MenuItem("_Nuevo",            "Vacía la agenda en memoria",  () => NuevaAgenda()),
                    new MenuItem("_Abrir...",         "Carga otro archivo JSON",     () => AbrirOtro()),
                    new MenuItem("_Guardar",          "Guarda en el archivo actual", () => Guardar()),
                    new MenuItem("Guardar _como...",  "Guarda en otro archivo",      () => GuardarComo()),
                    null!,
                    new MenuItem("_Salir",            "Sale del programa",           () => IntentarSalir()),
                }),
                new MenuBarItem("_Contacto", new[]
                {
                    new MenuItem("_Agregar...", "Agrega un contacto",             () => AgregarContacto()),
                    new MenuItem("_Editar...",  "Edita el contacto seleccionado", () => EditarContacto()),
                    new MenuItem("_Borrar",     "Borra el contacto seleccionado", () => BorrarContacto()),
                }),
            }
        };

        _win = new Window
        {
            X      = 0,
            Y      = 1,
            Width  = Dim.Fill(),
            Height = Dim.Fill(1),
            Title  = TituloVentana(),
        };

        var lblBusq = new Label { X = 1, Y = 0, Text = "Buscar:" };
        _txtBusqueda = new TextField
        {
            X     = Pos.Right(lblBusq) + 1,
            Y     = 0,
            Width = Dim.Fill(2),
        };
        _txtBusqueda.TextChanged += (_, _) => AplicarFiltro();

        _tabla = new TableView
        {
            X              = 0,
            Y              = 2,
            Width          = Dim.Fill(),
            Height         = Dim.Fill(1),
            Table          = new DataTableSource(_dt),
            FullRowSelect  = true,
        };
        _tabla.KeyDown += TablaKeyDown;

        _lblEstado = new Label
        {
            X     = 1,
            Y     = Pos.AnchorEnd(1),
            Width = Dim.Fill(2),
            Text  = "",
        };

        _win.Add(lblBusq, _txtBusqueda, _tabla, _lblEstado);
        top.Add(menu, _win);

        // Atajos globales (F2 / F3 / Ctrl+S / Ctrl+Q)
        top.KeyDown += GlobalKeyDown;

        // Confirmar descarte de cambios al cerrar (X / Alt+F4 / Application.RequestStop)
        top.Closing += (_, e) =>
        {
            if (!ConfirmarDescartarCambios()) e.Cancel = true;
        };

        RefrescarTabla();
        ActualizarEstado();

        Application.Run(top);
        top.Dispose();
    }

    private void GlobalKeyDown(object? sender, Key e)
    {
        if (e.KeyCode == KeyCode.F2)
        {
            AgregarContacto();
            e.Handled = true;
        }
        else if (e.KeyCode == KeyCode.F3)
        {
            _txtBusqueda?.SetFocus();
            e.Handled = true;
        }
        else if (e == Key.S.WithCtrl)
        {
            Guardar();
            e.Handled = true;
        }
        else if (e == Key.Q.WithCtrl)
        {
            IntentarSalir();
            e.Handled = true;
        }
    }

    private void TablaKeyDown(object? sender, Key e)
    {
        if (e.KeyCode == KeyCode.Enter)
        {
            EditarContacto();
            e.Handled = true;
        }
        else if (e.KeyCode == KeyCode.Delete)
        {
            BorrarContacto();
            e.Handled = true;
        }
    }

    // -----------------------------------------------------------------
    //  Acciones de contactos
    // -----------------------------------------------------------------
    private void AgregarContacto()
    {
        var c = DialogoContacto("Agregar contacto", null);
        if (c is null) return;
        _contactos.Add(c);
        _modificado = true;
        AplicarFiltro();
        if (_win is not null) _win.Title = TituloVentana();
    }

    private Contacto? ContactoSeleccionado()
    {
        if (_tabla is null) return null;
        var idx = _tabla.SelectedRow;
        if (idx < 0 || idx >= _vista.Count) return null;
        return _vista[idx];
    }

    private void EditarContacto()
    {
        var sel = ContactoSeleccionado();
        if (sel is null) return;

        var editado = DialogoContacto("Editar contacto", sel);
        if (editado is null) return;

        sel.Nombre    = editado.Nombre;
        sel.Apellido  = editado.Apellido;
        sel.Domicilio = editado.Domicilio;
        sel.Telefonos = editado.Telefonos;

        _modificado = true;
        AplicarFiltro();
        if (_win is not null) _win.Title = TituloVentana();
    }

    private void BorrarContacto()
    {
        var sel = ContactoSeleccionado();
        if (sel is null) return;

        var nombre = $"{sel.Nombre} {sel.Apellido}".Trim();
        if (string.IsNullOrEmpty(nombre)) nombre = "(sin nombre)";

        var r = MessageBox.Query(
            "Confirmar borrado",
            $"¿Borrar el contacto «{nombre}»?",
            "Sí", "No");
        if (r != 0) return;

        _contactos.Remove(sel);
        _modificado = true;
        AplicarFiltro();
        if (_win is not null) _win.Title = TituloVentana();
    }

    // -----------------------------------------------------------------
    //  Diálogo agregar / editar
    // -----------------------------------------------------------------
    private Contacto? DialogoContacto(string titulo, Contacto? existente)
    {
        var nombre    = existente?.Nombre    ?? "";
        var apellido  = existente?.Apellido  ?? "";
        var domicilio = existente?.Domicilio ?? "";
        var tels      = (existente?.Telefonos ?? []).Take(MaxTelefonos).ToList();
        while (tels.Count < MaxTelefonos) tels.Add("");

        Contacto? resultado = null;

        var dlg = new Dialog
        {
            Title  = titulo,
            Width  = 64,
            Height = 17,
        };

        var lblN = new Label     { X = 1, Y = 0, Text = "Nombre:" };
        var txtN = new TextField { X = 13, Y = 0, Width = Dim.Fill(2), Text = nombre    };

        var lblA = new Label     { X = 1, Y = 1, Text = "Apellido:" };
        var txtA = new TextField { X = 13, Y = 1, Width = Dim.Fill(2), Text = apellido  };

        var lblD = new Label     { X = 1, Y = 2, Text = "Domicilio:" };
        var txtD = new TextField { X = 13, Y = 2, Width = Dim.Fill(2), Text = domicilio };

        var lblT    = new Label { X = 1, Y = 4, Text = $"Teléfonos (hasta {MaxTelefonos}):" };
        var txtTels = new TextField[MaxTelefonos];
        for (int i = 0; i < MaxTelefonos; i++)
        {
            var l = new Label     { X = 1, Y = 5 + i, Text = $"{i + 1}." };
            var t = new TextField { X = 4, Y = 5 + i, Width = Dim.Fill(2), Text = tels[i] };
            txtTels[i] = t;
            dlg.Add(l, t);
        }

        var btnOk = new Button { Text = "_Aceptar", IsDefault = true };
        btnOk.Accepting += (_, _) =>
        {
            var n = (txtN.Text ?? "").Trim();
            var a = (txtA.Text ?? "").Trim();
            if (n.Length == 0 && a.Length == 0)
            {
                MessageBox.ErrorQuery(
                    "Datos incompletos",
                    "Debe ingresar al menos un nombre o un apellido.",
                    "Aceptar");
                return; // dejar el diálogo abierto
            }
            resultado = new Contacto
            {
                Nombre    = n,
                Apellido  = a,
                Domicilio = (txtD.Text ?? "").Trim(),
                Telefonos = txtTels
                    .Select(t => (t.Text ?? "").Trim())
                    .Where(s => s.Length > 0)
                    .Take(MaxTelefonos)
                    .ToList(),
            };
            Application.RequestStop();
        };

        var btnCancel = new Button { Text = "_Cancelar" };
        btnCancel.Accepting += (_, _) =>
        {
            resultado = null;
            Application.RequestStop();
        };

        dlg.AddButton(btnOk);
        dlg.AddButton(btnCancel);
        dlg.Add(lblN, txtN, lblA, txtA, lblD, txtD, lblT);

        Application.Run(dlg);
        dlg.Dispose();

        return resultado;
    }

    // -----------------------------------------------------------------
    //  Búsqueda y refresco de la vista
    // -----------------------------------------------------------------
    private void AplicarFiltro()
    {
        var termino = (_txtBusqueda?.Text ?? "").Trim();
        IEnumerable<Contacto> filtrados = _contactos;
        if (termino.Length > 0)
        {
            filtrados = _contactos.Where(c =>
                Contiene(c.Nombre,    termino)
             || Contiene(c.Apellido,  termino)
             || Contiene(c.Domicilio, termino)
             || c.Telefonos.Any(p => Contiene(p, termino)));
        }
        _vista = OrdenarVista(filtrados);
        RefrescarTabla();
        ActualizarEstado();
    }

    private static List<Contacto> OrdenarVista(IEnumerable<Contacto> origen) =>
        origen
            .OrderBy(c => c.Apellido, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy (c => c.Nombre,   StringComparer.CurrentCultureIgnoreCase)
            .ToList();

    private static bool Contiene(string fuente, string termino) =>
        fuente?.Contains(termino, StringComparison.CurrentCultureIgnoreCase) ?? false;

    private void RefrescarTabla()
    {
        _dt.Rows.Clear();
        foreach (var c in _vista)
        {
            _dt.Rows.Add(
                c.Nombre,
                c.Apellido,
                c.Domicilio,
                string.Join(" / ", c.Telefonos));
        }
        _tabla?.SetNeedsDraw();
    }

    private void ActualizarEstado()
    {
        if (_lblEstado is null) return;

        var total    = _contactos.Count;
        var visibles = _vista.Count;
        var marca    = _modificado ? "  •  modificado" : "";
        var arch     = _archivo is not null ? Path.GetFileName(_archivo) : "(sin archivo)";

        _lblEstado.Text =
            $"{visibles}/{total} contactos | {arch}{marca} | " +
            "F2 agregar  Enter editar  Supr borrar  F3 buscar  Ctrl+S guardar  Ctrl+Q salir";
    }

    private string TituloVentana()
    {
        var arch  = _archivo is not null ? Path.GetFileName(_archivo) : "(sin archivo)";
        var marca = _modificado ? " *" : "";
        return $"Agenda — {arch}{marca}";
    }

    // -----------------------------------------------------------------
    //  Comandos del menú
    // -----------------------------------------------------------------
    private void NuevaAgenda()
    {
        if (!ConfirmarDescartarCambios()) return;
        _contactos  = [];
        _archivo    = null;
        _modificado = false;
        if (_win is not null) _win.Title = TituloVentana();
        AplicarFiltro();
    }

    private void AbrirOtro()
    {
        if (!ConfirmarDescartarCambios()) return;
        var elegido = ElegirArchivoAbrir();
        if (elegido is null) return;
        if (File.Exists(elegido))
        {
            CargarDesdeArchivo(elegido);
            _archivo = elegido;
        }
        else
        {
            _archivo    = elegido;
            _contactos  = [];
            _modificado = false;
        }
        if (_win is not null) _win.Title = TituloVentana();
        AplicarFiltro();
    }

    private bool Guardar() => _archivo is null ? GuardarComo() : GuardarEn(_archivo);

    private bool GuardarComo()
    {
        var elegido = ElegirArchivoGuardar();
        if (elegido is null) return false;
        return GuardarEn(elegido);
    }

    private void IntentarSalir()
    {
        if (!ConfirmarDescartarCambios()) return;
        Application.RequestStop();
    }

    private bool ConfirmarDescartarCambios()
    {
        if (!_modificado) return true;
        var r = MessageBox.Query(
            "Cambios sin guardar",
            "Hay cambios sin guardar.\n¿Qué desea hacer?",
            "Guardar", "Descartar", "Cancelar");
        if (r == 0) return Guardar();
        if (r == 1) return true;        // descartar
        return false;                    // cancelar / esc
    }

    // -----------------------------------------------------------------
    //  Selectores de archivo
    // -----------------------------------------------------------------
    private string? ElegirArchivoAbrir()
    {
        var od = new OpenDialog
        {
            Title                   = "Abrir agenda",
            AllowsMultipleSelection = false,
            OpenMode                = OpenMode.File,
        };
        od.AllowedTypes = new List<IAllowedType>
        {
            new AllowedType("Archivos JSON", ".json"),
        };
        Application.Run(od);
        string? path = null;
        if (!od.Canceled && od.Path is { Length: > 0 })
        {
            path = od.Path;
        }
        od.Dispose();
        return path;
    }

    private string? ElegirArchivoGuardar()
    {
        var sd = new SaveDialog
        {
            Title    = "Guardar agenda como…",
            OpenMode = OpenMode.File,
        };
        sd.AllowedTypes = new List<IAllowedType>
        {
            new AllowedType("Archivos JSON", ".json"),
        };
        Application.Run(sd);
        string? path = null;
        if (!sd.Canceled && sd.Path is { Length: > 0 })
        {
            path = sd.Path;
            if (!Path.HasExtension(path)) path += ".json";
        }
        sd.Dispose();
        return path;
    }
}