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
List<MovimientoDto> movimientos;
try
{
    int id = 2;
    using var http = new HttpClient();
    movimientos = await ObtenerMovimientos(http, id);
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

SchemeManager.AddScheme("Esquemaestro", esquemaestro);
SchemeManager.AddScheme("esquedetalle", esquedetalle);

using (IApplication app = Application.Create().Init())
{

    Window gui = new() { };

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
        Height = Dim.Fill(1),
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
        Height = Dim.Fill(),
        SchemeName = "esquedetalle"
    };


    var detalles = new TextView
    {
        Text = string.Join("\n\n", movimientos.Select(m => $"""
    ID:    {m.Id} | Cod:   {m.ProductoId}
    Nombre: {m.Tipo}
    Precio: ${m.Cantidad}
    Stock : {producto.Stock}
    Fecha: {m.Fecha}
    """)),
        X = 0,
        Y = 0,
        Width = Dim.Fill(),
        Height = Dim.Fill(),
        ReadOnly = true,
        SchemeName = "esquedetalle"

    };
    detalle.Add(detalles, buscar);


    //Añadir las views
    gui.Add(menu, maestro, detalle, buscar, input);
    gui.SchemeName = "Dialog";

    app.Run(gui);
}

// Funciones ----------------------------------------------------

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