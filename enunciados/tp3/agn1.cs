#:package Terminal.Gui@2.0.1-develop.1

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

var usaArchivoPorDefecto = args.Length == 0;
var jsonPath = !usaArchivoPorDefecto
    ? Path.GetFullPath(args[0])
    : Path.Combine(SourceDirectory(), "agenda.json");
var jsonInicialPath = Path.Combine(SourceDirectory(), "agenda-inicial.json");

var contactos = CargarAgenda(jsonPath, usaArchivoPorDefecto ? jsonInicialPath : null);

using IApplication app = Application.Create();
app.Init();

using var root = new Runnable {
    Width = Dim.Fill(),
    Height = Dim.Fill()
};

var vista = new List<Contacto>();
var indiceSeleccionado = -1;
TextField txtBuscar = null!;
ListView lista = null!;
const int AnchoNombre = 28;
const int AnchoDomicilio = 34;

Menu.DefaultBorderStyle = LineStyle.Single;

var menu = new MenuBar {
    X = 0,
    Y = 0,
    Width = Dim.Fill(),
    Menus = new[] {
        new MenuBarItem("_Agenda", new MenuItem[] {
            new MenuItem("_Salir", Key.Q.WithCtrl, Salir),
        }),
        new MenuBarItem("_Contacto", new MenuItem[] {
            new MenuItem("_Agregar", Key.F2, Agregar),
            new MenuItem("_Editar", Key.Enter, EditarSeleccionado),
            new MenuItem("_Borrar", Key.Delete, BorrarSeleccionado),
        }),
    }
};

var ventana = new Window {
    X = 0,
    Y = 1,
    Width = Dim.Fill(),
    Height = Dim.Fill(1),
    Title = $"Agenda - {Path.GetFileName(jsonPath)}",
    BorderStyle = LineStyle.Single
};

var lblBuscar = new Label { Text = "Buscar:", X = 2, Y = 1 };
txtBuscar = new TextField {
    X = Pos.Right(lblBuscar) + 1,
    Y = 1,
    Width = Dim.Fill(3)
};

var lblColumnas = new Label {
    Text = EncabezadoLista(),
    X = 2,
    Y = 3,
    Width = Dim.Fill(3)
};

lista = new ListView {
    X = 2,
    Y = 4,
    Width = Dim.Fill(3),
    Height = Dim.Fill(3),
    CanFocus = true
};

ventana.Add(lblBuscar, txtBuscar, lblColumnas, lista);
root.Add(menu, ventana);
GuardarAutomatico();

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

List<Contacto> CargarAgenda(string path, string? fallbackPath) {
    var pathToLoad = File.Exists(path)
        ? path
        : (!string.IsNullOrWhiteSpace(fallbackPath) && File.Exists(fallbackPath) ? fallbackPath : null);

    if (pathToLoad is null) {
        return [];
    }

    var json = File.ReadAllText(pathToLoad);

    if (string.IsNullOrWhiteSpace(json)) {
        return [];
    }

    try {
        return JsonSerializer.Deserialize(json, AgendaJsonContext.Default.ListContacto) ?? [];
    } catch {
        return [];
    }
}

void GuardarAutomatico() {
    try {
        var directorio = Path.GetDirectoryName(jsonPath);
        if (!string.IsNullOrWhiteSpace(directorio)) {
            Directory.CreateDirectory(directorio);
        }

        var json = JsonSerializer.Serialize(contactos, AgendaJsonContext.Default.ListContacto);
        File.WriteAllText(jsonPath, json);
    } catch (Exception ex) {
        MostrarMensaje("No se pudo guardar", ex.Message, "OK");
    }
}

void Salir() {
    app.RequestStop(root);
}

void Refrescar(Contacto? seleccionar = null) {
    var filtro = Texto(txtBuscar);

    vista = contactos
        .Where(c => string.IsNullOrWhiteSpace(filtro) || Coincide(c, filtro))
        .OrderBy(c => c.Apellido)
        .ThenBy(c => c.Nombre)
        .ToList();

    lista.SetSource<string>([.. vista.Select(FormatearContacto)]);

    if (vista.Count == 0) {
        indiceSeleccionado = -1;
    } else {
        var indice = seleccionar is null ? 0 : vista.IndexOf(seleccionar);
        if (indice < 0) {
            indice = 0;
        }

        lista.SelectedItem = indice;
        indiceSeleccionado = indice;
        lista.EnsureSelectedItemVisible();
    }

    lista.SetNeedsDraw();
}

void Agregar() {
    var nuevo = EditarContacto(null);

    if (nuevo is null) {
        return;
    }

    contactos.Add(nuevo);
    GuardarAutomatico();
    txtBuscar.Text = "";
    Refrescar(nuevo);
    lista.SetFocus();
}

void EditarSeleccionado() {
    var contacto = ContactoSeleccionado();

    if (contacto is null) {
        return;
    }

    var editado = EditarContacto(contacto);

    if (editado is null) {
        return;
    }

    contacto.Nombre = editado.Nombre;
    contacto.Apellido = editado.Apellido;
    contacto.Domicilio = editado.Domicilio;
    contacto.Telefonos = editado.Telefonos;
    GuardarAutomatico();
    Refrescar(contacto);
    lista.SetFocus();
}

void BorrarSeleccionado() {
    var contacto = ContactoSeleccionado();

    if (contacto is null) {
        return;
    }

    var respuesta = MostrarMensaje(
        "Borrar contacto",
        $"¿Borrar a {NombreVisible(contacto)}?",
        "Sí",
        "No");

    if (respuesta != 0) {
        return;
    }

    contactos.Remove(contacto);
    GuardarAutomatico();
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

    var dialogo = new Dialog {
        Title = original is null ? "Agregar contacto" : "Editar contacto",
        Width = 60,
        Height = 14
    };

    var txtNombre    = new TextField { Text = original?.Nombre ?? "", X = 14, Y = 1, Width = 40 };
    var txtApellido  = new TextField { Text = original?.Apellido ?? "", X = 14, Y = 3, Width = 40 };
    var txtDomicilio = new TextField { Text = original?.Domicilio ?? "", X = 14, Y = 5, Width = 40 };
    var txtTelefonos = new TextField {
        Text = string.Join(", ", original?.Telefonos ?? []),
        X = 14,
        Y = 7,
        Width = 40
    };

    var error = new Label { X = 2, Y = 9, Width = Dim.Fill(2), Text = "" };

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
            Telefonos = Texto(txtTelefonos)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList()
        };

        app.RequestStop(dialogo);
        e.Handled = true;
    };

    btnCancelar.Accepting += (_, e) => {
        app.RequestStop(dialogo);
        e.Handled = true;
    };

    dialogo.Add(
        new Label { Text = "Nombre:", X = 2, Y = 1 },
        txtNombre,
        new Label { Text = "Apellido:", X = 2, Y = 3 },
        txtApellido,
        new Label { Text = "Domicilio:", X = 2, Y = 5 },
        txtDomicilio,
        new Label { Text = "Teléfonos:", X = 2, Y = 7 },
        txtTelefonos,
        new Label { Text = "Separalos con coma", X = 14, Y = 8 },
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
    var nombre = NombreVisible(contacto);
    var domicilio = string.IsNullOrWhiteSpace(contacto.Domicilio) ? "(sin domicilio)" : contacto.Domicilio;
    var telefonos = contacto.Telefonos.Count == 0 ? "(sin teléfonos)" : string.Join(", ", contacto.Telefonos);
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
        || contacto.Telefonos.Any(t => Contiene(t, filtro));
}

bool Contiene(string? texto, string filtro) {
    return texto?.Contains(filtro, StringComparison.OrdinalIgnoreCase) == true;
}

string Texto(TextField field) {
    return field.Text?.ToString()?.Trim() ?? "";
}

public sealed class Contacto {
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Domicilio { get; set; } = "";
    public List<string> Telefonos { get; set; } = [];
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(List<Contacto>))]
internal partial class AgendaJsonContext : JsonSerializerContext {
}
