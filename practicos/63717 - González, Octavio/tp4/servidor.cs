#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@10
#:property PublishAot=false


using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

// ── Configuración ──────────────────────────────────────────────────────────

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogoDb>(options => options.UseSqlite("Data Source=catalogo.db"));
builder.Services.AddScoped<Repositorio>();
var app = builder.Build();

// ── Inicialización de la base de datos ────────────────────────────────────

using (var scope = app.Services.CreateScope())
{
    var repositorio = scope.ServiceProvider.GetRequiredService<Repositorio>();
    repositorio.Iniciar();
}

// ── Endpoints ─────────────────────────────────────────────────────────────

//productos///
app.MapGet("/productos", (Repositorio repositorio) =>
{
    var productos = repositorio.TraerProducto();
    if (productos is null) return Results.NotFound();
    return Results.Ok(productos);
});
app.MapGet("/productos/{id:int}", (int id, Repositorio repositorio) =>
{
    var producto = repositorio.TraerProducto(id);
    if (producto is null) return Results.NotFound();
    return Results.Ok(producto);
});

app.MapPost("/productos", (Repositorio repositorio) => { });

app.MapPut("/productos/{id:int}", (int id, Producto producto, Repositorio repositorio) => { });

app.MapDelete("/productos/{id:int}", (int id, Repositorio repositorio) => { });
//Movimiento de productos///

app.MapGet("/productos/{id:int}/movimientos", (int id, Repositorio repositorio) =>
{
    var movimiento = repositorio.TraerMovimiento(id);
    if (movimiento is null) return Results.NotFound();
    return Results.Ok(movimiento);
});

app.MapPost("/productos/{id:int}/movimientos", (int id, Repositorio repositorio) => { });


app.Run("http://localhost:3000");


// ── Modelo ────────────────────────────────────────────────────────────────

//relacion 1 producto  a N movs
class Producto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public Producto(int id, string codigo, string nombre, decimal precio, int stock)
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        Precio = precio;
        Stock = stock;
    }
}

class Movimiento
{
    public int Id { get; set; }
    public int Codigo { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public Movimiento(int id, int codigo, TipoMovimiento tipo, int cantidad, DateTime fecha, int productoId)
    {
        Id = id;
        Codigo = codigo;
        Tipo = tipo;
        Cantidad = cantidad;
        Fecha = fecha;
        ProductoId = productoId;
    }
}

enum TipoMovimiento { compra = 1, venta = 2, ajuste = 3 }


// ── DbContext ─────────────────────────────────────────────────────────────
class CatalogoDb : DbContext
{
    public CatalogoDb(DbContextOptions<CatalogoDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>(); //tabla producto. 
    public DbSet<Movimiento> Movimientos => Set<Movimiento>(); //tabla Movimientos. 
}

// ── Repositorio ───────────────────────────────────────────────────────────

class Repositorio
{
    private readonly CatalogoDb db;

    public Repositorio(CatalogoDb db) => this.db = db;

    public void Iniciar()
    {
        db.Database.EnsureCreated();

        if (!db.Productos.Any())
        {
            db.Productos.Add(new Producto(0, "P000", "", 0m, 0));
            db.Productos.Add(new Producto(0, "P001", "Yerba Mate 500g", 1500m, 100));
            db.Productos.Add(new Producto(0, "P002", "Coca Cola 2L", 2000m, 150));
            db.Productos.Add(new Producto(0, "P003", "Papas Fritas 150g", 800m, 200));
            db.SaveChanges();
        }
    }

    public List<Producto> TraerProducto() =>
        db.Productos.ToList();

    public Producto? TraerProducto(int id) =>
        db.Productos.FirstOrDefault(p => p.Id == id);

    public List<Movimiento> TraerMovimiento(int id) =>
    db.Movimientos.Where(m => m.ProductoId == id).ToList();

    public Producto? Actualizar(int id) => db.Productos.FirstOrDefault(p => p.Id == id);

}


