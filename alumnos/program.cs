namespace Tup26.AlumnosApp;

class Program {
    static int Main(string[] args) {
        Alumnos alumnos = AlumnosManager.Leer();

        if (args.Length == 0 || EsComando(args[0], "ayuda", "help", "-h", "--help")) {
            MostrarAyuda();
            return 0;
        }

        string comando = args[0].Trim().ToLowerInvariant();

        switch (comando) {
            case "listar":
                AlumnosManager.Listar(alumnos);
                return 0;

            case "sin-github":
                AlumnosManager.Listar(alumnos.ConGithub(false), "Alumnos sin GitHub");
                return 0;

            case "sin-telefono":
                AlumnosManager.Listar(alumnos.ConTelefono(false), "Alumnos sin telefono");
                return 0;

            case "sin-foto":
                AlumnosManager.Listar(alumnos.ConFotos(false), "Alumnos sin foto");
                return 0;

            case "guardar":
                AlumnosManager.Escribir(alumnos, ObtenerRuta(args, 1, AppPaths.ArchivoAlumnos));
                return 0;

            case "json":
                AlumnosManager.EscribirJSON(alumnos, ObtenerRuta(args, 1, AppPaths.ArchivoJsonAlumnos));
                return 0;

            case "vcf":
                AlumnosManager.EscribirVCard(alumnos, ObtenerRuta(args, 1, AppPaths.ArchivoVcf));
                return 0;

            case "crear-carpetas":
                AlumnosManager.CrearCarpetas(alumnos);
                return 0;

            case "prs":
                RevisarPullRequests(alumnos);
                return 0;

            case "normalizar-prs":
                new GitHub().NormalizarTitulos(alumnos, simular: args.Contains("--simular"));
                return 0;

            case "wapp":
                WAppService wapp = new();
                foreach(var grupo in wapp.Grupos()) {
                    Console.WriteLine($"Grupo: {grupo.Group}");
                    foreach(var contacto in wapp.Participantes(grupo.Group)) {
                        Console.WriteLine($"  - {contacto.Name,-30} {contacto.PhoneNumber}");
                    }
                }
                wapp.Enviar("prueba", $"Hola, este es un mensaje de prueba desde la aplicación de alumnos. Enviado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}.", rutaArchivo: "/Users/adibattista/Documents/GitHub/tup26-p3/practicos/63174 - Jerez, Luciano Germán/TP1/enunciado.md");
                return 0;

            default:
                Log.Error($"Comando desconocido: {args[0]}");
                MostrarAyuda();
                return 1;
        }
    }

    static bool EsComando(string valor, params string[] opciones) =>
        opciones.Any(opcion => string.Equals(valor, opcion, StringComparison.OrdinalIgnoreCase));

    static string ObtenerRuta(string[] args, int index, string rutaPorDefecto) =>
        args.Length > index ? args[index] : rutaPorDefecto;

    static void MostrarAyuda() {
        Console.WriteLine("""
        Uso:
          dotnet run -- listar
          dotnet run -- sin-github
          dotnet run -- sin-telefono
          dotnet run -- sin-foto
          dotnet run -- guardar [archivo.md]
          dotnet run -- json [archivo.json]
          dotnet run -- vcf [archivo.vcf]
          dotnet run -- crear-carpetas
          dotnet run -- prs
          dotnet run -- normalizar-prs [--simular]
        """);
    }

    static void RevisarPullRequests(Alumnos alumnos) {
        GitHub gh = new();

        foreach (var pr in gh.PullRequests()) {
            int legajo = GitHub.ExtraerLegajo(pr.Titulo);
            Alumno? alumno = alumnos.BuscarPorLegajo(legajo);

            if (alumno is null) {
                Console.WriteLine($"Alumno con legajo {legajo} no encontrado en la lista de alumnos.");
                continue;
            }

            var detallePr = gh.ObtenerEstado(pr.Numero);
            int cantidadArchivos = gh.ListarArchivos(pr.Numero).Count;
            int cantidadCommits = gh.Commits(pr.Numero).Count;
            string estado = detallePr.Estado == "open" ? "abierto" : detallePr.Estado == "closed" ? "cerrado" : "sin dato";
            string mergeable = detallePr.EsMergeable ? "mergeable" : "con conflictos";
            int tp = GitHub.ExtraerTP(pr.Titulo);

            Console.ForegroundColor = detallePr.EsMergeable ? ConsoleColor.Green : ConsoleColor.Red;
            Console.BackgroundColor = cantidadArchivos < 10 ? ConsoleColor.Black : ConsoleColor.DarkRed;
            Console.WriteLine($"PR #{pr.Numero:000} | {legajo} | {alumno.NombreCompleto,-40} | A:{cantidadArchivos,4} | C:{cantidadCommits,2} | {estado} | {mergeable,-13} | TP{tp}");
            Console.ResetColor();
        }
    }
}
