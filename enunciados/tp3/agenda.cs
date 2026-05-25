#!/usr/bin/env dotnet
#:property PublishAot=false
#:package Terminal.Gui@2.0.1
#:package Microsoft.Data.Sqlite@*
#:package Dapper@*
#:package Dapper.Contrib@*

using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using Microsoft.Data.Sqlite;

using Dapper;
using Dapper.Contrib.Extensions;

using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Data;

string dbPath = args.Length > 0 ? args[0] : "agenda.db";

SqliteAgendaStore store;

try {
    store = new SqliteAgendaStore(dbPath);
}
catch (Exception ex) {
    Console.WriteLine($"Error al abrir la base: {ex.Message}");
    return;
}

using IApplication app = Application.Create().Init();

AgendaWindow window = new(store);

app.Run(window);





public sealed class AgendaWindow : Window {

    private readonly SqliteAgendaStore store;

    private List<Contacto> contacts = [];
    private List<Contacto> filteredContacts = [];

    private readonly ListView contactList;
    private readonly TextField searchField;
    private readonly TextView detailView;
    private readonly StatusBar statusBar;

    private bool onlyFavorites = false;

    public AgendaWindow(SqliteAgendaStore store) {

        this.store = store;

        Title  = "AgendaT - Terminal.Gui";
        Width  = Dim.Fill();
        Height = Dim.Fill();

        MenuBar menu = new() {
            Menus = [

                new MenuBarItem("_Archivo", [

                    new MenuItem("_Importar JSON", "", ImportJson),
                    new MenuItem("_Exportar JSON", "", ExportJson),
                    null!,
                    new MenuItem("_Salir", "Ctrl+Q", Salir)

                ]),

                new MenuBarItem("_Contactos", [

                    new MenuItem("_Nuevo", "F2", NuevoContacto),
                    new MenuItem("_Editar", "F3", EditarContacto),
                    new MenuItem("_Eliminar", "Del", EliminarContacto)

                ]),

                new MenuBarItem("_Ver", [

                    new MenuItem("_Solo favoritos", "", ToggleFavoritos)

                ]),

                new MenuBarItem("_Ayuda", [

                    new MenuItem("_Acerca de", "", AcercaDe)

                ])
            ]
        };

        Add(menu);

        Label searchLabel = new() {
            Text = "Buscar:",
            X = 1,
            Y = 1
        };

        searchField = new TextField("") {
            X = 10,
            Y = 1,
            Width = Dim.Fill(2)
        };

        searchField.TextChanged += (_) => ApplyFilters();

        FrameView listFrame = new() {
            Title = "Contactos",
            X = 0,
            Y = 3,
            Width = Dim.Percent(40),
            Height = Dim.Fill(1)
        };

        contactList = new ListView() {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        contactList.SelectedItemChanged += (_) => UpdateDetails();

        contactList.OpenSelectedItem += (_) => EditarContacto();

        listFrame.Add(contactList);

        FrameView detailFrame = new() {
            Title = "Detalle",
            X = Pos.Right(listFrame),
            Y = 3,
            Width = Dim.Fill(),
            Height = Dim.Fill(1)
        };

        detailView = new TextView() {
            ReadOnly = true,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        detailFrame.Add(detailView);

        statusBar = new StatusBar([
            new Shortcut(Key.F2, "Nuevo", NuevoContacto),
            new Shortcut(Key.F3, "Editar", EditarContacto),
            new Shortcut(Key.DeleteChar, "Eliminar", EliminarContacto),
            new Shortcut(Key.CtrlMask | Key.I, "Importar", ImportJson),
            new Shortcut(Key.CtrlMask | Key.E, "Exportar", ExportJson),
            new Shortcut(Key.F4, "Buscar", FocusSearch),
            new Shortcut(Key.CtrlMask | Key.Q, "Salir", Salir)
        ]);

        Add(searchLabel);
        Add(searchField);
        Add(listFrame);
        Add(detailFrame);
        Add(statusBar);

        LoadContacts();
    }

    protected override bool OnKeyDown(Key key) {

        if (key == (Key.CtrlMask | Key.N) || key == Key.F2) {
            NuevoContacto();
            return true;
        }

        if (key == Key.F3 || key == Key.Enter) {
            EditarContacto();
            return true;
        }

        if (key == (Key.CtrlMask | Key.D) || key == Key.DeleteChar) {
            EliminarContacto();
            return true;
        }

        if (key == (Key.CtrlMask | Key.I)) {
            ImportJson();
            return true;
        }

        if (key == (Key.CtrlMask | Key.E)) {
            ExportJson();
            return true;
        }

        if (key == Key.F4) {
            FocusSearch();
            return true;
        }

        if (key == (Key.CtrlMask | Key.Q)) {
            Salir();
            return true;
        }

        return base.OnKeyDown(key);
    }

    private void LoadContacts() {
        contacts = store.GetAll();
        ApplyFilters();
    }

    private void ApplyFilters() {

        string search = searchField.Text?.ToString()?.ToLower() ?? "";

        filteredContacts = contacts
            .Where(c =>
                (!onlyFavorites || c.Favorito)
                &&
                (
                    c.Nombre.ToLower().Contains(search)
                    ||
                    c.Telefonos.ToLower().Contains(search)
                    ||
                    c.Email.ToLower().Contains(search)
                )
            )
            .ToList();

        List<string> items = filteredContacts
            .Select(c => $"{(c.Favorito ? "★" : " ")} {c.Nombre}")
            .ToList();

        contactList.SetSource(items);

        UpdateDetails();
    }

    private Contacto? SelectedContact() {

        if (filteredContacts.Count == 0)
            return null;

        int index = contactList.SelectedItem;

        if (index < 0 || index >= filteredContacts.Count)
            return null;

        return filteredContacts[index];
    }

    private void UpdateDetails() {

        Contacto? c = SelectedContact();

        if (c == null) {
            detailView.Text = "";
            return;
        }

        detailView.Text =
$"""
Nombre: {c.Nombre}

Teléfonos:
{c.Telefonos}

Email:
{c.Email}

Favorito:
{(c.Favorito ? "Sí" : "No")}

Notas:
{c.Notas}
""";
    }

    private void NuevoContacto() {

        ContactDialog dialog = new();

        Application.Run(dialog);

        if (!dialog.Accepted)
            return;

        try {

            store.Insert(dialog.Contacto!);

            contacts.Add(dialog.Contacto!);

            ApplyFilters();

            MessageBox.Query("OK", "Contacto creado correctamente.", "Aceptar");
        }
        catch (Exception ex) {
            MessageBox.ErrorQuery("Error", ex.Message, "Aceptar");
        }
    }

    private void EditarContacto() {

        Contacto? selected = SelectedContact();

        if (selected == null)
            return;

        ContactDialog dialog = new(selected.Clone());

        Application.Run(dialog);

        if (!dialog.Accepted)
            return;

        try {

            store.Update(dialog.Contacto!);

            int idx = contacts.FindIndex(c => c.Id == dialog.Contacto!.Id);

            if (idx >= 0)
                contacts[idx] = dialog.Contacto!;

            ApplyFilters();

            MessageBox.Query("OK", "Contacto actualizado.", "Aceptar");
        }
        catch (Exception ex) {
            MessageBox.ErrorQuery("Error", ex.Message, "Aceptar");
        }
    }

    private void EliminarContacto() {

        Contacto? selected = SelectedContact();

        if (selected == null)
            return;

        int result = MessageBox.Query(
            "Confirmar",
            $"¿Eliminar a {selected.Nombre}?",
            "Sí",
            "No"
        );

        if (result != 0)
            return;

        try {

            store.Delete(selected.Id);

            contacts.RemoveAll(c => c.Id == selected.Id);

            ApplyFilters();

            MessageBox.Query("OK", "Contacto eliminado.", "Aceptar");
        }
        catch (Exception ex) {
            MessageBox.ErrorQuery("Error", ex.Message, "Aceptar");
        }
    }

    private void ImportJson() {

        string path = AskPath("Importar JSON");

        if (string.IsNullOrWhiteSpace(path))
            return;

        try {

            List<Contacto> imported = JsonAgendaIO.Import(path);

            int confirm = MessageBox.Query(
                "Confirmar",
                $"Se importarán {imported.Count} contactos.",
                "Importar",
                "Cancelar"
            );

            if (confirm != 0)
                return;

            foreach (Contacto c in imported) {

                c.Id = 0;

                store.Insert(c);

                contacts.Add(c);
            }

            ApplyFilters();

            MessageBox.Query("OK", "Importación completada.", "Aceptar");
        }
        catch (Exception ex) {
            MessageBox.ErrorQuery("Error", ex.Message, "Aceptar");
        }
    }

    private void ExportJson() {

        string path = AskPath("Exportar JSON");

        if (string.IsNullOrWhiteSpace(path))
            return;

        try {

            JsonAgendaIO.Export(path, contacts);

            MessageBox.Query("OK", "Exportación completada.", "Aceptar");
        }
        catch (Exception ex) {
            MessageBox.ErrorQuery("Error", ex.Message, "Aceptar");
        }
    }

    private string AskPath(string title) {

        string result = "";

        Dialog dialog = new() {
            Title = title,
            Width = 60,
            Height = 10
        };

        TextField field = new() {
            X = 1,
            Y = 1,
            Width = Dim.Fill(2)
        };

        Button ok = new() {
            Text = "OK",
            X = Pos.Center() - 10,
            Y = 3
        };

        Button cancel = new() {
            Text = "Cancelar",
            X = Pos.Center() + 2,
            Y = 3
        };

        ok.Accepting += (_, e) => {
            result = field.Text?.ToString() ?? "";
            Application.RequestStop();
            e.Handled = true;
        };

        cancel.Accepting += (_, e) => {
            result = "";
            Application.RequestStop();
            e.Handled = true;
        };

        dialog.Add(field);
        dialog.AddButton(ok);
        dialog.AddButton(cancel);

        Application.Run(dialog);

        return result;
    }

    private void ToggleFavoritos() {
        onlyFavorites = !onlyFavorites;
        ApplyFilters();
    }

    private void FocusSearch() {
        searchField.SetFocus();
    }

    private void AcercaDe() {

        MessageBox.Query(
            "Acerca de",
            "AgendaT\nTP3 - Programación\nTerminal.Gui + SQLite + JSON",
            "Aceptar"
        );
    }

    private void Salir() {
        Application.RequestStop();
    }
}



public sealed class ContactDialog : Dialog {

    public Contacto? Contacto { get; private set; }

    public bool Accepted { get; private set; }

    private readonly TextField nombreField;
    private readonly TextField emailField;

    private readonly TextField[] telefonos;

    private readonly CheckBox favoritoCheck;

    private readonly TextView notasField;

    public ContactDialog(Contacto? contacto = null) {

        contacto ??= new Contacto();

        Title = contacto.Id == 0 ? "Nuevo Contacto" : "Editar Contacto";

        Width = 70;
        Height = 24;

        Add(new Label("Nombre:") { X = 1, Y = 1 });

        nombreField = new(contacto.Nombre) {
            X = 15,
            Y = 1,
            Width = 40
        };

        Add(nombreField);

        telefonos = new TextField[5];

        for (int i = 0; i < 5; i++) {

            Add(new Label($"Tel {i + 1}:") {
                X = 1,
                Y = 3 + i
            });

            string tel = "";

            string[] arr = contacto.Telefonos.Split(",");

            if (i < arr.Length)
                tel = arr[i].Trim();

            telefonos[i] = new TextField(tel) {
                X = 15,
                Y = 3 + i,
                Width = 30
            };

            Add(telefonos[i]);
        }

        Add(new Label("Email:") {
            X = 1,
            Y = 9
        });

        emailField = new(contacto.Email) {
            X = 15,
            Y = 9,
            Width = 40
        };

        Add(emailField);

        favoritoCheck = new() {
            Text = "Favorito",
            X = 15,
            Y = 11,
            CheckedState = contacto.Favorito
                ? CheckState.Checked
                : CheckState.UnChecked
        };

        Add(favoritoCheck);

        Add(new Label("Notas:") {
            X = 1,
            Y = 13
        });

        notasField = new() {
            X = 15,
            Y = 13,
            Width = 40,
            Height = 4,
            Text = contacto.Notas
        };

        Add(notasField);

        Button save = new() {
            Text = "Guardar"
        };

        Button cancel = new() {
            Text = "Cancelar"
        };

        save.Accepting += (_, e) => {

            string nombre = nombreField.Text?.ToString()?.Trim() ?? "";

            string email = emailField.Text?.ToString()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(nombre)) {

                MessageBox.ErrorQuery(
                    "Error",
                    "El nombre no puede estar vacío.",
                    "Aceptar"
                );

                return;
            }

            if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@")) {

                MessageBox.ErrorQuery(
                    "Error",
                    "El email debe contener @",
                    "Aceptar"
                );

                return;
            }

            string tels = string.Join(
                ", ",
                telefonos
                    .Select(t => t.Text?.ToString()?.Trim() ?? "")
                    .Where(t => !string.IsNullOrWhiteSpace(t))
            );

            Contacto = new Contacto {
                Id = contacto.Id,
                Nombre = nombre,
                Telefonos = tels,
                Email = email,
                Notas = notasField.Text?.ToString() ?? "",
                Favorito = favoritoCheck.CheckedState == CheckState.Checked
            };

            Accepted = true;

            Application.RequestStop();

            e.Handled = true;
        };

        cancel.Accepting += (_, e) => {

            Accepted = false;

            Application.RequestStop();

            e.Handled = true;
        };

        AddButton(save);
        AddButton(cancel);
    }
}






public sealed class SqliteAgendaStore {

    private readonly string connectionString;

    public SqliteAgendaStore(string path) {

        connectionString = $"Data Source={path}";

        using SqliteConnection connection = new(connectionString);

        connection.Open();

        string sql =
"""
CREATE TABLE IF NOT EXISTS Contactos(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    Telefonos TEXT,
    Email TEXT,
    Notas TEXT,
    Favorito INTEGER NOT NULL DEFAULT 0
);
""";

        connection.Execute(sql);
    }

    private SqliteConnection Connection() {
        return new SqliteConnection(connectionString);
    }

    public List<Contacto> GetAll() {

        using SqliteConnection db = Connection();

        return db.GetAll<Contacto>().ToList();
    }

    public void Insert(Contacto contacto) {

        using SqliteConnection db = Connection();

        long id = db.Insert(contacto);

        contacto.Id = (int)id;
    }

    public void Update(Contacto contacto) {

        using SqliteConnection db = Connection();

        db.Update(contacto);
    }

    public void Delete(int id) {

        using SqliteConnection db = Connection();

        Contacto? c = db.Get<Contacto>(id);

        if (c != null)
            db.Delete(c);
    }
}




public static class JsonAgendaIO {

    public static List<Contacto> Import(string path) {

        if (!File.Exists(path))
            throw new Exception("El archivo JSON no existe.");

        string json = File.ReadAllText(path);

        List<Contacto>? data =
            JsonSerializer.Deserialize<List<Contacto>>(json);

        return data ?? [];
    }

    public static void Export(string path, List<Contacto> contactos) {

        JsonSerializerOptions options = new() {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        string json = JsonSerializer.Serialize(contactos, options);

        File.WriteAllText(path, json);
    }
}




[Table("Contactos")]
public sealed class Contacto {

    [Key]
    public int Id { get; set; }

    public string Nombre { get; set; } = "";

    public string Telefonos { get; set; } = "";

    public string Email { get; set; } = "";

    public string Notas { get; set; } = "";

    public bool Favorito { get; set; }

    public Contacto Clone() {

        return new Contacto {
            Id = Id,
            Nombre = Nombre,
            Telefonos = Telefonos,
            Email = Email,
            Notas = Notas,
            Favorito = Favorito
        };
    }
}