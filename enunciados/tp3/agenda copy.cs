#:package Terminal.Gui@2.0.1-develop.1
#:package Microsoft.Data.Sqlite@10.0.0

using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

var usaArchivoPorDefecto = args.Length == 0;
var archivoPath = !usaArchivoPorDefecto ? Path.GetFullPath(args[0]) : Path.Combine(SourceDirectory(), "agenda.json");

IUnitOfWork<Contacto> unidadDeTrabajo = CrearUnidadDeTrabajo(archivoPath);

// Interfaz de usuario

using IApplication app = Application.Create();
app.Init();

using var root = new Runnable {
    Width  = Dim.Fill(),
    Height = Dim.Fill()
};

var vista = new List<Contacto>();
var indiceSeleccionado = -1;
TextField txtBuscar = null!;
ListView lista = null!;
const int AnchoNombre    = 30;
const int AnchoDomicilio = 30;

Menu.DefaultBorderStyle = LineStyle.Single;

var ventana = new Window {
    X = 0, Y = 1,
    Width  = Dim.Fill(),
    Height = Dim.Fill(1),
    Title = "Agenda",
    BorderStyle = LineStyle.Single
};

var menu = new MenuBar {
    X = 0, Y = 0,
    Width = Dim.Fill(),
    Menus = new[] {
        new MenuBarItem("_Agenda", new MenuItem[] {
            new MenuItem("_Guardar", Key.S.WithCtrl, () => _ = GuardarCambios()),
            new MenuItem("_Salir",   Key.Q.WithCtrl, Salir),
        }),
        new MenuBarItem("_Contacto", new MenuItem[] {
            new MenuItem("_Agregar", Key.F2,     Agregar),
            new MenuItem("_Editar",  Key.Enter,  EditarSeleccionado),
            new MenuItem("_Borrar",  Key.Delete, BorrarSeleccionado),
        }),
    }
};

var lblBuscar = new Label { Text = "Buscar:", X = 2, Y = 1 };
txtBuscar = new TextField {
    X = Pos.Right(lblBuscar) + 1, Y = 1,
    Width = Dim.Fill(3)
};

var lblColumnas = new Label {
    Text = EncabezadoLista(),
    X = 2, Y = 3,
    Width = Dim.Fill(3)
};

lista = new ListView {
    X = 2, Y = 4,
    Width  = Dim.Fill(3),
    Height = Dim.Fill(3),
    CanFocus = true
};

ventana.Add(lblBuscar, txtBuscar, lblColumnas, lista);
root.Add(menu, ventana);

txtBuscar.TextChanged += (_, _) => Refrescar();

txtBuscar.KeyDown += (_, key) => {
    if (key == Key.Enter) {
        EditarSeleccionado();
        key.Handled = true;
    }
};

lista.ValueChanged += (_, _) => {
    var indice = lista.SelectedItem;
    indiceSeleccionado = indice is >= 0 && indice < vista.Count ? indice.Value : -1;
};

lista.KeyDown += (_, key) => {
    if (key == Key.Enter) {
        EditarSeleccionado();
        key.Handled = true;
    } else if (key == Key.Delete || key == Key.Backspace) {
        BorrarSeleccionado();
        key.Handled = true;
    }
};

Refrescar();
lista.SetFocus();
app.Run(root);

static string SourceDirectory([CallerFilePath] string sourcePath = "") {
    return Path.GetDirectoryName(sourcePath) ?? Directory.GetCurrentDirectory();
}

bool GuardarCambios() {
    try {
        unidadDeTrabajo.SaveChanges();
        ActualizarTitulo();
        return true;
    } catch (Exception ex) {
        MostrarMensaje("No se pudo guardar", ex.Message, "OK");
        return false;
    }
}

void ActualizarTitulo() {
    var prefijoPendiente = unidadDeTrabajo.HasPendingChanges ? "* " : "";
    var nombreArchivo = Path.GetFileName(archivoPath) ?? archivoPath;
    ventana.Title = $"{prefijoPendiente}Agenda - {nombreArchivo}";
    ventana.SetNeedsDraw();
}

static IUnitOfWork<Contacto> CrearUnidadDeTrabajo(string path) {
    return Path.GetExtension(path).ToLowerInvariant() switch {
        ".json" => new UnitOfWork<Contacto>(new JsonContactoStore(path)),
        ".db"   => new UnitOfWork<Contacto>(new SqliteContactoStore(path)),
        _       => throw new ArgumentException("La agenda debe usar un archivo .json o .db.", nameof(path)),
    };
}

bool ConfirmarSalidaConPendientes() {
    if (!unidadDeTrabajo.HasPendingChanges) {
        return true;
    }

    var respuesta = MostrarMensaje(
        "Salir",
        "Hay cambios sin guardar.",
        "Guardar y salir",
        "Salir sin guardar",
        "Cancelar");

    return respuesta switch {
        0 => GuardarCambios(),
        1 => true,
        _ => false,
    };
}

void Salir() {
    if (!ConfirmarSalidaConPendientes()) { return; }

    app.RequestStop(root);
}

void Refrescar(int? seleccionarId = null) {
    var filtro = Texto(txtBuscar);

    vista = unidadDeTrabajo.ReadAll()
        .Where(c => string.IsNullOrWhiteSpace(filtro) || Coincide(c, filtro))
        .OrderBy(c => c.Apellido)
        .ThenBy(c => c.Nombre)
        .ToList();

    lista.SetSource<string>([.. vista.Select(FormatearContacto)]);

    if (vista.Count == 0) {
        indiceSeleccionado = -1;
    } else {
        var indice = seleccionarId is null ? 0 : vista.FindIndex(contacto => contacto.Id == seleccionarId.Value);

        if (indice < 0) { indice = 0; }

        lista.SelectedItem = indice;
        indiceSeleccionado = indice;
        lista.EnsureSelectedItemVisible();
    }

    ActualizarTitulo();
    lista.SetNeedsDraw();
}

void Agregar() {
    var nuevo = EditarContacto(null);

    if (nuevo is null) { return; }

    unidadDeTrabajo.Create(nuevo);
    txtBuscar.Text = "";
    Refrescar(nuevo.Id);
    lista.SetFocus();
}

void EditarSeleccionado() {
    var contacto = ContactoSeleccionado();

    if (contacto is null) { return; }

    var editado = EditarContacto(contacto);

    if (editado is null) { return; }

    editado.Id = contacto.Id;
    unidadDeTrabajo.Update(editado);
    Refrescar(contacto.Id);
    lista.SetFocus();
}

void BorrarSeleccionado() {
    var contacto = ContactoSeleccionado();

    if (contacto is null) { return; }

    var respuesta = MostrarMensaje(
        "Borrar contacto",
        $"¿Borrar a {NombreVisible(contacto)}?",
        "Sí",
        "No");

    if (respuesta != 0) { return; }

    unidadDeTrabajo.Remove(contacto.Id);
    Refrescar();
    lista.SetFocus();
}

Contacto? ContactoSeleccionado() {
    return indiceSeleccionado >= 0 && indiceSeleccionado < vista.Count
        ? vista[indiceSeleccionado]
        : null;
}

Contacto? EditarContacto(Contacto? original) {
    Contacto? resultado = null;
    var telefonosOriginales = (original?.Telefonos ?? "").Split(',').Select(t => t.Trim()).ToList();

    var dialogo = new Dialog {
        Title = original is null ? "Agregar contacto" : "Editar contacto",
        Width  = 60,
        Height = 18
    };

    var txtNombre    = new TextField { Text = original?.Nombre    ?? "", X = 14, Y = 1, Width = 40 };
    var txtApellido  = new TextField { Text = original?.Apellido  ?? "", X = 14, Y = 3, Width = 40 };
    var txtDomicilio = new TextField { Text = original?.Domicilio ?? "", X = 14, Y = 5, Width = 40 };
    var txtTelefono1 = new TextField { Text = telefonosOriginales.ElementAtOrDefault(0) ?? "", X = 14, Y =  7, Width = 20 };
    var txtTelefono2 = new TextField { Text = telefonosOriginales.ElementAtOrDefault(1) ?? "", X = 14, Y =  8, Width = 20 };
    var txtTelefono3 = new TextField { Text = telefonosOriginales.ElementAtOrDefault(2) ?? "", X = 14, Y =  9, Width = 20 };
    var txtTelefono4 = new TextField { Text = telefonosOriginales.ElementAtOrDefault(3) ?? "", X = 14, Y = 10, Width = 20 };
    var telefonos    = new[] { txtTelefono1, txtTelefono2, txtTelefono3, txtTelefono4 };

    var error        = new Label { X = 2, Y = 15, Width = Dim.Fill(2), Text = "" };

    var btnAceptar  = new Button { Text = "Aceptar", IsDefault = true };
    var btnCancelar = new Button { Text = "Cancelar" };

    btnAceptar.Accepting += (_, e) => {
        var nombre = Texto(txtNombre);
        var apellido = Texto(txtApellido);

        if (string.IsNullOrWhiteSpace(nombre) && string.IsNullOrWhiteSpace(apellido)) {
            error.Text = "Ingresá al menos nombre o apellido.";
            error.SetNeedsDraw();
            e.Handled = true;
            return;
        }

        resultado = new Contacto {
            Nombre = nombre,
            Apellido = apellido,
            Domicilio = Texto(txtDomicilio),
            Telefonos = string.Join(", ", telefonos
                .Select(Texto)
                .Where(texto => !string.IsNullOrWhiteSpace(texto))
                .Take(4)
                )
        };

        app.RequestStop(dialogo);
        e.Handled = true;
    };

    btnCancelar.Accepting += (_, e) => {
        app.RequestStop(dialogo);
        e.Handled = true;
    };

    dialogo.Add(
        new Label { Text = "Nombre:",     X = 2, Y = 1 },
        txtNombre,
        new Label { Text = "Apellido:",   X = 2, Y = 3 },
        txtApellido,
        new Label { Text = "Domicilio:",  X = 2, Y = 5 },
        txtDomicilio,
        new Label { Text = "Teléfono 1:", X = 2, Y = 7 },
        txtTelefono1,
        // new Label { Text = "Teléfono 2:", X = 2, Y = 9 },
        txtTelefono2,
        // new Label { Text = "Teléfono 3:", X = 2, Y = 11 },
        txtTelefono3,
        // new Label { Text = "Teléfono 4:", X = 2, Y = 13 },
        txtTelefono4,
        new Label { Text = "Podés cargar hasta 4 teléfonos", X = 14, Y = 14 },
        error
    );

    dialogo.AddButton(btnCancelar);
    dialogo.AddButton(btnAceptar);

    app.Run(dialogo);
    dialogo.Dispose();
    return resultado;
}

int MostrarMensaje(string title, string message, params string[] buttons) {
    return MessageBox.Query(app, title, message, buttons) ?? -1;
}

string FormatearContacto(Contacto contacto) {
    var nombre    = NombreVisible(contacto);
    var domicilio = string.IsNullOrWhiteSpace(contacto.Domicilio) ? "(sin domicilio)" : contacto.Domicilio;
    var telefonos = string.IsNullOrWhiteSpace(contacto.Telefonos) ? "(sin teléfonos)" : contacto.Telefonos;
    return $"{Columna(nombre, AnchoNombre)}  {Columna(domicilio, AnchoDomicilio)}  {UnaLinea(telefonos)}";
}

string EncabezadoLista() {
    return $"{Columna("Contacto", AnchoNombre)}  {Columna("Domicilio", AnchoDomicilio)}  Teléfonos";
}

string Columna(string texto, int ancho) {
    texto = UnaLinea(texto);

    if (texto.Length > ancho) {
        return texto[..Math.Max(0, ancho - 3)] + "...";
    }

    return texto.PadRight(ancho);
}

string UnaLinea(string texto) {
    return texto.Replace('\r', ' ').Replace('\n', ' ').Trim();
}

string NombreVisible(Contacto contacto) {
    var nombre = $"{contacto.Apellido}, {contacto.Nombre}".Trim(' ', ',');
    return string.IsNullOrWhiteSpace(nombre) ? "(sin nombre)" : nombre;
}

bool Coincide(Contacto contacto, string filtro) {
    return Contiene(contacto.Nombre, filtro)
        || Contiene(contacto.Apellido, filtro)
        || Contiene(contacto.Domicilio, filtro)
    || Contiene(contacto.Telefonos, filtro);
}

bool Contiene(string? texto, string filtro) {
    return texto?.Contains(filtro, StringComparison.OrdinalIgnoreCase) == true;
}

string Texto(TextField field) {
    return field.Text?.ToString()?.Trim() ?? "";
}

// === Modelos, repositorios y unidad de trabajo ===

public interface IEntity {
    int Id { get; set; }
}

public sealed class Contacto : IEntity {
    public int Id           { get; set; }
    public string Nombre    { get; set; } = "";
    public string Apellido  { get; set; } = "";
    public string Domicilio { get; set; } = "";
    public string Telefonos { get; set; } = "";
}

public interface IRepository<T> where T : class, IEntity {
    IReadOnlyList<T> ReadAll();
    T? ReadOne(int id);
    void Create(T item);
    void Update(T item);
    void Remove(int id);
}

public interface IUnitOfWork<T> : IRepository<T> where T : class, IEntity {
    bool HasPendingChanges { get; }
    void SaveChanges();
}

public interface IStore<T> where T : class, IEntity {
    IReadOnlyList<T> Load();
    void SaveChanges(IReadOnlyCollection<StoreChange<T>> changes);
}

public enum StoreChangeKind { Insert, Update, Delete, }
public sealed record StoreChange<T>(StoreChangeKind Kind, int Id, T? Entity = null) where T : class, IEntity;

public enum EntityState { Unchanged, Added, Modified, Deleted, }
public sealed record TrackedEntry<T>(T Entity, EntityState State) where T : IEntity {}


public sealed class UnitOfWork<T> : IUnitOfWork<T> where T : class, IEntity {
    readonly IStore<T> store;
    readonly Dictionary<int, TrackedEntry<T>> tracked;

    public UnitOfWork(IStore<T> store) {
        this.store = store;
        tracked    = store.Load().ToDictionary( 
                        item => item.Id,
                        item => new TrackedEntry<T>(item, EntityState.Unchanged));
    }

    public bool HasPendingChanges => tracked.Values.Any(entry => entry.State != EntityState.Unchanged);

    public IReadOnlyList<T> ReadAll() {
        return tracked.Values
            .Where(entry  => entry.State != EntityState.Deleted)
            .Select(entry => entry.Entity)
            .ToList();
    }

    public T? ReadOne(int id) {
        return tracked.TryGetValue(id, out var entry) && entry.State != EntityState.Deleted
            ? entry.Entity
            : null;
    }

    public void Create(T item) {
        item.Id = tracked.Keys.DefaultIfEmpty(0).Max() + 1;
        tracked[item.Id] = new TrackedEntry<T>(item, EntityState.Added);
    }

    public void Update(T item) {
        var estado = EntityState.Modified;
        if (tracked.ContainsKey(item.Id) && tracked[item.Id].State == EntityState.Added) {
            estado = EntityState.Added;
        }
        tracked[item.Id] = new TrackedEntry<T>(item, estado);
    }

    public void Remove(int id) {
        if (!tracked.TryGetValue(id, out var entry)) {
            return;
        }

        if (entry.State == EntityState.Added) {
            tracked.Remove(id);
            return;
        }

        tracked[id] = entry with { State = EntityState.Deleted };
    }

    public void SaveChanges() {
        if (!HasPendingChanges) { return; }

        var changes = tracked.Values
            .Where(entry => entry.State != EntityState.Unchanged)
            .Select(ToStoreChange)
            .ToList();

        store.SaveChanges(changes);
        AcceptChanges();
    }

    private static StoreChange<T> ToStoreChange(TrackedEntry<T> entry) {
        return entry.State switch {
            EntityState.Added    => new StoreChange<T>(StoreChangeKind.Insert, entry.Entity.Id, entry.Entity),
            EntityState.Modified => new StoreChange<T>(StoreChangeKind.Update, entry.Entity.Id, entry.Entity),
            EntityState.Deleted  => new StoreChange<T>(StoreChangeKind.Delete, entry.Entity.Id),
            _ => throw new InvalidOperationException("No hay cambios para persistir."),
        };
    }

    private void AcceptChanges() {
        var ids = tracked.Keys.ToList();
        foreach (var id in ids) {
            var item = tracked[id];
            if (item.State == EntityState.Deleted) {
                tracked.Remove(id);
            } else {
                tracked[id] = item with { State = EntityState.Unchanged };
            }
        }
    }
}


public sealed class JsonContactoStore : IStore<Contacto> {
    private readonly string path;

    public JsonContactoStore(string path) {
        this.path = path;
    }

    public IReadOnlyList<Contacto> Load() {
        return LoadContactos();
    }

    public void SaveChanges(IReadOnlyCollection<StoreChange<Contacto>> changes) {
        var directorio = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directorio)) {
            Directory.CreateDirectory(directorio);
        }

        var contactos = LoadContactos().ToDictionary(contacto => contacto.Id);

        foreach (var change in changes) {
            switch (change.Kind) {
                case StoreChangeKind.Insert:
                case StoreChangeKind.Update:
                    contactos[change.Id] = change.Entity!;
                    break;
                case StoreChangeKind.Delete:
                    contactos.Remove(change.Id);
                    break;
            }
        }

        var vivos = contactos.Values
            .OrderBy(contacto => contacto.Apellido)
            .ThenBy(contacto => contacto.Nombre)
            .ToList();

        var json = JsonSerializer.Serialize(vivos, AgendaJsonContext.Default.ListContacto);
        File.WriteAllText(path, json);
    }

    private List<Contacto> LoadContactos() {
        if (!File.Exists(path)) { return []; }

        var json = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(json)) { return []; }

        List<Contacto> contactos;

        try {
            contactos = JsonSerializer.Deserialize(json, AgendaJsonContext.Default.ListContacto) ?? [];
        } catch {
            return [];
        }

        return contactos;
    }
}


public sealed class SqliteContactoStore : IStore<Contacto> {
    private readonly string path;
    private readonly string connectionString;

    public SqliteContactoStore(string path) {
        this.path = path;
        connectionString = $"Data Source={path}";
        InitializeSchema();
    }

    public IReadOnlyList<Contacto> Load() {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command   = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nombre, Apellido, Domicilio, TelefonosJson FROM Contactos;";

        using var reader = command.ExecuteReader();
        var result = new List<Contacto>();

        while (reader.Read()) {
            var contacto = MaterializarContacto(reader);
            result.Add(contacto);
        }

        return result;
    }

    public void SaveChanges(IReadOnlyCollection<StoreChange<Contacto>> changes) {
        if (changes.Count == 0) { return; }

        var directorio = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directorio)) {
            Directory.CreateDirectory(directorio);
        }

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        foreach (var change in changes) {
            switch (change.Kind) {
                case StoreChangeKind.Insert:
                    ExecuteInsert(connection, transaction, change.Entity!);
                    break;
                case StoreChangeKind.Update:
                    ExecuteUpdate(connection, transaction, change.Entity!);
                    break;
                case StoreChangeKind.Delete:
                    ExecuteDelete(connection, transaction, change.Id);
                    break;
            }
        }

        transaction.Commit();
    }

    private void InitializeSchema() {
        var directorio = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directorio)) {
            Directory.CreateDirectory(directorio);
        }

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Contactos (
                Id            INTEGER PRIMARY KEY,
                Nombre        TEXT NOT NULL,
                Apellido      TEXT NOT NULL,
                Domicilio     TEXT NOT NULL,
                TelefonosJson TEXT NOT NULL
            );
        """;
        command.ExecuteNonQuery();
    }

    private static void ExecuteInsert(SqliteConnection connection, SqliteTransaction transaction, Contacto contacto) {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO Contactos (Id, Nombre, Apellido, Domicilio, TelefonosJson)
            VALUES ($id, $nombre, $apellido, $domicilio, $telefonosJson);
        """;
        BindParameters(command, contacto);
        command.ExecuteNonQuery();
    }

    private static void ExecuteUpdate(SqliteConnection connection, SqliteTransaction transaction, Contacto contacto) {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            UPDATE Contactos
            SET Nombre = $nombre,
                Apellido  = $apellido,
                Domicilio = $domicilio,
                TelefonosJson = $telefonosJson
            WHERE Id = $id;
        """;
        BindParameters(command, contacto);
        command.ExecuteNonQuery();
    }

    private static void ExecuteDelete(SqliteConnection connection, SqliteTransaction transaction, int id) {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "DELETE FROM Contactos WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    private static void BindParameters(SqliteCommand command, Contacto contacto) {
        command.Parameters.AddWithValue("$id",        contacto.Id);
        command.Parameters.AddWithValue("$nombre",    contacto.Nombre);
        command.Parameters.AddWithValue("$apellido",  contacto.Apellido);
        command.Parameters.AddWithValue("$domicilio", contacto.Domicilio);
        command.Parameters.AddWithValue("$telefonosJson", contacto.Telefonos);
    }

    private static Contacto MaterializarContacto(SqliteDataReader reader) {
        return new Contacto {
            Id        = reader.GetInt32(0),
            Nombre    = reader.GetString(1),
            Apellido  = reader.GetString(2),
            Domicilio = reader.GetString(3),
            Telefonos = reader.GetString(4),
        };
    }
}


[JsonSourceGenerationOptions( WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(List<Contacto>))]
[JsonSerializable(typeof(List<string>))]
internal partial class AgendaJsonContext : JsonSerializerContext { }
