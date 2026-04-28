#:package Terminal.Gui@2.0.1-develop.1

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

var jsonPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(SourceDirectory(), "agenda.json");

var agenda = AgendaStore.Load(jsonPath);
RunAgendaEditor(jsonPath, agenda);

static string SourceDirectory([CallerFilePath] string sourcePath = "") {
    return Path.GetDirectoryName(sourcePath) ?? Directory.GetCurrentDirectory();
}

static void RunAgendaEditor(string jsonPath, List<Contacto> contactos) {
    using IApplication app = Application.Create();
    app.Init();

    using var root = new Runnable {
        Width = Dim.Fill(),
        Height = Dim.Fill()
    };

    using var window = new Window {
        X = 0,
        Y = 1,
        Width = Dim.Fill(),
        Height = Dim.Fill(1),
        Title = $"Agenda - {Path.GetFileName(jsonPath)}"
    };

    var filtered = new List<string>();
    var filteredContacts = new List<Contacto>();
    int? selectedIndexInFiltered = null;
    var modalOpen = false;
    TextField txtBuscar = null!;
    ListView list = null!;

    var menu = new MenuBar {
        X = 0,
        Y = 0,
        Width = Dim.Fill(),
        Menus = new[] {
            new MenuBarItem("_Agenda", new MenuItem[] {
                new("_Abrir...", "Carga otro archivo JSON", () => OpenAgenda()),
                new("Guardar _como...", "Guarda en otro archivo", () => SaveAs()),
                null!,
                new("_Salir", "Sale del programa", () => RequestExit()),
            }),
            new MenuBarItem("_Contacto", new MenuItem[] {
                new("_Agregar...", "Agrega un contacto", () => NewContact()),
                new("_Editar...", "Edita el contacto seleccionado", () => EditContact()),
                new("_Borrar", "Borra el contacto seleccionado", () => DeleteContact()),
            }),
        }
    };

    var lblBuscar = new Label { Text = "Buscar:", X = 1, Y = 2 };
    txtBuscar = new TextField {
        X = Pos.Right(lblBuscar) + 1,
        Y = 2,
        Width = Dim.Fill(2)
    };

    var header = new Label {
        Text = FormatHeader(),
        X = 1, Y = 5, Width = Dim.Fill() - 2 };

    list = new ListView {
        X = 1, Y = 6, Width = Dim.Fill() - 2, Height = Dim.Fill() - 7,
        CanFocus = true
    };

    void BindListSource() {
        list.SetSource(new System.Collections.ObjectModel.ObservableCollection<string>(filtered));
    }

    BindListSource();

    root.Add(
        window,
        menu
    );

    window.Add(
        lblBuscar,
        txtBuscar,
        header,
        list
    );

    void UpdateWindowTitle() {
        window.Title = $"Agenda - {Path.GetFileName(jsonPath)}";
    }

    int? ShowModalMessage(string title, string message, params string[] buttons) {
        modalOpen = true;

        try {
            return MessageBox.Query(app, title, message, buttons);
        } finally {
            modalOpen = false;
        }
    }

    bool TrySaveTo(string path, bool showConfirmation) {
        try {
            AgendaStore.Save(path, contactos);
            jsonPath = path;
            UpdateWindowTitle();

            if (showConfirmation) {
                ShowModalMessage(
                    "Guardado",
                    $"Se guardó {Path.GetFileName(jsonPath)}.",
                    "OK");
            }

            return true;
        } catch (Exception ex) {
            ShowModalMessage(
                "No se pudo guardar",
                ex.Message,
                "OK");
            return false;
        }
    }

    void AutoSave() {
        TrySaveTo(jsonPath, showConfirmation: false);
    }

    string? PickAgendaSaveFile() {
        modalOpen = true;

        try {
            using var dialog = new SaveDialog {
                Title = "Guardar agenda como...",
                OpenMode = OpenMode.File,
            };

            dialog.AllowedTypes = new List<IAllowedType> {
                new AllowedType("Archivos JSON", ".json")
            };

            app.Run(dialog);

            if (dialog.Canceled || string.IsNullOrWhiteSpace(dialog.Path))
                return null;

            var path = dialog.Path;

            if (!Path.HasExtension(path))
                path += ".json";

            return path;
        } finally {
            modalOpen = false;
        }
    }

    void SaveAs() {
        var selectedPath = PickAgendaSaveFile();

        if (selectedPath is null)
            return;

        TrySaveTo(selectedPath, showConfirmation: true);
    }

    void RequestExit() {
        app.RequestStop(root);
    }

    void RefreshList(Contacto? select = null) {
        var query = txtBuscar.Text?.ToString()?.Trim() ?? "";

        filteredContacts = contactos
            .Where(c => Matches(c, query))
            .OrderBy(c => c.Apellido)
            .ThenBy(c => c.Nombre)
            .ToList();

        filtered = filteredContacts
            .Select(FormatContact)
            .ToList();

        BindListSource();

        if (filteredContacts.Count > 0) {
            var index = select is null ? 0 : filteredContacts.IndexOf(select);

            if (index < 0)
                index = 0;

            list.SelectedItem = index;
            selectedIndexInFiltered = index;
            list.EnsureSelectedItemVisible();
        } else {
            selectedIndexInFiltered = null;
        }

        list.SetNeedsDraw();
    }

    void NewContact() {
        var nuevo = OpenContactDialog(null);

        if (nuevo is null)
            return;

        contactos.Add(nuevo);
        AutoSave();
        txtBuscar.Text = "";
        RefreshList(nuevo);
        list.SetFocus();
    }

    void EditContact() {
        if (selectedIndexInFiltered is not int index ||
            index < 0 ||
            index >= filteredContacts.Count) {
            return;
        }

        var original = filteredContacts[index];
        var edited = OpenContactDialog(original);

        if (edited is null)
            return;

        original.Nombre = edited.Nombre;
        original.Apellido = edited.Apellido;
        original.Domicilio = edited.Domicilio;
        original.Telefonos = edited.Telefonos;
        AutoSave();
        RefreshList(original);
    }

    void DeleteContact() {
        if (selectedIndexInFiltered is not int index ||
            index < 0 ||
            index >= filteredContacts.Count) {
            return;
        }

        var c = filteredContacts[index];

        var confirm = ShowModalMessage(
            "Borrar contacto",
            $"¿Borrar a {c.Apellido}, {c.Nombre}?",
            "Sí",
            "No"
        );

        if (confirm == 0) {
            contactos.Remove(c);
            AutoSave();
            RefreshList();
        }
    }

    void Save() {
        SaveAs();
    }

    void OpenAgenda() {
        var selectedPath = OpenAgendaFile();

        if (selectedPath is null)
            return;

        try {
            contactos = AgendaStore.Load(selectedPath);
            jsonPath = selectedPath;
            UpdateWindowTitle();
            txtBuscar.Text = "";
            RefreshList();
            window.SetNeedsDraw();
        } catch (Exception ex) {
            ShowModalMessage(
                "No se pudo cargar",
                ex.Message,
                "OK"
            );
        }
    }

    string? OpenAgendaFile() {
        modalOpen = true;

        try {
            using var dialog = new OpenDialog {
                Title = "Elegir archivo de agenda JSON",
                OpenMode = OpenMode.File,
                AllowsMultipleSelection = false,
                AllowedTypes = new List<IAllowedType> {
                    new AllowedType("Archivos JSON", ".json")
                }
            };

            app.Run(dialog);

            if (dialog.Result is null)
                return null;

            return dialog.FilePaths.FirstOrDefault() ?? dialog.Path;
        } finally {
            modalOpen = false;
        }
    }

    Contacto? OpenContactDialog(Contacto? original) {
        modalOpen = true;

        try {
            return ShowContactDialog(app, original);
        } finally {
            modalOpen = false;
        }
    }

    txtBuscar.TextChanged += (_, _) => RefreshList();

    list.ValueChanged += (_, _) => {
        var index = list.SelectedItem;

        if (index is >= 0 && index < filteredContacts.Count) {
            selectedIndexInFiltered = index.Value;
        }
    };

    list.Accepted += (_, _) => EditContact();

    bool HandleMainShortcut(Key args) {
        if (args == Key.F2 || args == Key.N.WithCtrl) {
            NewContact();
            return true;
        }

        if (args == Key.F3 || args == Key.E.WithCtrl) {
            EditContact();
            return true;
        }

        if (args == Key.F4 || args == Key.B.WithCtrl) {
            DeleteContact();
            return true;
        }

        if (args == Key.F5 || args == Key.A.WithCtrl) {
            OpenAgenda();
            return true;
        }

        if (args == Key.S.WithCtrl || args == Key.G.WithCtrl) {
            Save();
            return true;
        }

        if (args == Key.Q.WithCtrl || args == Key.Esc) {
            RequestExit();
            return true;
        }

        return false;
    }

    app.Keyboard.KeyDown += (_, args) => {
        if (modalOpen) {
            if (args == Key.Esc) {
                app.RequestStop();
                args.Handled = true;
            }

            return;
        }

        if (HandleMainShortcut(args)) {
            args.Handled = true;
        }
    };

    RefreshList();
    UpdateWindowTitle();
    txtBuscar.SetFocus();

    app.Run(root);
}

static Contacto? ShowContactDialog(IApplication app, Contacto? original) {
    using var dialog = new ContactEditorDialog(app, original);
    return dialog.ShowDialog();
}

static string FormatHeader() {
    return string.Join("  ",
        Column("Apellido",  14),
        Column("Nombre",    14),
        Column("Domicilio", 24),
        Column("Teléfonos", 30));
}

static string FormatContact(Contacto c) {
    var telefonos = c.Telefonos.Count == 0
        ? "(sin teléfonos)"
        : string.Join(" / ", c.Telefonos);

    return string.Join("  ",
        Column(c.Apellido,  14),
        Column(c.Nombre,    14),
        Column(c.Domicilio, 24),
        Column(telefonos,   30));
}

static string Column(string? value, int width) {
    var text = string.IsNullOrWhiteSpace(value) ? "" : value.Trim();

    if (text.Length > width)
        text = width <= 3 ? text[..width] : text[..(width - 3)] + "...";

    return text.PadRight(width);
}

static bool Matches(Contacto c, string query) {
    if (string.IsNullOrWhiteSpace(query))
        return true;

    return Contains(c.Nombre, query)
        || Contains(c.Apellido, query)
        || Contains(c.Domicilio, query)
        || c.Telefonos.Any(t => Contains(t, query));
}

static bool Contains(string? value, string query) {
    return value?.Contains(query, StringComparison.OrdinalIgnoreCase) == true;
}

public sealed class ContactEditorDialog : Dialog {
    private readonly IApplication app;
    private readonly TextField txtNombre;
    private readonly TextField txtApellido;
    private readonly TextField txtDomicilio;
    private readonly TextView txtTelefonos;
    private readonly Label error;
    private bool closing;


    public ContactEditorDialog(IApplication app, Contacto? original) {
        this.app = app;

        Title = original is null ? "Nuevo contacto" : "Editar contacto";
        Width  = 58;
        Height = 20;


        txtNombre = new TextField {
            Text = original?.Nombre ?? "",
            X = 14, Y = 2, Width = 40 };

        txtApellido = new TextField {
            Text = original?.Apellido ?? "",
            X = 14, Y = 4, Width = 40 };

        txtDomicilio = new TextField {
            Text = original?.Domicilio ?? "",
            X = 14, Y = 6, Width = 40 };

        txtTelefonos = new TextView {
            Text = string.Join(Environment.NewLine, original?.Telefonos ?? []),
            X = 14, Y = 8, Width = 40,
            Height = 6
        };

        error = new Label {
            Text = "",
            X = 2, Y = 15, Width = Dim.Fill() - 2
        };

        var btnAceptar  = new Button { Text = "Aceptar", IsDefault = true };
        var btnCancelar = new Button { Text = "Cancelar" };

        btnAceptar.Accepting += (_, args) => {
            Accept();
            args.Handled = true;
        };

        btnAceptar.Accepted += (_, _) => Accept();

        btnCancelar.Accepting += (_, args) => {
            CloseDialog();
            args.Handled = true;
        };

        KeyDown += (_, args) => HandleKeyDown(args);

        Add(
            new Label { Text = "Nombre:", X = 2, Y = 2 },
            txtNombre,
            new Label { Text = "Apellido:", X = 2, Y = 4 },
            txtApellido,
            new Label { Text = "Domicilio:", X = 2, Y = 6 },
            txtDomicilio,
            new Label { Text = "Teléfonos:", X = 2, Y = 8 },
            txtTelefonos,
            error
        );

        AddButton(btnCancelar);
        AddButton(btnAceptar);
        txtNombre.SetFocus();
    }

    public Contacto? ContactResult { get; private set; }

    public Contacto? ShowDialog() {
        app.Keyboard.KeyDown += OnKeyboardKeyDown;

        try {
            app.Run(this);
        } finally {
            app.Keyboard.KeyDown -= OnKeyboardKeyDown;
        }

        return ContactResult;
    }

    private void Accept() {
        if (closing)
            return;

        var edited = new Contacto {
            Nombre = txtNombre.Text?.ToString()?.Trim() ?? "",
            Apellido = txtApellido.Text?.ToString()?.Trim() ?? "",
            Domicilio = txtDomicilio.Text?.ToString()?.Trim() ?? "",
            Telefonos = ReadPhones(txtTelefonos.Text?.ToString() ?? "")
        };

        if (string.IsNullOrWhiteSpace(edited.Nombre) &&
            string.IsNullOrWhiteSpace(edited.Apellido)) {
            error.Text = "El contacto necesita al menos nombre o apellido.";
            error.SetNeedsDraw();
            return;
        }

        ContactResult = edited;
        CloseDialog();
    }

    private void CloseDialog() {
        if (closing)
            return;

        closing = true;
        app.RequestStop(this);
    }

    private void HandleKeyDown(Key args) {
        if (args == Key.F3 || args == Key.S.WithCtrl) {
            Accept();
            args.Handled = true;
            return;
        }

        if (args == Key.Esc) {
            CloseDialog();
            args.Handled = true;
        }
    }

    private void OnKeyboardKeyDown(object? _, Key args) {
        HandleKeyDown(args);
    }

    private static List<string> ReadPhones(string value) {
        return value
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }
}

public sealed class Contacto {
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = "";

    [JsonPropertyName("domicilio")]
    public string Domicilio { get; set; } = "";

    [JsonPropertyName("telefonos")]
    public List<string> Telefonos { get; set; } = [];
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(List<Contacto>))]
public partial class AgendaJsonContext : JsonSerializerContext {
}

public static class AgendaStore {
    public static List<Contacto> Load(string path) {
        if (!File.Exists(path))
            return [];

        var json = File.ReadAllText(path);

        if (string.IsNullOrWhiteSpace(json))
            return [];

        return JsonSerializer.Deserialize(json, AgendaJsonContext.Default.ListContacto) ?? [];
    }

    public static void Save(string path, List<Contacto> contactos) {
        var json = JsonSerializer.Serialize(contactos, AgendaJsonContext.Default.ListContacto);
        File.WriteAllText(path, json);
    }
}
