#!/usr/bin/env -S dotnet run

#:package Microsoft.EntityFrameworkCore.Sqlite@10.0.0
#:property PublishAot=false

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using static System.Console;

var dbPath = ResolverRutaDb();

Clear();
WriteLine("=== Carrito de compras con EF Core + SQLite ===");
WriteLine($"Base SQLite: {dbPath}");
WriteLine();

using var context = new TiendaContext(dbPath);

context.Database.EnsureDeleted();
context.Database.EnsureCreated();

var tienda = new TiendaService(context);

CargarCatalogo(tienda);
MostrarCatalogo(tienda.ListarCatalogo());

var carrito = tienda.CrearCarrito("Ada Lovelace");
WriteLine($"Carrito creado: #{carrito.Id} para {carrito.Cliente}");
WriteLine();

tienda.AgregarItem(carrito.Id,  "NOTE-15",  1);
tienda.AgregarItem(carrito.Id,  "MOUSE-WL", 2);
tienda.AgregarItem(carrito.Id,  "USB-C",    3);
tienda.EliminarItem(carrito.Id, "USB-C",    1);

var carritoActual = tienda.ObtenerCarrito(carrito.Id);
MostrarCarrito(carritoActual);

var carritoConfirmado = tienda.ConfirmarCompra(carrito.Id);
MostrarConfirmacion(carritoConfirmado);

return;

static string ResolverRutaDb([CallerFilePath] string sourceFile = "") {
	var directory = Path.GetDirectoryName(sourceFile) ?? Environment.CurrentDirectory;
	return Path.Combine(directory, "30.1-carrito.sqlite");
}

static void CargarCatalogo(TiendaService tienda) {
	tienda.CrearProducto("NOTE-15",  "Notebook 15 pulgadas",  1_250_000m);
	tienda.CrearProducto("MOUSE-WL", "Mouse inalámbrico",        28_500m);
	tienda.CrearProducto("USB-C",    "Hub USB-C",                41_990m);
	tienda.CrearProducto("MON-27",   "Monitor 27 pulgadas",     310_000m);
    tienda.CrearProducto("TECL-WL", "Teclado inalámbrico",       45_000m);
    tienda.CrearProducto("WEBCAM",   "Webcam Full HD",           85_000m);
    tienda.CrearProducto("AUR-WL",   "Auriculares inalámbricos", 60_000m);
}

static void MostrarCatalogo(IReadOnlyList<Producto> productos) {
	WriteLine("Catalogo disponible:");
	foreach (var producto in productos) {
		WriteLine($"- {producto.Codigo,-8} | {producto.Nombre,-24} | ${producto.Precio,10:0.00}");
	}
	WriteLine();
}

static void MostrarCarrito(Carrito carrito) {
	WriteLine($"Carrito #{carrito.Id} [{carrito.Estado}] - Cliente: {carrito.Cliente}");
	foreach (var item in carrito.Items.OrderBy(item => item.Producto.Nombre)) {
		WriteLine($"- {item.Producto.Nombre,-24} x {item.Cantidad,2} = ${item.Subtotal,10:0.00}");
	}
	WriteLine($"Total del carrito: ${carrito.Total:0.00}");
	WriteLine();
}

static void MostrarConfirmacion(Carrito carrito) {
	WriteLine($"Compra confirmada para el carrito #{carrito.Id}");
	WriteLine($"Cliente: {carrito.Cliente}");
	WriteLine($"Fecha UTC: {carrito.ConfirmadoUtc:yyyy-MM-dd HH:mm:ss}");
	WriteLine($"Total confirmado: ${carrito.Total:0.00}");
}

enum EstadoCarrito {
	Abierto    = 1,
	Confirmado = 2
}

class Producto {
	public int Id { get; set; }
	public string Codigo { get; set; } = string.Empty;
	public string Nombre { get; set; } = string.Empty;
	public decimal Precio { get; set; }
	public bool Activo { get; set; } = true;

	public ICollection<CarritoItem> ItemsCarrito { get; set; } = [];
}

class Carrito {
	public int Id { get; set; }
	public string Cliente { get; set; } = string.Empty;
	public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
	public DateTime? ConfirmadoUtc { get; set; }
	public EstadoCarrito Estado { get; set; } = EstadoCarrito.Abierto;

	public ICollection<CarritoItem> Items { get; set; } = [];

	[NotMapped]
	public decimal Total => Items.Sum(item => item.Subtotal);
}

class CarritoItem {
	public int Id { get; set; }
	public int CarritoId { get; set; }
	public int ProductoId { get; set; }
	public int Cantidad { get; set; }
	public decimal PrecioUnitario { get; set; }

	public Carrito Carrito { get; set; } = null!;
	public Producto Producto { get; set; } = null!;

	[NotMapped]
	public decimal Subtotal => PrecioUnitario * Cantidad;
}

class TiendaService(TiendaContext context) {
	public Producto CrearProducto(string codigo, string nombre, decimal precio) {
		if (precio <= 0) {
			throw new ArgumentOutOfRangeException(nameof(precio), "El precio debe ser mayor a cero.");
		}

		var codigoNormalizado = NormalizarCodigo(codigo);
		var nombreNormalizado = Requerido(nombre, nameof(nombre), "El nombre del producto es obligatorio.");

		var existente = context.Productos.SingleOrDefault(producto => producto.Codigo == codigoNormalizado);
		if (existente is not null) {
			return existente;
		}

		var producto = new Producto {
			Codigo = codigoNormalizado,
			Nombre = nombreNormalizado,
			Precio = precio
		};

		context.Productos.Add(producto);
		context.SaveChanges();

		return producto;
	}

	public IReadOnlyList<Producto> ListarCatalogo() {
		return context.Productos
			.AsNoTracking()
			.OrderBy(producto => producto.Nombre)
			.ToList();
	}

	public Carrito CrearCarrito(string cliente) {
		var carrito = new Carrito {
			Cliente = Requerido(cliente, nameof(cliente), "El cliente es obligatorio.")
		};

		context.Carritos.Add(carrito);
		context.SaveChanges();

		return carrito;
	}

	public Carrito ObtenerCarrito(int carritoId) {
		var carrito = context.Carritos
			.Include(carrito => carrito.Items)
			.ThenInclude(item => item.Producto)
			.SingleOrDefault(carrito => carrito.Id == carritoId);

		return carrito ?? throw new InvalidOperationException($"No existe el carrito #{carritoId}.");
	}

	public void AgregarItem(int carritoId, string codigoProducto, int cantidad) {
		if (cantidad <= 0) {
			throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad debe ser mayor a cero.");
		}

		var carrito = ObtenerCarrito(carritoId);
		ValidarCarritoAbierto(carrito);

		var producto = ObtenerProductoPorCodigo(codigoProducto);

		var existente = carrito.Items.SingleOrDefault(item => item.ProductoId == producto.Id);
		if (existente is null) {
			carrito.Items.Add(new CarritoItem {
				ProductoId = producto.Id,
				Cantidad = cantidad,
				PrecioUnitario = producto.Precio
			});
		} else {
			existente.Cantidad += cantidad;
		}

		context.SaveChanges();
	}

	public void EliminarItem(int carritoId, string codigoProducto, int cantidad) {
		if (cantidad <= 0) {
			throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad debe ser mayor a cero.");
		}

		var carrito = ObtenerCarrito(carritoId);
		ValidarCarritoAbierto(carrito);

		var producto = ObtenerProductoPorCodigo(codigoProducto);
		var item = carrito.Items.SingleOrDefault(item => item.ProductoId == producto.Id);
		if (item is null) {
			throw new InvalidOperationException($"El carrito #{carritoId} no contiene el producto {producto.Codigo}.");
		}

		if (item.Cantidad <= cantidad) {
			context.CarritoItems.Remove(item);
		} else {
			item.Cantidad -= cantidad;
		}

		context.SaveChanges();
	}

	public Carrito ConfirmarCompra(int carritoId) {
		var carrito = ObtenerCarrito(carritoId);
		ValidarCarritoAbierto(carrito);

		if (!carrito.Items.Any()) {
			throw new InvalidOperationException("No se puede confirmar una compra con el carrito vacio.");
		}

		carrito.Estado = EstadoCarrito.Confirmado;
		carrito.ConfirmadoUtc = DateTime.UtcNow;

		context.SaveChanges();

		return ObtenerCarrito(carritoId);
	}

	Producto ObtenerProductoPorCodigo(string codigo) {
		var codigoNormalizado = NormalizarCodigo(codigo);
		var producto = context.Productos.SingleOrDefault(producto => producto.Codigo == codigoNormalizado);

		return producto ?? throw new InvalidOperationException($"No existe el producto {codigoNormalizado}.");
	}

	static void ValidarCarritoAbierto(Carrito carrito) {
		if (carrito.Estado != EstadoCarrito.Abierto) {
			throw new InvalidOperationException("El carrito ya fue confirmado y no admite cambios.");
		}
	}

	static string NormalizarCodigo(string codigo) {
		return Requerido(codigo, nameof(codigo), "El codigo del producto es obligatorio.").ToUpperInvariant();
	}

	static string Requerido(string valor, string parametro, string mensaje) {
		if (string.IsNullOrWhiteSpace(valor)) {
			throw new ArgumentException(mensaje, parametro);
		}

		return valor.Trim();
	}
}

class TiendaContext(string dbPath) : DbContext {
	public DbSet<Producto> Productos => Set<Producto>();
	public DbSet<Carrito> Carritos => Set<Carrito>();
	public DbSet<CarritoItem> CarritoItems => Set<CarritoItem>();

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		optionsBuilder.UseSqlite($"Data Source={dbPath}");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<Producto>(entity => {
			entity.ToTable("Productos");
			entity.HasKey(producto => producto.Id);
			entity.HasIndex(producto => producto.Codigo).IsUnique();
			entity.Property(producto => producto.Codigo).IsRequired().HasMaxLength(20);
			entity.Property(producto => producto.Nombre).IsRequired().HasMaxLength(120);
			entity.Property(producto => producto.Precio).HasPrecision(10, 2);
		});

		modelBuilder.Entity<Carrito>(entity => {
			entity.ToTable("Carritos");
			entity.HasKey(carrito => carrito.Id);
			entity.Property(carrito => carrito.Cliente).IsRequired().HasMaxLength(120);
			entity.Property(carrito => carrito.Estado).HasConversion<string>().HasMaxLength(20);
		});

		modelBuilder.Entity<CarritoItem>(entity => {
			entity.ToTable("CarritoItems");
			entity.HasKey(item => item.Id);
			entity.HasIndex(item => new { item.CarritoId, item.ProductoId }).IsUnique();
			entity.Property(item => item.PrecioUnitario).HasPrecision(10, 2);
			entity.HasOne(item => item.Carrito)
				.WithMany(carrito => carrito.Items)
				.HasForeignKey(item => item.CarritoId)
				.OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(item => item.Producto)
				.WithMany(producto => producto.ItemsCarrito)
				.HasForeignKey(item => item.ProductoId)
				.OnDelete(DeleteBehavior.Restrict);
		});
	}
}
