namespace AgendaRazorHtmx.Dtos;

public record CrearContactoDto(
    string Nombre,
    string Apellido,
    string Telefono,
    string Email,
    string Direccion
);