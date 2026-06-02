namespace AgendaRazorHtmx.Dtos;

public record EditarContactoDto(
    int Id,
    string Nombre,
    string Apellido,
    string Telefono,
    string Email,
    string Direccion
);