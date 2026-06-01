#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@*
#:property PublishAot=false

using Microsoft.EntityFrameworkCore;

// ── Configuración ──────────────────────────────────────────────────────────

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogoDb>(opt => opt.UseSqlite("Data Source=catalogo.db"));
builder.Services.AddScoped<CatalogoRepositorio>();

var app = builder.Build();

// ── Inicialización de la base de datos ────────────────────────────────────

using (var scope = app.Services.CreateScope()) {
    var repositorio = scope.ServiceProvider.GetRequiredService<CatalogoRepositorio>();
    repositorio.Iniciar();
}

// ── Endpoints ─────────────────────────────────────────────────────────────

app.MapGet("/producto", (CatalogoRepositorio repositorio) => {
    var producto = repositorio.TraerProducto();
    if(producto is null) return Results.NotFound();

    return Results.Ok(producto);
});

app.Run("http://localhost:5050");



// ── Modelo ────────────────────────────────────────────────────────────────

public class Producto {
    public int Id { get; set; }
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

public class MovimientoDeProducto {
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string Tipo { get; set; } = ""; // Compra, Venta o Ajuste
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
}

// ── DbContext ─────────────────────────────────────────────────────────────

class CatalogoDb : DbContext {
    public CatalogoDb(DbContextOptions<CatalogoDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<MovimientoDeProducto> Movimientos => Set<MovimientoDeProducto>();
}

// ── Repositorio ───────────────────────────────────────────────────────────

class CatalogoRepositorio {
    private readonly CatalogoDb db;

    public CatalogoRepositorio(CatalogoDb db) => this.db = db;

    public void Iniciar() {
        db.Database.EnsureCreated();

        if (!db.Productos.Any()) {
            db.Productos.Add(new Producto { 
                Codigo = "P001", 
                Nombre = "Yerba Mate 500g", 
                Precio = 1500m, 
                Stock = 100 
            });
            db.SaveChanges();
        }
    }
// ── Métodos de Producto ──────────────────────────────────────────────────

    public List<Producto> TraerTodos() => db.Productos.ToList();

    public Producto? TraerProducto(int id) => db.Productos.Find(id);

    public void CrearProducto(Producto p) {
        db.Productos.Add(p);
        db.SaveChanges();
    }

    public bool ModificarProducto(int id, Producto pActualizado) {
        var p = db.Productos.Find(id);
        if (p is null) return false;

        p.Codigo = pActualizado.Codigo;
        p.Nombre = pActualizado.Nombre;
        p.Precio = pActualizado.Precio;
        
        db.SaveChanges();
        return true;
    }

    public bool EliminarProducto(int id) {
        var p = db.Productos.Find(id);
        if (p is null) return false;

        db.Productos.Remove(p);
        db.SaveChanges();
        return true;
    }

    // ── Métodos de Movimiento ────────────────────────────────────────────────
    public List<MovimientoDeProducto> TraerMovimientos(int productoId) =>
        db.Movimientos
           .Where(m => m.ProductoId == productoId)
           .OrderByDescending(m => m.Fecha)
           .ToList();

    public MovimientoDeProducto RegistrarMovimiento(int productoId, MovimientoDeProducto dto) {
        var producto = db.Productos.Find(productoId);
        
        if (producto is null) 
            throw new KeyNotFoundException("Producto no encontrado.");

        if (dto.Cantidad <= 0 && dto.Tipo != "Ajuste") 
            throw new ArgumentException("La cantidad debe ser siempre positiva.");

        var movimiento = new MovimientoDeProducto {
            ProductoId = productoId,
            Tipo = dto.Tipo,
            Cantidad = dto.Cantidad,
            Fecha = DateTime.Now
        };

        if (movimiento.Tipo == "Compra") {
            producto.Stock += movimiento.Cantidad;
        } 
        else if (movimiento.Tipo == "Venta") {
            if (producto.Stock < movimiento.Cantidad) {
                throw new ArgumentException("Stock insuficiente para realizar la venta.");
            }
            producto.Stock -= movimiento.Cantidad;
        } 
        else if (movimiento.Tipo == "Ajuste") {
            producto.Stock = movimiento.Cantidad;
        } 
        else {
            throw new ArgumentException("Tipo de movimiento inválido. Use: Compra, Venta o Ajuste.");
        }
        db.Movimientos.Add(movimiento);
        db.SaveChanges();

        return movimiento;
    }
}