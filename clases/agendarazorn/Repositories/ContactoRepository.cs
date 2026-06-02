using AgendaRazorHtmx.Models;

namespace AgendaRazorHtmx.Repositories;

public class ContactoRepository : IContactoRepository {
    private readonly List<Contacto> contactos = [
        new Contacto {
            Id = 1,
            Nombre = "Juan",
            Apellido = "Perez",
            Telefono = "3815551111",
            Email = "juan@mail.com",
            Direccion = "San Miguel de Tucuman"
        },
        new Contacto {
            Id = 2,
            Nombre = "Ana",
            Apellido = "Gomez",
            Telefono = "3815552222",
            Email = "ana@mail.com",
            Direccion = "Yerba Buena"
        },
        new Contacto {
            Id = 3,
            Nombre = "Luis",
            Apellido = "Torres",
            Telefono = "3815553333",
            Email = "luis@mail.com",
            Direccion = "Tafi Viejo"
        }
    ];

    private int siguienteId = 4;

    public List<Contacto> ObtenerTodos() {
        return this.contactos;
    }

    public Contacto? ObtenerPorId(int id) {
        return this.contactos.FirstOrDefault(contacto => contacto.Id == id);
    }

    public Contacto Agregar(Contacto contacto) {
        contacto.Id = this.siguienteId;
        this.siguienteId++;

        this.contactos.Add(contacto);
        return contacto;
    }

    public void Actualizar(Contacto contactoActualizado) {
        Contacto? contacto = this.ObtenerPorId(contactoActualizado.Id);

        if (contacto is null) {
            return;
        }

        contacto.Nombre    = contactoActualizado.Nombre;
        contacto.Apellido  = contactoActualizado.Apellido;
        contacto.Telefono  = contactoActualizado.Telefono;
        contacto.Email     = contactoActualizado.Email;
        contacto.Direccion = contactoActualizado.Direccion;
    }

    public void Borrar(int id) {
        Contacto? contacto = this.ObtenerPorId(id);

        if (contacto is null) { return; }

        this.contactos.Remove(contacto);
    }
}