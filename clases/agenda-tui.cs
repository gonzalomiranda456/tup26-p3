#:package Terminal.Gui@2.0.1
#:package Microsoft.Data.Sqlite@9.0.0
#:package Dapper@2.1.35
#:package Dapper.Contrib@2.0.78
#:property PublishAot=false

#nullable enable

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

Console.OutputEncoding = Encoding.UTF8;

string dbPath = args.Length > 0 ? args[0] : "agenda.db";

SqliteAgendaStore store = new(dbPath);
JsonAgendaIO json = new();

using IApplication app = Application.Create().Init();
app.Run(new AgendaWindow(store, json));

public sealed class AgendaWindow : Runnable {
    private readonly SqliteAgendaStore store;
    private readonly JsonAgendaIO json;
    private readonly TextField searchField = new();
    private readonly CheckBox favoritesOnly = new();
    private readonly ListView<Contacto> contactsList = new();
    private readonly Label detailsLabel = new();
    private readonly Label statusLabel = new();

    private List<Contacto> contacts = new();
    private List<Contacto> filteredContacts = new();

    public AgendaWindow(SqliteAgendaStore store, JsonAgendaIO json) {
        this.store = store;
        this.json = json;

        Title = "Agenda - Terminal.Gui";
        Width = Dim.Fill();
        Height = Dim.Fill();

        Menu.DefaultBorderStyle = LineStyle.Single;

        BuildLayout();
        LoadContacts();
        searchField.SetFocus();
    }

    private void BuildLayout() {
        MenuBar menu = new() {
            Menus = [
                new MenuBarItem("_Archivo", [
                    new MenuItem("_Importar JSON...", "Ctrl+I", ImportFromJson),
                    new MenuItem("_Exportar JSON...", "Ctrl+E", ExportToJson),
                    new MenuItem("_Recargar", "Ctrl+R", ReloadContacts),
                    new MenuItem("_Salir", "Ctrl+Q", RequestExit)
                ]),
                new MenuBarItem("_Contactos", [
                    new MenuItem("_Nuevo", "F2 / Ctrl+N", NewContact),
                    new MenuItem("_Editar", "F3 / Enter", EditSelectedContact),
                    new MenuItem("E_liminar", "Del", DeleteSelectedContact)
                ])
            ]
        };

        Label searchLabel = new() {
            Text = "Buscar:",
            X = 1,
            Y = 2
        };

        searchField.X = Pos.Right(searchLabel) + 1;
        searchField.Y = Pos.Top(searchLabel);
        searchField.Width = Dim.Percent(42);
        searchField.ValueChanged += (_, _) => RefreshContacts();

        favoritesOnly.Text = "Solo _favoritos";
        favoritesOnly.X = Pos.Right(searchField) + 2;
        favoritesOnly.Y = Pos.Top(searchField);
        favoritesOnly.ValueChanged += (_, _) => RefreshContacts();

        Button newButton = new() {
            Text = "Nuevo",
            X = 1,
            Y = Pos.Bottom(searchField) + 1
        };
        newButton.Accepted += (_, _) => NewContact();

        Button editButton = new() {
            Text = "Editar",
            X = Pos.Right(newButton) + 1,
            Y = Pos.Top(newButton)
        };
        editButton.Accepted += (_, _) => EditSelectedContact();

        Button deleteButton = new() {
            Text = "Eliminar",
            X = Pos.Right(editButton) + 1,
            Y = Pos.Top(newButton)
        };
        deleteButton.Accepted += (_, _) => DeleteSelectedContact();

        FrameView listPanel = new() {
            Title = "Contactos",
            X = 0,
            Y = 5,
            Width = Dim.Percent(40),
            Height = Dim.Fill(2)
        };
        listPanel.BorderStyle = LineStyle.Single;

        contactsList.X = 0;
        contactsList.Y = 0;
        contactsList.Width = Dim.Fill();
        contactsList.Height = Dim.Fill();
        contactsList.ValueChanged += (_, _) => RefreshDetails();
        contactsList.Accepted += (_, _) => EditSelectedContact();
        listPanel.Add(contactsList);

        FrameView detailsPanel = new() {
            Title = "Detalle",
            X = Pos.Right(listPanel),
            Y = Pos.Top(listPanel),
            Width = Dim.Fill(),
            Height = Dim.Fill(2)
        };
        detailsPanel.BorderStyle = LineStyle.Single;

        detailsLabel.X = 1;
        detailsLabel.Y = 0;
        detailsLabel.Width = Dim.Fill(1);
        detailsLabel.Height = Dim.Fill();
        detailsPanel.Add(detailsLabel);

        statusLabel.X = 1;
        statusLabel.Y = Pos.AnchorEnd(1);
        statusLabel.Width = Dim.Fill();

        Add(
            menu,
            searchLabel, searchField, favoritesOnly,
            newButton, editButton, deleteButton,
            listPanel,
            detailsPanel,
            statusLabel
        );
    }

    protected override bool OnKeyDown(Key key) {
        switch (key) {
            case var _ when key == Key.F2 || key == Key.N.WithCtrl:
                NewContact();
                return true;

            case var _ when key == Key.F3 || (key == Key.Enter && contactsList.HasFocus):
                EditSelectedContact();
                return true;

            case var _ when key == Key.Delete:
                DeleteSelectedContact();
                return true;

            case var _ when key == Key.I.WithCtrl:
                ImportFromJson();
                return true;

            case var _ when key == Key.E.WithCtrl:
                ExportToJson();
                return true;

            case var _ when key == Key.R.WithCtrl:
                ReloadContacts();
                return true;

            case var _ when key == Key.Q.WithCtrl:
                RequestExit();
                return true;

            case var _ when key == Key.F4:
                searchField.SetFocus();
                return true;

            default:
                return base.OnKeyDown(key);
        }
    }

    private void LoadContacts() {
        contacts = store.GetAll();
        RefreshContacts();
        SetStatus("Datos cargados desde la base.");
    }

    private void ReloadContacts() {
        LoadContacts();
        SetStatus("Datos recargados desde la base.");
    }

    private void NewContact() {
        Contacto contact = new();

        ContactDialog dialog = new("Nuevo contacto", contact);
        App!.Run(dialog);

        if (!dialog.Saved || dialog.Contact is null) {
            return;
        }

        int newId = store.Insert(dialog.Contact);
        dialog.Contact.Id = newId;
        contacts.Add(dialog.Contact);

        SetStatus($"Contacto '{dialog.Contact.Nombre}' creado.");
        RefreshContacts(newId);
    }

    private void EditSelectedContact() {
        Contacto? selected = contactsList.Value;
        if (selected is null) {
            MessageBox.Query(App!, "Editar", "Seleccioná un contacto.", "OK");
            return;
        }

        ContactDialog dialog = new("Editar contacto", selected.Clone());
        App!.Run(dialog);

        if (!dialog.Saved || dialog.Contact is null) {
            return;
        }

        store.Update(dialog.Contact);
        int index = contacts.FindIndex(contact => contact.Id == selected.Id);
        if (index >= 0) {
            contacts[index] = dialog.Contact;
        }

        SetStatus($"Contacto '{dialog.Contact.Nombre}' actualizado.");
        RefreshContacts(dialog.Contact.Id);
    }

    private void DeleteSelectedContact() {
        Contacto? selected = contactsList.Value;
        if (selected is null) {
            return;
        }

        int answer = MessageBox.Query(
            App!,
            "Eliminar contacto",
            $"¿Eliminar a {selected.Nombre}?",
            "No",
            "Sí") ?? 0;

        if (answer == 1) {
            store.Delete(selected.Id);
            contacts.RemoveAll(contact => contact.Id == selected.Id);

            SetStatus("Contacto eliminado.");
            RefreshContacts();
        }
    }

    private void ImportFromJson() {
        string? path = AskFilePath(
            "Importar JSON",
            "Ruta del archivo a importar:",
            Path.Combine(AppContext.BaseDirectory, "agenda-tui.json"));

        if (string.IsNullOrWhiteSpace(path)) {
            return;
        }

        try {
            List<Contacto> imported = json.Read(path);

            int answer = MessageBox.Query(
                App!,
                "Importar",
                $"Se agregarán {imported.Count} contactos del archivo a la base. ¿Continuar?",
                "No",
                "Sí") ?? 0;

            if (answer != 1) {
                return;
            }

            foreach (Contacto contact in imported) {
                contact.Id = 0;
                int newId = store.Insert(contact);
                contact.Id = newId;
            }

            LoadContacts();
            SetStatus($"{imported.Count} contactos importados desde '{path}'.");
        } catch (Exception ex) {
            MessageBox.ErrorQuery(App!, "Importar", $"Error: {ex.Message}", "OK");
        }
    }

    private void ExportToJson() {
        string? path = AskFilePath(
            "Exportar JSON",
            "Ruta de salida:",
            Path.Combine(AppContext.BaseDirectory, "agenda-tui.json"));

        if (string.IsNullOrWhiteSpace(path)) {
            return;
        }

        try {
            json.Write(path, contacts);
            SetStatus($"Exportados {contacts.Count} contactos a '{path}'.");
        } catch (Exception ex) {
            MessageBox.ErrorQuery(App!, "Exportar", $"Error: {ex.Message}", "OK");
        }
    }

    private string? AskFilePath(string title, string prompt, string defaultPath) {
        Dialog dialog = new() {
            Title = title,
            Width = 60,
            Height = 8
        };

        Label label = new() {
            Text = prompt,
            X = 1,
            Y = 1
        };

        TextField input = new() {
            Text = defaultPath,
            X = 1,
            Y = Pos.Bottom(label) + 1,
            Width = Dim.Fill(2)
        };

        string? result = null;

        Button ok = new() {
            Text = "_Aceptar",
            IsDefault = true
        };
        ok.Accepting += (_, e) => {
            result = input.Text?.ToString();
            e.Handled = true;
            dialog.RequestStop();
        };

        Button cancel = new() {
            Text = "_Cancelar"
        };
        cancel.Accepting += (_, e) => {
            result = null;
            e.Handled = true;
            dialog.RequestStop();
        };

        dialog.AddButton(ok);
        dialog.AddButton(cancel);
        dialog.Add(label, input);

        App!.Run(dialog);
        return result;
    }

    private void RequestExit() {
        App!.RequestStop();
    }

    private void RefreshContacts(int? selectedId = null) {
        selectedId ??= contactsList.Value?.Id;
        string search = searchField.Text?.ToString()?.Trim() ?? "";
        bool onlyFavorites = favoritesOnly.Value == CheckState.Checked;

        filteredContacts = contacts
            .Where(contact => Matches(contact, search, onlyFavorites))
            .OrderByDescending(contact => contact.Favorito)
            .ThenBy(contact => contact.Nombre)
            .ToList();

        contactsList.SetSource(new ObservableCollection<Contacto>(filteredContacts));

        int index = filteredContacts.Count == 0
            ? -1
            : selectedId is null
                ? 0
                : Math.Max(filteredContacts.FindIndex(contact => contact.Id == selectedId.Value), 0);

        contactsList.Index = index;
        RefreshDetails();
        RefreshStatus();
    }

    private void RefreshDetails() {
        Contacto? selected = contactsList.Value;

        if (selected is null) {
            detailsLabel.Text = "Sin contacto seleccionado.";
            return;
        }

        detailsLabel.Text = $"""
            Nombre:   {selected.Nombre}
            Teléfono: {selected.Telefono}
            Email:    {selected.Email}
            Favorito: {(selected.Favorito ? "Sí" : "No")}

            Notas:
            {SinNotas(selected.Notas)}
            """;
    }

    private void RefreshStatus() {
        statusLabel.Text = $"{filteredContacts.Count} de {contacts.Count} contactos. F2 Nuevo | F3 Editar | Del Eliminar | Ctrl+I Importar | Ctrl+E Exportar | Ctrl+R Recargar | Ctrl+Q Salir";
    }

    private void SetStatus(string message) {
        RefreshStatus();
        statusLabel.Text = $"{message} {statusLabel.Text}";
    }

    private static bool Matches(Contacto contact, string search, bool onlyFavorites) {
        if (onlyFavorites && !contact.Favorito) {
            return false;
        }

        if (string.IsNullOrWhiteSpace(search)) {
            return true;
        }

        return Contains(contact.Nombre, search)
            || Contains(contact.Telefono, search)
            || Contains(contact.Email, search)
            || Contains(contact.Notas, search);
    }

    private static bool Contains(string value, string search) {
        return value.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    private static string SinNotas(string notas) {
        return string.IsNullOrWhiteSpace(notas) ? "Sin notas." : notas;
    }
}

public sealed class ContactDialog : Dialog {
    private readonly TextField nameField;
    private readonly TextField phoneField;
    private readonly TextField emailField;
    private readonly CheckBox favoriteField;
    private readonly TextView notesField;

    public ContactDialog(string title, Contacto contact) {
        Contact = contact;
        Title = title;
        Width = 72;
        Height = 18;

        Label nameLabel = new() {
            Text = "Nombre:",
            X = 2,
            Y = 1
        };
        nameField = new TextField {
            Text = contact.Nombre,
            X = 14,
            Y = Pos.Top(nameLabel),
            Width = Dim.Fill(2)
        };

        Label phoneLabel = new() {
            Text = "Teléfono:",
            X = Pos.Left(nameLabel),
            Y = Pos.Bottom(nameLabel) + 1
        };
        phoneField = new TextField {
            Text = contact.Telefono,
            X = Pos.Left(nameField),
            Y = Pos.Top(phoneLabel),
            Width = Dim.Fill(2)
        };

        Label emailLabel = new() {
            Text = "Email:",
            X = Pos.Left(nameLabel),
            Y = Pos.Bottom(phoneLabel) + 1
        };
        emailField = new TextField {
            Text = contact.Email,
            X = Pos.Left(nameField),
            Y = Pos.Top(emailLabel),
            Width = Dim.Fill(2)
        };

        favoriteField = new CheckBox {
            Text = "_Favorito",
            X = Pos.Left(nameField),
            Y = Pos.Bottom(emailLabel) + 1,
            Value = contact.Favorito ? CheckState.Checked : CheckState.UnChecked
        };

        Label notesLabel = new() {
            Text = "Notas:",
            X = Pos.Left(nameLabel),
            Y = Pos.Bottom(favoriteField) + 1
        };
        notesField = new TextView {
            Text = contact.Notas,
            X = Pos.Left(nameField),
            Y = Pos.Top(notesLabel),
            Width = Dim.Fill(2),
            Height = 4
        };

        Button saveButton = new() {
            Text = "Guardar",
            IsDefault = true,
            X = 18,
            Y = Pos.Bottom(notesField) + 1
        };
        saveButton.Accepting += (_, e) => {
            Save();
            e.Handled = true;
        };

        Button cancelButton = new() {
            Text = "Cancelar",
            X = Pos.Right(saveButton) + 2,
            Y = Pos.Top(saveButton)
        };
        cancelButton.Accepting += (_, e) => {
            App!.RequestStop();
            e.Handled = true;
        };

        Add(
            nameLabel, nameField,
            phoneLabel, phoneField,
            emailLabel, emailField,
            favoriteField,
            notesLabel, notesField
        );
        AddButton(saveButton);
        AddButton(cancelButton);

        nameField.SetFocus();
    }

    public bool Saved { get; private set; }
    public Contacto? Contact { get; private set; }

    private void Save() {
        string name = Clean(nameField.Text);

        if (string.IsNullOrWhiteSpace(name)) {
            MessageBox.Query(App!, "Validación", "El nombre es obligatorio.", "OK");
            nameField.SetFocus();
            return;
        }

        Contact!.Nombre = name;
        Contact.Telefono = Clean(phoneField.Text);
        Contact.Email = Clean(emailField.Text);
        Contact.Notas = Clean(notesField.Text);
        Contact.Favorito = favoriteField.Value == CheckState.Checked;

        Saved = true;
        App!.RequestStop();
    }

    private static string Clean(object? value) {
        return value?.ToString()?.Trim() ?? "";
    }
}

public sealed class SqliteAgendaStore {
    private readonly string connectionString;

    public SqliteAgendaStore(string dbPath) {
        connectionString = $"Data Source={dbPath}";
        EnsureSchema();
    }

    private SqliteConnection Open() {
        SqliteConnection connection = new(connectionString);
        connection.Open();
        return connection;
    }

    private void EnsureSchema() {
        using SqliteConnection db = Open();
        db.Execute("""
            CREATE TABLE IF NOT EXISTS Contactos (
                Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre   TEXT    NOT NULL,
                Telefono TEXT    NOT NULL DEFAULT '',
                Email    TEXT    NOT NULL DEFAULT '',
                Notas    TEXT    NOT NULL DEFAULT '',
                Favorito INTEGER NOT NULL DEFAULT 0
            );
        """);
    }

    public List<Contacto> GetAll() {
        using SqliteConnection db = Open();
        return db.GetAll<Contacto>().ToList();
    }

    public int Insert(Contacto contact) {
        using SqliteConnection db = Open();
        return (int)db.Insert(contact);
    }

    public bool Update(Contacto contact) {
        using SqliteConnection db = Open();
        return db.Update(contact);
    }

    public bool Delete(int id) {
        using SqliteConnection db = Open();
        return db.Delete(new Contacto { Id = id });
    }

    public void DeleteAll() {
        using SqliteConnection db = Open();
        db.Execute("DELETE FROM Contactos;");
    }
}

public sealed class JsonAgendaIO {
    private static readonly JsonSerializerOptions JsonOptions = new() {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public List<Contacto> Read(string path) {
        if (!File.Exists(path)) {
            throw new FileNotFoundException($"Archivo no encontrado: '{path}'");
        }

        string json = File.ReadAllText(path, Encoding.UTF8);
        return JsonSerializer.Deserialize<List<Contacto>>(json, JsonOptions) ?? new();
    }

    public void Write(string path, IEnumerable<Contacto> contacts) {
        string json = JsonSerializer.Serialize(
            contacts.OrderBy(contact => contact.Id),
            JsonOptions);

        File.WriteAllText(path, json, Encoding.UTF8);
    }
}

[Table("Contactos")]
public sealed class Contacto {
    [Key] public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Email { get; set; } = "";
    public string Notas { get; set; } = "";
    public bool Favorito { get; set; }

    public Contacto Clone() {
        return new Contacto {
            Id = Id,
            Nombre = Nombre,
            Telefono = Telefono,
            Email = Email,
            Notas = Notas,
            Favorito = Favorito
        };
    }

    public override string ToString() {
        string favorite = Favorito ? "*" : " ";
        return $"{favorite} {Cut(Nombre, 22),-22} {Cut(Telefono, 18),-18} {Cut(Email, 28),-28}";
    }

    private static string Cut(string value, int width) {
        if (string.IsNullOrEmpty(value) || value.Length <= width) {
            return value;
        }

        if (width <= 3) {
            return value[..width];
        }

        return value[..(width - 3)] + "...";
    }
}