#:package Terminal.Gui@2.*
#:property PublishAot=false

using System.Net.Http.Json;
using Terminal.Gui.App;
using Terminal.Gui.Views;

// ── Consulta inicial al servidor ──────────────────────────────────────────

// ProductoDto producto; //no terminada 
// try {
//     using var http = new HttpClient();
//     int id = 0; 
//     producto = await CargarProductoAsync(http, id);
// } catch (HttpRequestException ex) {
//     Console.Error.WriteLine($"No se pudo conectar con el servidor: {ex.Message}");
//     Console.Error.WriteLine("Verificá que servidor.cs esté corriendo");
//     return;
// }

ProductoDto[] productos;
try {
    using var http = new HttpClient();
    productos = await CargarProductosAsync(http);
} catch (Exception ex) {
    Console.Error.WriteLine($"{ex.Message}");
    return;
}

// ── Interfaz TUI ──────────────────────────────────────────────────────────

using IApplication app = Application.Create().Init();
using Window ventana = new () { Title = " Catalogo de Productos (ESC para salir) " };

var detalleProducto = new Label {
    Text = string.Join("\n\n", productos.Select(p => $"""
            # PRODUCTOS
            - Id     : {p.Id, 1}
            - Código : {p.Codigo, 1}
            - Nombre : {p.Nombre, 1}
            - Precio : {p.Precio, 1}
            - Stock  : {p.Stock, 1}
            """)),
    X = 4, Y = 2,
};  
ventana.Add(detalleProducto);


app.Run(ventana);

static async Task<ProductoDto[]> CargarProductosAsync (HttpClient http) {
    const string url = "http://localhost:3000/productos";
    return await http.GetFromJsonAsync<ProductoDto[]>(url) ?? throw new HttpRequestException("No hay productos");
}

static async Task<ProductoDto> CargarProductoAsync (HttpClient http, int id) {
    string url = $"http://localhost:3000/productos/{id}";
    return await http.GetFromJsonAsync<ProductoDto>(url) ?? throw new HttpRequestException("No existe un producto con este ID");
}
// ── DTO ───────────────────────────────────────────────────────────────────

record ProductoDto(int Id, string Codigo, string Nombre, decimal Precio, int Stock);
// record MovimientoDTO(int Id, int Codigo, TipoMovimiento Tipo, int Cantidad,DateTime Fecha,
// int ProductoId) 
// {
//     public Producto? Producto { get; set; }
// }