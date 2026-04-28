namespace Tup26.AlumnosApp;

class Program {
    static int Main(string[] args) {
        Alumnos alumnos = AlumnosManager.Leer();
        Console.WriteLine($"Alumnos cargados: {alumnos.Count()}");
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

            case "bajar-prs":
                BajarPullRequests(args);
                return 0;

            case "cerrar-prs":
                GitHub gh = new();
                gh.CerrarPRsAbiertos();
                return 0;

            case "revisar-presentados":
                RevisarPresentados(alumnos, args);
                return 0;

            case "registrar-asistencias":
                var contar = 0;
                foreach (Alumno alumno in alumnos) {
                    if(alumno.Presente) {
                        alumno.Asistencias++;
                        contar++;
                    }
                    alumno.Presente = false;
                }
                AlumnosManager.Escribir(alumnos, AppPaths.ArchivoAlumnos);
                Console.WriteLine($"Asistencias registradas: {contar}");
                return 0;
                
            case "relevar-asistencias":
                CargarAsistenciasHoy(alumnos);
                AlumnosManager.Escribir(alumnos, AppPaths.ArchivoAlumnos);
                return 0;
            
            case "wapp-grupos":
                WAppService wapp = new();
                foreach(var grupo in wapp.Grupos()) {
                    Console.WriteLine($"Grupo: {grupo.Group}");
                    foreach(var contacto in wapp.Participantes(grupo.Group)) {
                        Console.WriteLine($"  - {contacto.Name,-30} {contacto.PhoneNumber} {contacto.Jid}");
                    }
                }
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
            dotnet run -- bajar-prs TP1 [--forzar]
            dotnet run -- cerrar-prs TP1
            dotnet run -- revisar-presentados TP1
            dotnet run -- wapp-grupos
            dotnet run -- wapp-asistencias
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
            int cantidadLineas   = gh.CantidadLineas(pr.Numero);
            int cantidadCommits  = gh.Commits(pr.Numero).Count;
            string estado    = detallePr.Estado == "open" ? "abierto" : detallePr.Estado == "closed" ? "cerrado" : "sin dato";
            string mergeable = detallePr.EsMergeable ? "mergeable" : "con conflictos";
            int tp = GitHub.ExtraerTP(pr.Titulo);
            string carpetaTp = tp > 0 ? $"tp{tp}" : string.Empty;
            List<string> archivosTp = string.IsNullOrWhiteSpace(carpetaTp)
                ? new()
                : gh.ListarArchivosDirectorio(pr.Numero, alumno.CarpetaNombre, carpetaTp);

            Console.ForegroundColor = detallePr.EsMergeable ? ConsoleColor.Green : ConsoleColor.Red;
            Console.BackgroundColor = cantidadArchivos < 10 ? ConsoleColor.Black : ConsoleColor.DarkRed;
            Console.WriteLine($"PR #{pr.Numero:000} | {legajo} | {alumno.NombreCompleto,-40} | A:{cantidadArchivos,4} | L:{cantidadLineas,4} | C:{cantidadCommits,2} | {estado} | {mergeable,-15} | TP{tp}");
            foreach (string archivo in archivosTp) {
                Console.WriteLine($"  - {archivo}");
            }
            Console.ResetColor();
        }
    }

    static void BajarPullRequests(string[] args) {
        if (args.Length < 2) {
            Log.Error("Debe indicar el trabajo práctico a bajar. Ejemplo: bajar-prs TP1");
            return;
        }

        int numeroTp = ObtenerNumeroTP(args[1]);
        if (numeroTp <= 0) {
            Log.Error($"No se pudo interpretar el trabajo práctico '{args[1]}'. Use un valor como TP1 o 1.");
            return;
        }

        bool forzar = args.Any(arg => string.Equals(arg, "--forzar", StringComparison.OrdinalIgnoreCase));
        GitHub gh = new();
        IEnumerable<(int Numero, string Titulo)> prs = gh.PullRequests().Where(pr => GitHub.ExtraerTP(pr.Titulo) == numeroTp);

        foreach (var pr in prs) {
            gh.BajarArchivosAlumno(pr.Numero, forzar);
        }
    }

    static void CargarAsistenciasHoy(Alumnos alumnos) {
        WAppService wapp = new();
        DateTime hoy = DateTime.Today;

        DateTime desde = hoy.AddHours(8);
        DateTime hasta = hoy.AddHours(12);
    
        foreach(var grupo in new[] { "C7", "C9" }) {
            foreach (var mensaje in wapp.Mensajes(grupo, desde, hasta)) {
                string telefono = wapp.ObtenerTelefonoAutorMensaje(mensaje);
                alumnos.BuscarPorTelefono(telefono)?.Presente = true;
            }
        }
    }

    static void RevisarPresentados(Alumnos alumnos, string[] args) {
        if (args.Length < 2) {
            Log.Error("Debe indicar el trabajo práctico a revisar. Ejemplo: revisar-presentados TP1");
            return;
        }

        int numeroTp = ObtenerNumeroTP(args[1]);
        if (numeroTp <= 0) {
            Log.Error($"No se pudo interpretar el trabajo práctico '{args[1]}'. Use un valor como TP1 o 1.");
            return;
        }

        const int minimoLineasAgregadas = 100;
        string carpetaTp = $"tp{numeroTp}";
        string rutaEnunciado = AppPaths.EnunciadoPracticoDirectory(carpetaTp);
        int lineasEnunciado  = AppPaths.ContarLineasArchivos(rutaEnunciado, "*.cs");

        Log.Info($"{carpetaTp.ToUpperInvariant()} | líneas base del enunciado: {lineasEnunciado}");
        int marcados = 0;

        foreach (Alumno alumno in alumnos.OrderBy(alumno => alumno.Legajo)) {
            string rutaPractico = AppPaths.PracticoAlumnoSubdirectory(alumno, carpetaTp);
            int lineasTotales   = ContarLineasPracticoLocal(rutaPractico);
            int lineasAgregadas = Math.Max(0, lineasTotales - lineasEnunciado);

            Estado estado = Estado.Desaprobado;
            if (lineasAgregadas >= minimoLineasAgregadas) {
                estado = Estado.Aprobado;
                marcados++;
            }
            alumno.Practico(numeroTp, estado);
            Log.Info($"{alumno.Legajo} | {alumno.NombreCompleto,-40} | L:{lineasTotales,4} | L+:{lineasAgregadas,4} | marcado    {estado.ToEmoji()}");
        }

        if (marcados > 0) {
            AlumnosManager.Escribir(alumnos, AppPaths.ArchivoAlumnos);
        }

        Log.Info($"Resumen TP{numeroTp}: marcados={marcados}, total={alumnos.Count()}, porcentaje={marcados * 100.0 / alumnos.Count():F2}%");
    }

    static int ContarLineasPracticoLocal(string rutaPractico) {
        return AppPaths.ContarLineasArchivos(rutaPractico, "*.cs", SearchOption.TopDirectoryOnly);
    }

    static int ObtenerNumeroTP(string valor) {
        int numeroTp = GitHub.ExtraerTP(valor);
        if (numeroTp == 0 && int.TryParse(valor, out int tpDesdeNumero)) {
            numeroTp = tpDesdeNumero;
        }

        return numeroTp;
    }
}