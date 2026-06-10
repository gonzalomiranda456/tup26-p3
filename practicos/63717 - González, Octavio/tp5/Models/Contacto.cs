namespace tp5.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.ComponentModel.DataAnnotations;

public class Contacto

{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; } = "";
    [Required]
    public string Apellido { get; set; } = "";
    [Required]
    [MaxLength(20)]
    public string Telefono { get; set; } = "";
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    public string Empresa { get; set; } = "";

    public string Cargo { get; set; } = "";

    public string Direccion { get; set; } = "";
  
    public DateOnly? FechaNacimiento { get; set; }
    public string Notas { get; set; } = "";
}
//DB CONTEXT. 
class ContactoDb : DbContext
{
    public ContactoDb(DbContextOptions<ContactoDb> options) : base(options) { }
    public DbSet<Contacto> Contactos => Set<Contacto>();
}