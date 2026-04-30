class Persona {
    public string Nombre { get; }
    public string Apellido { get; }
    public int Edad { get; }

    public Persona(string nombre, string apellido, int edad) {
        Nombre = nombre;
        Apellido = apellido;
        Edad = edad;
    }
}

class PersonaService {
    private readonly IRepositorioPersonas _repositorio;

    public PersonaService(IRepositorioPersonas repositorio) {
        _repositorio = repositorio;
    }

    public void AgregarPersona(string nombre, string apellido, int edad) {
        Persona persona = new(nombre, apellido, edad);
        _repositorio.Guardar(persona);
    }
}

interface IRepositorioPersonas {
    Enumerable<Persona> ObtenerTodas();
    void Guardar(Persona persona);
}
