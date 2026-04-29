#:package Terminal.Gui@2.*

using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Terminal.Gui;


var agenda = new List<Contacto> {
    new Contacto("Alejandro", "Acosta",    "Calle Falsa 123",           new List<string> { "12345678", "87654321" }),
    new Contacto("Bruno",     "Benitez",   "Avenida Siempre Viva 742",  new List<string> { "55555555" }),
    new Contacto("Camila",    "Castro",    "Calle Real 456",            new List<string> { "11111111", "22222222" }),
    new Contacto("Diego",     "Diaz",      "Pasaje del Sol 89",         new List<string> { "33333333" }),
    new Contacto("Elena",     "Escobar",   "Boulevard Central 120",     new List<string> { "44444444", "99999999" }),
    new Contacto("Federico",  "Fernandez", "Ruta 9 Km 12",              new List<string> { "66666666" }),
    new Contacto("Gabriela",  "Gomez",     "Los Lapachos 300",          new List<string> { "77777777", "10101010" }),
    new Contacto("Hector",    "Herrera",   "San Martin 450",            new List<string> { "88888888" }),
    new Contacto("Ines",      "Ibarra",    "Mitre 155",                 new List<string> { "12121212" }),
    new Contacto("Julian",    "Juarez",    "Belgrano 980",              new List<string> { "13131313", "14141414" }),
};

TableView? tabla = null;
Toplevel? top = null;
Window? ventana = null;
string? archivoActual = null;
JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

Application.Init();
try {
    top = new Toplevel();

    var menu   = CrearMenu();
    var window = CrearVentana();

    top.KeyDown += TopKeyDown;
    top.Add(menu, window) ;
    Application.Run(top);
} finally {
    Application.Shutdown();
}


Window CrearVentana() {
    var win = new Window { Title = TituloVentana(), X = 0,  Y = 1 };
    win.Add(new Label{Title = "Centro", X = Pos.Center(), Y = 0 });

    tabla = new TableView {
        X = 0, Y = 1,
        Width  = Dim.Fill(),
        Height = Dim.Fill(),
        Table  = new DatosContactos(agenda),
        FullRowSelect = true,
        MultiSelect = false
    };
    tabla.KeyDown += TablaKeyDown;
    win.Add( tabla );
    ventana = win;
    return win;
}

void TablaKeyDown(object? sender, Key e) {
    if (e.KeyCode == KeyCode.Enter) {
        EditarContacto();
        e.Handled = true;
    } else if (EsTeclaBorrado(e)) {
        BorrarContacto();
        e.Handled = true;
    }
}

void TopKeyDown(object? sender, Key e) {
    if (e.KeyCode == KeyCode.F2) {
        AgregarContacto();
        e.Handled = true;
    } else if (EsTeclaBorrado(e)) {
        BorrarContacto();
        e.Handled = true;
    } else if (e == Key.O.WithCtrl) {
        AbrirAgenda();
        e.Handled = true;
    } else if (e == Key.S.WithCtrl) {
        GuardarAgenda();
        e.Handled = true;
    } else if (e == Key.S.WithCtrl.WithShift) {
        GuardarAgendaComo();
        e.Handled = true;
    } else if (e == Key.Q.WithCtrl) {
        Salir();
        e.Handled = true;
    }
}

bool EsTeclaBorrado(Key e) {
    return e.KeyCode == KeyCode.Delete
        || e.KeyCode == KeyCode.Backspace
        || e == Key.Delete
        || e == Key.Backspace;
}

// Funciones
void NuevaAgenda() {
    agenda.Clear();
    archivoActual = null;
    RefrescarTabla();
    RefrescarTituloVentana();
}

void AbrirAgenda() {
    string? ruta = ElegirArchivoAbrir();
    if (string.IsNullOrWhiteSpace(ruta)) {
        return;
    }

    CargarDesdeArchivo(ruta);
}

void GuardarAgenda() {
    if (!string.IsNullOrWhiteSpace(archivoActual)) {
        GuardarEnArchivo(archivoActual);
        return;
    }

    GuardarAgendaComo();
}

void GuardarAgendaComo() {
    string? ruta = ElegirArchivoGuardar();
    if (string.IsNullOrWhiteSpace(ruta)) {
        return;
    }

    GuardarEnArchivo(ruta);
}

void Salir() {
    Console.WriteLine("Salir");
    top?.RequestStop();
}

void AgregarContacto(){
    Contacto? nuevo = EditorContacto("Agregar contacto", new Contacto("", "", "", new List<string>()));
    if (nuevo is null) {
        return;
    }

    agenda.Add(nuevo);
    RefrescarTabla();
};

void EditarContacto() {
    if (tabla is null) {
        return;
    }

    int indice = tabla.SelectedRow;
    if (indice < 0 || indice >= agenda.Count) {
        return;
    }

    Contacto? editado = EditorContacto("Editar contacto", agenda[indice]);
    if (editado is null) {
        return;
    }

    agenda[indice] = editado;
    RefrescarTabla();
}

int IndiceContactoSeleccionado() {
    if (tabla is null) {
        return -1;
    }

    int indice = tabla.SelectedRow;
    return indice >= 0 && indice < agenda.Count ? indice : -1;
}

void RefrescarTabla() {
    if (tabla is null) {
        return;
    }

    tabla.Update();
    tabla.SetNeedsDraw();
}

string TituloVentana() {
    string nombreArchivo = string.IsNullOrWhiteSpace(archivoActual)
        ? "(sin archivo)"
        : Path.GetFileName(archivoActual);

    return $"Agenda de contactos — {nombreArchivo}";
}

void RefrescarTituloVentana() {
    if (ventana is null) {
        return;
    }

    ventana.Title = TituloVentana();
    ventana.SetNeedsDraw();
}

void CargarDesdeArchivo(string ruta) {
    try {
        string json = File.ReadAllText(ruta);
        List<Contacto>? contactos = JsonSerializer.Deserialize(json, AgendaJsonContext.Default.ListContacto);

        agenda.Clear();
        if (contactos is not null) {
            agenda.AddRange(contactos);
        }

        archivoActual = ruta;
        RefrescarTabla();
        RefrescarTituloVentana();
    } catch (Exception ex) {
        MessageBox.ErrorQuery("Error al abrir", ex.Message, "Aceptar");
    }
}

void GuardarEnArchivo(string ruta) {
    try {
        string json = JsonSerializer.Serialize(agenda, AgendaJsonContext.Default.ListContacto);
        File.WriteAllText(ruta, json);
        archivoActual = ruta;
        RefrescarTituloVentana();
    } catch (Exception ex) {
        MessageBox.ErrorQuery("Error al guardar", ex.Message, "Aceptar");
    }
}

string? ElegirArchivoAbrir() {
    var dialog = new OpenDialog {
        Title = "Abrir agenda",
        AllowsMultipleSelection = false,
        OpenMode = OpenMode.File,
    };
    dialog.AllowedTypes = [ new AllowedType("Archivos JSON", ".json") ];

    Application.Run(dialog);
    string? ruta = !dialog.Canceled && dialog.Path is { Length: > 0 } ? dialog.Path : null;
    dialog.Dispose();
    return ruta;
}

string? ElegirArchivoGuardar() {
    var dialog = new SaveDialog {
        Title = "Guardar agenda como...",
        OpenMode = OpenMode.File,
    };
    dialog.AllowedTypes = [ new AllowedType("Archivos JSON", ".json") ];

    Application.Run(dialog);

    string? ruta = null;
    if (!dialog.Canceled && dialog.Path is { Length: > 0 }) {
        ruta = dialog.Path;
        if (!Path.HasExtension(ruta)) {
            ruta += ".json";
        }
    }

    dialog.Dispose();
    return ruta;
}

void BorrarContacto() {
    int indice = IndiceContactoSeleccionado();
    if (indice < 0) {
        return;
    }

    Contacto contacto = agenda[indice];
    int respuesta = MessageBox.Query(
        "Borrar contacto",
        $"¿Querés borrar a {contacto.NombreCompleto}?",
        "Sí",
        "No");

    if (respuesta != 0) {
        return;
    }

    agenda.RemoveAt(indice);
    RefrescarTabla();
}

MenuBar CrearMenu() {
    return new MenuBar { // Linea horizontal
        Menus = [
            new MenuBarItem ("_Agenda", [   // Menu Vertical
                new MenuItem("_Nueva",           Key.N.WithCtrl,           NuevaAgenda),       // Cada item
                new MenuItem("_Abrir...",        Key.O.WithCtrl,           AbrirAgenda),
                null!,
                new MenuItem("_Guardar",         Key.S.WithCtrl,           GuardarAgenda),
                new MenuItem("Guardar _Como...", Key.S.WithCtrl.WithShift, GuardarAgendaComo),
                null!,
                new MenuItem ("_Salir",          Key.Q.WithCtrl,           Salir )
            ]),
            new MenuBarItem ("_Contacto", [
                new MenuItem("_Agregar",         Key.F2,                   AgregarContacto),
                new MenuItem("_Editar...",       Key.Enter,                EditarContacto),
                null!,
                new MenuItem("_Borrar",          Key.Backspace,            BorrarContacto),
            ])
        ]
    };
}

Contacto? EditorContacto(string titulo, Contacto contacto) {
    var dialog = new Dialog { Title = titulo, Width = 60, Height = 15, Modal = true };
    Contacto? editado = null;
    
    dialog.Add(new Label { Text = "Nombre    :", X = 1, Y = 1 });
    var nombre = new TextField { Text = contacto.Nombre, X = 12, Y = 1, Width = 40 };
    dialog.Add(nombre);

    dialog.Add(new Label { Text = "Apellido  :", X = 1, Y = 2 });
    var apellido = new TextField { Text = contacto.Apellido, X = 12, Y = 2, Width = 40 };
    dialog.Add(apellido);
    
    dialog.Add(new Label { Text = "Domicilio :", X = 1, Y = 3 });
    var domicilio = new TextField { Text = contacto.Domicilio, X = 12, Y = 3, Width = 40 };
    dialog.Add(domicilio);

    dialog.Add(new Label { Text = "Telefonos :", X = 1, Y = 4 });
    var telefonos = new TextField { Text = string.Join(" | ", contacto.Telefonos), X = 12, Y = 4, Width = 40 };
    dialog.Add(telefonos);

    var aceptar  = new Button{ Title = "Aceptar", IsDefault = true, };
    var cancelar = new Button{ Title = "Cancelar" };
    aceptar.Accepting += (_,_) => {
        editado = new Contacto(
            nombre.Text.ToString(),
            apellido.Text.ToString(),
            domicilio.Text.ToString(),
            telefonos.Text.ToString()
                .Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToList());
        dialog.RequestStop();
    };
    cancelar.Accepting += (_,_) => dialog.RequestStop();
    
    dialog.AddButton(aceptar);
    dialog.AddButton(cancelar);
    Application.Run(dialog);
    dialog.Dispose();
    return editado;
}

record Contacto(string Nombre,  string Apellido, string Domicilio, List<string> Telefonos) {
    public string NombreCompleto => $"{Apellido}, {Nombre}";
};

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<Contacto>))]
internal partial class AgendaJsonContext : JsonSerializerContext { }

class DatosContactos : ITableSource {
    private List<Contacto> agenda;

    public DatosContactos(List<Contacto> agenda) => this.agenda = agenda;

    public int Columns => 3;
    public int Rows => agenda.Count;

    public string[] ColumnNames => new[] { "Nombre                      ", "Domicilio", "Telefonos" };
    public object this[int row, int col] { get
        => col switch {
            0 => agenda[row].NombreCompleto,
            1 => agenda[row].Domicilio,
            2 => string.Join(" | ", agenda[row].Telefonos),
            _ => throw new ArgumentOutOfRangeException(nameof(col))
        };
    }
    
}
