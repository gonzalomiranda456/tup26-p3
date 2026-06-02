using AgendaRazorHtmx.Dtos;
using AgendaRazorHtmx.Models;
using AgendaRazorHtmx.Repositories;

namespace AgendaRazorHtmx.Services;

public class ContactoService : IContactoService {
    private readonly IContactoRepository contactoRepository;

    public ContactoService(IContactoRepository contactoRepository) {
        this.contactoRepository = contactoRepository;
    }

    public List<Contacto> Buscar(string? texto) {
        List<Contacto> contactos = this.contactoRepository.ObtenerTodos();

        if (string.IsNullOrWhiteSpace(texto)) { return contactos; }

        string textoNormalizado = texto.Trim().ToLower();

        return contactos
            .Where(contacto =>
                contacto.Nombre.ToLower().Contains(textoNormalizado) ||
                contacto.Apellido.ToLower().Contains(textoNormalizado) ||
                contacto.Telefono.ToLower().Contains(textoNormalizado) ||
                contacto.Email.ToLower().Contains(textoNormalizado) ||
                contacto.Direccion.ToLower().Contains(textoNormalizado))
            .ToList();
    }

    public Contacto? ObtenerDetalle(int id) {
        return this.contactoRepository.ObtenerPorId(id);
    }

    public EditarContactoDto? ObtenerParaEditar(int id) {
        Contacto? contacto = this.contactoRepository.ObtenerPorId(id);

        if (contacto is null) { return null; }

        return new EditarContactoDto(
            contacto.Id,
            contacto.Nombre,
            contacto.Apellido,
            contacto.Telefono,
            contacto.Email,
            contacto.Direccion
        );
    }

    public Contacto Crear(CrearContactoDto dto) {
        Contacto contacto = new() {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Direccion = dto.Direccion
        };

        return this.contactoRepository.Agregar(contacto);
    }

    public Contacto? Actualizar(EditarContactoDto dto) {
        Contacto? contacto = this.contactoRepository.ObtenerPorId(dto.Id);

        if (contacto is null) { return null; }

        Contacto contactoActualizado = new() {
            Id = dto.Id,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Direccion = dto.Direccion
        };

        this.contactoRepository.Actualizar(contactoActualizado);
        return this.contactoRepository.ObtenerPorId(dto.Id);
    }

    public void Borrar(int id) {
        this.contactoRepository.Borrar(id);
    }
}