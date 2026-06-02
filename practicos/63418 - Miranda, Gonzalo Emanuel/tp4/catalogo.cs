#:package Terminal.Gui@2.*
#:property PublishAot=false

using System.Net.Http.Json;
using Terminal.Gui.App;
using Terminal.Gui.Views;

// ── Consulta inicial al servidor ──────────────────────────────────────────

ProductoDto producto;
try {
    using var http = new HttpClient();
    producto = await CargarProductoAsync(http);
} catch (HttpRequestException ex) {
    Console.Error.WriteLine($"No se pudo conectar con el servidor: {ex.Message}");
    Console.Error.WriteLine("Verificá que servidor.cs esté corriendo en http://localhost:5050");
    return;
}

// ── Interfaz TUI ──────────────────────────────────────────────────────────

using IApplication app = Application.Create().Init();
using Window ventana = new () { Title = " Catalogo REST — Producto (ESC para salir) " };

var detalleProducto = new Label {
    Text = $"""
            # PRODUCTO 

            - Id     : {producto.Id}
            - Código : {producto.Codigo}
            - Nombre : {producto.Nombre}
            - Precio : ${producto.Precio,10:N2}
            - Stock  :  {producto.Stock,10}
            """,
    X = 4, Y = 2,
};

ventana.Add(detalleProducto);

app.Run(ventana);

// ── Cliente HTTP ──────────────────────────────────────────────────────────

public class CatalogoApiClient
{
    private readonly HttpClient _http;

    public CatalogoApiClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<List<Producto>> GetProductosAsync() =>
        await _http.GetFromJsonAsync<List<Producto>>("/productos") ?? new();

    public async Task CrearProductoAsync(Producto producto) =>
        await _http.PostAsJsonAsync("/productos", producto);

    public async Task ActualizarProductoAsync(Producto producto) =>
        await _http.PutAsJsonAsync($"/productos/{producto.Id}", producto);

    public async Task EliminarProductoAsync(int id) =>
        await _http.DeleteAsync($"/productos/{id}");

    public async Task<List<MovimientoDeProducto>> GetMovimientosAsync(int productoId) =>
        await _http.GetFromJsonAsync<List<MovimientoDeProducto>>($"/productos/{productoId}/movimientos") ?? new();

    public async Task RegistrarMovimientoAsync(int productoId, MovimientoDeProducto movimiento) =>
        await _http.PostAsJsonAsync($"/productos/{productoId}/movimientos", movimiento);
}

// ── Modelos de Datos ──────────────────────────────────────────────────────

public class Producto 
{
    public int Id { get; set; }
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

public class MovimientoDeProducto 
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string Tipo { get; set; } = ""; 
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
}
