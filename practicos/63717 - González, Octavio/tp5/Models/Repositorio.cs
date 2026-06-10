namespace tp5.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

class Repositorio
{
    private readonly ContactoDb db;
    public Repositorio(ContactoDb db) => this.db = db;

    public void Iniciar() => db.Database.EnsureCreated();

    public List<Contacto> TraerContactos() =>
        db.Contactos
        .OrderBy(p => p.Id)
        .ToList();

    public Contacto? TraerContacto(int id) =>
        db.Contactos
        .FirstOrDefault(p => p.Id == id);

    async public Task<Contacto> AgregarContacto(Contacto contacto)
    {

        db.Contactos.Add(contacto);
        await db.SaveChangesAsync();
        return contacto;
    }
    async public Task<Contacto?> Actualizar(int id, Contacto actualizacion)
    {
        var cambio = db.Contactos.FirstOrDefault(p => p.Id == id);

        if (cambio is null) return null;
        cambio.Nombre = actualizacion.Nombre;
        cambio.Apellido = actualizacion.Apellido;
        cambio.Telefono = actualizacion.Telefono;
        cambio.Email = actualizacion.Email;
        cambio.Empresa = actualizacion.Empresa;
        cambio.Cargo = actualizacion.Cargo;
        cambio.Direccion = actualizacion.Direccion;
        cambio.Notas = actualizacion.Notas;
        await db.SaveChangesAsync();
        return cambio;
    }

    async public Task<bool> Eliminar(int id)
    {
        var eliminado = db.Contactos.FirstOrDefault(p => p.Id == id);
        if (eliminado is null) return false;
        db.Contactos.Remove(eliminado);
        await db.SaveChangesAsync();
        return true;
    }




}

