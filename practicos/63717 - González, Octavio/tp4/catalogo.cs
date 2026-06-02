#:package Terminal.Gui@2.*
#:property PublishAot=false

using System.Net.Http.Json;
using Terminal.Gui;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Terminal.Gui.Input;
using Terminal.Gui.Drawing;
using Terminal.Gui.Configuration;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

Console.OutputEncoding = System.Text.Encoding.UTF8;
#pragma warning disable CS0618


// ── Consulta inicial al servidor ──────────────────────────────────────────


List<ProductoDto> productos;
try
{
    using var http = new HttpClient();
    productos = await ObtenerProductos(http);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"{ex.Message}");
    return;
}

ProductoDto producto = null!;
try
{
    int id = 2;
    using var http = new HttpClient();
    producto = await TraerProducto(http, id);
}
catch (Exception e)
{
    Console.Error.WriteLine($"{e.Message}");
}

// ── Interfaz TUI ──────────────────────────────────────────────────────────

ConfigurationManager.Enable(ConfigLocations.All);
ConfigurationManager.Apply();

//SCHEMES 

Color fondo = new Color(30, 30, 46);


var esquemaestro = new Scheme
{
    Normal = new Terminal.Gui.Drawing.Attribute(Color.Green, fondo),
    Focus = new Terminal.Gui.Drawing.Attribute(Color.White, Color.Green)
};

var esquedetalle = new Scheme
{
    Normal = new Terminal.Gui.Drawing.Attribute(Color.Green, Color.Black),
    Focus = new Terminal.Gui.Drawing.Attribute(Color.White, Color.Green),
    ReadOnly = new Terminal.Gui.Drawing.Attribute(Color.Green, Color.Black)
};
var esquemagui = new Scheme
{

    Normal = new Terminal.Gui.Drawing.Attribute(Color.Green, fondo)
};
SchemeManager.AddScheme("Esquemaestro", esquemaestro);
SchemeManager.AddScheme("esquedetalle", esquedetalle);
SchemeManager.AddScheme("gui", esquemagui);

using IApplication app = Application.Create().Init();
using Window gui = new() { SchemeName = "gui" };

int salir = 0;

//Para salir 
gui.KeyDown += (sender, e) =>
{
    if (e.KeyCode == Key.Esc)
    {
        e.Handled = true;
        salir = salir + 1;
        if (salir == 2)
        {
            e.Handled = false;
            app.RequestStop();
        }
    }
};

var dialogosalir = new Dialog
{
    Title = "Seguro desea salir?",
    X = Pos.Center(),
    Y = Pos.Center(),
    SchemeName = "dialog"
};

//Menu

var menu = new MenuBar
{
    Menus = [
        new MenuBarItem("_Archivo", [
            new MenuItem("_Agregar", "", () => {}),
            new MenuItem("Salir", "", () => app.RequestStop())
        ]),
        new MenuBarItem("_Movimientos", [
            new MenuItem("_Compra", "", () => {}),
        ])
    ],
    SchemeName = "Esquemaestro"

};

var buscar = new Label
{
    Text = " Buscar:",
    X = 3,
    Y = 2,
    SchemeName = "Esquemaestro"
};

var input = new TextField()
{
    X = Pos.Right(buscar),
    Y = Pos.Top(buscar),
    Width = 20,
    SchemeName = "Esquemaestro"
};

//maestro

var maestro = new FrameView
{
    Title = "Productos",
    X = 0,
    Y = 4,
    Width = 25,
    Height = Dim.Percent(90),
    SchemeName = "Esquemaestro",
    CanFocus = true

};

var panelmaestro = new ListView
{
    X = 0,
    Y = 0,
    Width = Dim.Fill(),
    Height = Dim.Fill(1),
    CanFocus = true
};


panelmaestro.SetSource(new ObservableCollection<string>(productos
.Select(p => "ID " + p.Id + " - " + p.Nombre)
.ToList()
));

maestro.Add(panelmaestro);

//detalle

var detalle = new FrameView
{
    Title = "Detalle",
    X = Pos.Right(maestro),
    Y = Pos.Top(maestro),
    Width = Dim.Fill(),
    Height = Dim.Percent(90),
    SchemeName = "esquedetalle",
    CanFocus = false
};


var listadetalles = new TextView
{
    X = 0,
    Y = 0,
    Width = Dim.Fill(),
    Height = Dim.Fill(),
    ReadOnly = true,
    SchemeName = "esquedetalle",
    WordWrap = true
};
detalle.Add(listadetalles, buscar);

var teclasdisponibles = new Label
{
    Text = "teclas disponibles",
    X = Pos.Right(detalle),
    Y = Pos.Bottom(maestro),
    Width = Dim.Fill(),
    Height = Dim.Fill(),
    SchemeName = "esquedetalle",
};

//Navegacion 
panelmaestro.KeyDown += (sender, e) =>
{
    if (e.KeyCode == Key.CursorDown && panelmaestro.SelectedItem == panelmaestro.Source?.Count - 1) 
    e.Handled = true;
    if (e.KeyCode == Key.CursorUp && panelmaestro.SelectedItem == 0)
    e.Handled = true;
};

input.KeyDown += (sender, e) =>
{
    if (e.KeyCode == Key.Enter || e.KeyCode == Key.Esc)
    {
        panelmaestro.SetFocus();
        e.Handled = true;
    }
};


//Añadir las views
gui.Add(menu, maestro, detalle, buscar, input, teclasdisponibles);
panelmaestro.ValueChanged += async (sender, e) => await Refrescardetalle(e.NewValue);

app.Run(gui);


// Funciones ----------------------------------------------------

async Task Refrescardetalle(int? indice)
{

    if (indice is null || indice < 0 || indice > productos.Count)
    {
        detalle.Text = "Nada seleccionado";
        return;
    }
    var prodseleccionado = productos[indice.Value];

    List<MovimientoDto> movimientosDelProducto = new();
    try
    {
        using var http = new HttpClient();
        string url = $"http://localhost:3000/productos/{prodseleccionado.Id}/movimientos";
        movimientosDelProducto = await http.GetFromJsonAsync<List<MovimientoDto>>(url) ?? new();
    }
    catch (Exception ex)
    {
        detalle.Text = $"No se puedo obtener movimientos - Error : {ex.Message}";
        return;
    }

    listadetalles.Text = string.Join("\n\n", movimientosDelProducto.Select(m =>
    $"""    
        ID Movimiento: {m.Id}

        Tipo:          {m.Tipo switch
    {
        1 => "Compra",
        2 => "Venta",
        3 => "Ajuste",
        _ => "No asignado"
    }}

        Descripcion:   {m.ProductoId}

        Cantidad:      {m.Cantidad}

        Stock :         unidades

        Fecha:         {m.Fecha}
        
    """));
}


async void Refrescar()
{
    try
    {
        using var http = new HttpClient();
        productos = await ObtenerProductos(http);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"{ex.Message}");
        return;
    }

}
static async Task<List<ProductoDto>> ObtenerProductos(HttpClient http)
{
    const string url = "http://localhost:3000/productos";
    return await http.GetFromJsonAsync<List<ProductoDto>>(url) ?? throw new HttpRequestException("No hay productos");
}
static async Task<List<MovimientoDto>> ObtenerMovimientos(HttpClient http, int id)
{
    string url = $"http://localhost:3000/productos/{id}/movimientos";
    return await http.GetFromJsonAsync<List<MovimientoDto>>(url) ?? throw new HttpRequestException("No hay movimientos");
}

static async Task<ProductoDto> TraerProducto(HttpClient http, int id)
{
    string url = $"http://localhost:3000/productos/{id}";
    return await http.GetFromJsonAsync<ProductoDto>(url) ?? throw new HttpRequestException("No existe un producto con este ID");
}


// ── DTO ───────────────────────────────────────────────────────────────────

record ProductoDto(int Id, string Codigo, string Nombre, decimal Precio, int Stock);
record MovimientoDto(int Id, int ProductoId, int Tipo, int Cantidad, DateTime Fecha);
