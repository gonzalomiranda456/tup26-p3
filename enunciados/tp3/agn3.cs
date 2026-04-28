#:package Terminal.Gui@2.*

using System.Data;
using System.Text.Json;
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

Application.Init();
try {
    var top = new Toplevel();

    var menu   = CrearMenu();
    var window = CrearVentana();

    top.Add(menu, window) ;
    Application.Run(top);
} finally {
    Application.Shutdown();
}


Window CrearVentana() {
    var win = new Window { Title = "Agenda de contactos", X = 0,  Y = 1, Width = 60, Height = 10 };
    win.Add(new Label{Title = "Centro", X = Pos.Center(), Y = 0 });

    var tabla = new TableView {
        X = 0, Y = 1,
        Width  = Dim.Fill(),
        Height = Dim.Fill(),
        Table  = new DatosContactos(agenda)
    };
    win.Add( tabla );
    return win;
}

// Funciones
void NuevaAgenda()       => Console.WriteLine("Nueva agenda");
void AbrirAgenda()       => Console.WriteLine("Abrir agenda");
void GuardarAgenda()     => Console.WriteLine("Guardar agenda");
void GuardarAgendaComo() => Console.WriteLine("Guardar agenda como");
void Salir()             => Console.WriteLine("Salir");

void AgregarContacto(){
    EditorContacto("Agregar contacto", new Contacto("", "", "", new List<string>()));
    Console.WriteLine("Agregar contacto");
};

void EditarContacto()    => Console.WriteLine("Editar contacto");
void BorrarContacto()    => Console.WriteLine("Borrar contacto");

MenuBar CrearMenu() {
    return new MenuBar { // Linea horizontal
        Menus = [
            new MenuBarItem ("_Agenda", [   // Menu Vertical
                new MenuItem("_Nueva",           Key.N.WithCtrl,            NuevaAgenda),       // Cada item
                new MenuItem("_Abrir...",        Key.A.WithCtrl,            AbrirAgenda),
                null!,
                new MenuItem("_Guardar",         Key.G.WithCtrl,            GuardarAgenda),
                new MenuItem("Guardar Como...",  Key.G.WithCtrl.WithShift,  GuardarAgendaComo),
                null!,
                new MenuItem ("Salir",           Key.X.WithCtrl,            Salir )
            ]),
            new MenuBarItem ("_Contacto", [
                new MenuItem("_Agregar",         Key.A.WithCtrl,            AgregarContacto),
                new MenuItem("_Editar...",       Key.E.WithCtrl,            EditarContacto),
                null!,
                new MenuItem("_Borrar",          Key.B.WithCtrl,            BorrarContacto),
            ])
        ]
    };
}

void EditorContacto(string titulo, Contacto contacto) {
    var dialog = new Dialog { Title = titulo, Width = 40, Height = 12 };
    
    dialog.Add(new Label { Text = "Nombre    :", X = 1, Y = 1 });
    var nombre = new TextField { Text = contacto.Nombre, X = 12, Y = 1, Width = 20 };
    dialog.Add(nombre);

    dialog.Add(new Label { Text = "Apellido  :", X = 1, Y = 2 });
    var apellido = new TextField { Text = contacto.Apellido, X = 12, Y = 2, Width = 20 };
    dialog.Add(apellido);
    
    dialog.Add(new Label { Text = "Domicilio :", X = 1, Y = 3 });
    var domicilio = new TextField { Text = contacto.Domicilio, X = 12, Y = 3, Width = 20 };
    dialog.Add(domicilio);

    dialog.Add(new Label { Text = "Telefonos :", X = 1, Y = 4 });
    var telefonos = new TextField { Text = string.Join(" | ", contacto.Telefonos), X = 12, Y = 4, Width = 20 };
    dialog.Add(telefonos);

    var aceptar  = new Button{ Title = "Aceptar", IsDefault = true, };
    var cancelar = new Button{ Title = "Cancelar" };
    aceptar.Accepting += (_,_) => {
        contacto = new Contacto(nombre.Text.ToString(), apellido.Text.ToString(), domicilio.Text.ToString(), telefonos.Text.ToString().Split(" | ").ToList());
        dialog.RequestStop();
    };
    cancelar.Accepting += (_,_) => dialog.RequestStop();
    
    dialog.AddButton(aceptar);
    dialog.AddButton(cancelar);
    Application.Run(dialog);
    dialog.Dispose();
}

record Contacto(string Nombre,  string Apellido, string Domicilio, List<string> Telefonos) {
    public string NombreCompleto => $"{Apellido}, {Nombre}";
};

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
