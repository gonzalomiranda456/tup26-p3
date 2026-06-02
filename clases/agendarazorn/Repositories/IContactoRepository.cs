using AgendaRazorHtmx.Models;

namespace AgendaRazorHtmx.Repositories;

public interface IContactoRepository {
    List<Contacto> ObtenerTodos();
    Contacto? ObtenerPorId(int id);
    Contacto Agregar(Contacto contacto);
    void Actualizar(Contacto contacto);
    void Borrar(int id);
}