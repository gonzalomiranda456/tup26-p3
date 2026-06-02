using AgendaRazorHtmx.Dtos;
using AgendaRazorHtmx.Models;

namespace AgendaRazorHtmx.Services;

public interface IContactoService {
    List<Contacto> Buscar(string? texto);
    Contacto? ObtenerDetalle(int id);
    EditarContactoDto? ObtenerParaEditar(int id);
    Contacto Crear(CrearContactoDto dto);
    Contacto? Actualizar(EditarContactoDto dto);
    void Borrar(int id);
}