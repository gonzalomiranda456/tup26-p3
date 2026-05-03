namespace Tup26.AlumnosApp;

readonly record struct CopiaRuta(string Origen, string Destino);
readonly record struct PerfilMarkdown(string Ruta, string[] Lineas);

static class AppPaths {
    static readonly string dataDirectory = ResolverDirectorioDatos();

    public static string DataDirectory => dataDirectory;
    public static string RepoRoot => Directory.GetParent(DataDirectory)?.FullName ?? DataDirectory;
    public static string ArchivoAlumnos => Path.Combine(DataDirectory, "alumnos.md");
    public static string ArchivoVcf => Path.Combine(DataDirectory, "alumnos.vcf");
    public static string ArchivoReadmeRepo => Path.Combine(RepoRoot, "README.md");
    public static string PracticosDirectory => Path.Combine(RepoRoot, "practicos");
    public static string EnunciadosDirectory => Path.Combine(RepoRoot, "enunciados");
    public static string ArchivoJsonAlumnos => Path.Combine(DataDirectory, "alumnos.json");

    public static string EnunciadoPracticoDirectory(string practico) =>
        Path.Combine(EnunciadosDirectory, practico);

    public static string PracticoAlumnoDirectory(Alumno alumno) =>
        Path.Combine(PracticosDirectory, alumno.CarpetaNombre);

    public static string PracticoAlumnoDirectory(string rutaBase, Alumno alumno) =>
        Path.Combine(rutaBase, alumno.CarpetaNombre);

    public static string PracticoAlumnoSubdirectory(Alumno alumno, string subdirectorio) =>
        Path.Combine(PracticoAlumnoDirectory(alumno), subdirectorio);

    public static string PracticoAlumnoSubdirectory(string rutaBase, Alumno alumno, string subdirectorio) =>
        Path.Combine(PracticoAlumnoDirectory(rutaBase, alumno), subdirectorio);

    public static string FotoPerfilOrigen(string rutaFotos, Alumno alumno) =>
        Path.Combine(rutaFotos, alumno.TelefonoId, "foto.png");

    public static string FotoAlumnoDestino(string rutaCarpetaAlumno) =>
        Path.Combine(rutaCarpetaAlumno, "foto.png");

    public static string WacliStoreDirectory(string? store) {
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (string.IsNullOrWhiteSpace(store)) {
            return Path.Combine(home, ".wacli");
        }

        if (store == "~") {
            return home;
        }

        if (store.StartsWith("~/", StringComparison.Ordinal)) {
            return Path.Combine(home, store.Substring(2));
        }

        return Environment.ExpandEnvironmentVariables(store);
    }

    public static string WacliDatabase(string? store) =>
        Path.Combine(WacliStoreDirectory(store), "wacli.db");

    public static string WacliSessionDatabase(string? store) =>
        Path.Combine(WacliStoreDirectory(store), "session.db");

    public static string[] LeerLineas(string rutaArchivo, Encoding? encoding = null) =>
        File.ReadAllLines(rutaArchivo, encoding ?? Encoding.UTF8);

    public static string[] LeerLineasAlumnos() =>
        LeerLineas(ArchivoAlumnos);

    public static void EscribirTexto(string rutaArchivo, string contenido, Encoding? encoding = null) {
        File.WriteAllText(rutaArchivo, contenido, encoding ?? Encoding.UTF8);
    }

    public static void EscribirAlumnosMarkdown(string contenido, string? rutaArchivo = null) {
        string destino = string.IsNullOrWhiteSpace(rutaArchivo) ? ArchivoAlumnos : rutaArchivo;
        EscribirTexto(destino, contenido, Encoding.UTF8);
    }

    public static void EscribirAlumnosJson(string contenido, string? rutaArchivo = null) {
        string destino = string.IsNullOrWhiteSpace(rutaArchivo) ? ArchivoJsonAlumnos : rutaArchivo;
        EscribirTexto(destino, contenido, new UTF8Encoding(false));
    }

    public static void EscribirAlumnosVCard(string contenido, string? rutaArchivo = null) {
        string destino = string.IsNullOrWhiteSpace(rutaArchivo) ? ArchivoVcf : rutaArchivo;
        EscribirTexto(destino, contenido, new UTF8Encoding(false));
    }

    public static bool ExisteArchivo(string rutaArchivo) =>
        File.Exists(rutaArchivo);

    public static string ResolverArchivo(string rutaArchivo) {
        if (string.IsNullOrWhiteSpace(rutaArchivo)) {
            return string.Empty;
        }

        string ruta = rutaArchivo.Trim();

        if (ruta == "~") {
            ruta = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        } else if (ruta.StartsWith("~/", StringComparison.Ordinal)) {
            ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ruta.Substring(2));
        } else {
            ruta = Environment.ExpandEnvironmentVariables(ruta);
        }

        return Path.GetFullPath(ruta);
    }

    public static string NombreArchivo(string rutaArchivo) =>
        Path.GetFileName(rutaArchivo);

    public static string ExtensionArchivo(string rutaArchivo) =>
        Path.GetExtension(rutaArchivo).ToLowerInvariant();

    public static bool ExisteDirectorio(string rutaDirectorio) =>
        Directory.Exists(rutaDirectorio);

    public static void AsegurarDirectorio(string rutaDirectorio) {
        Directory.CreateDirectory(rutaDirectorio);
    }

    public static string[] ListarArchivos(string rutaDirectorio) {
        if (!ExisteDirectorio(rutaDirectorio)) {
            return Array.Empty<string>();
        }

        return Directory.GetFiles(rutaDirectorio);
    }

    public static int ContarLineasArchivos(string rutaCarpeta, string patronArchivo, SearchOption opcionBusqueda = SearchOption.TopDirectoryOnly) {
        if (!ExisteDirectorio(rutaCarpeta)) {
            return 0;
        }

        int total = 0;
        foreach (string rutaArchivo in Directory.EnumerateFiles(rutaCarpeta, patronArchivo, opcionBusqueda)) {
            using StreamReader reader = new(rutaArchivo);
            while (reader.ReadLine() is not null) {
                total++;
            }
        }

        return total;
    }

    public static string[] ListarDirectorios(string rutaDirectorio) {
        if (!ExisteDirectorio(rutaDirectorio)) {
            return Array.Empty<string>();
        }

        return Directory.GetDirectories(rutaDirectorio);
    }

    public static void AsegurarDirectorioPracticos() {
        AsegurarDirectorio(PracticosDirectory);
    }

    public static bool ExisteDirectorioPracticos() =>
        ExisteDirectorio(PracticosDirectory);

    public static List<string> LimpiarDirectoriosCompilacionPracticos() {
        List<string> directorios = BuscarDirectoriosCompilacion(PracticosDirectory);

        foreach (string directorio in directorios) {
            Directory.Delete(directorio, recursive: true);
        }

        return directorios;
    }

    public static bool ExisteEnunciadoPractico(string practico) =>
        ExisteDirectorio(EnunciadoPracticoDirectory(practico));

    public static string RutaRelativaDesdePracticos(string ruta) =>
        Path.GetRelativePath(PracticosDirectory, ruta);

    public static string RutaCarpetaAlumnoEsperada(Alumno alumno) =>
        PracticoAlumnoDirectory(alumno);

    public static void AsegurarCarpetaAlumno(Alumno alumno) {
        AsegurarDirectorio(PracticoAlumnoDirectory(alumno));
    }

    public static bool ExisteCarpetaAlumno(string? rutaCarpetaAlumno) =>
        !string.IsNullOrWhiteSpace(rutaCarpetaAlumno) && ExisteDirectorio(rutaCarpetaAlumno);

    public static List<string> BuscarCarpetasMismoLegajo(int legajo) =>
        BuscarCarpetasMismoLegajo(PracticosDirectory, legajo);

    public static string? ObtenerCarpetaUnicaMismoLegajo(int legajo) =>
        ObtenerCarpetaUnicaMismoLegajo(PracticosDirectory, legajo);

    public static void RenombrarCarpetaAlumno(string origen, Alumno alumno) {
        RenombrarCarpeta(origen, PracticoAlumnoDirectory(alumno));
    }

    public static bool ExisteFotoPerfil(string rutaFotos, Alumno alumno) =>
        ExisteArchivo(FotoPerfilOrigen(rutaFotos, alumno));

    public static bool ExisteFotoAlumno(string rutaCarpetaAlumno) =>
        ExisteArchivo(FotoAlumnoDestino(rutaCarpetaAlumno));

    public static CopiaRuta CopiarFotoPerfil(string rutaFotos, Alumno alumno, string rutaCarpetaAlumno) {
        string origen = FotoPerfilOrigen(rutaFotos, alumno);
        string destino = FotoAlumnoDestino(rutaCarpetaAlumno);
        File.Copy(origen, destino);
        return new(origen, destino);
    }

    public static CopiaRuta CopiarEnunciadoPractico(Alumno alumno, string practico, bool forzar = false) {
        string nombrePractico = practico.Trim();
        string carpetaPractico = nombrePractico.ToLower();
        string origen = EnunciadoPracticoDirectory(nombrePractico);
        string destino = PracticoAlumnoSubdirectory(alumno, carpetaPractico);

        CopiarCarpeta(origen, destino, forzar);
        return new(origen, destino);
    }

    public static IEnumerable<PerfilMarkdown> LeerPerfilesMarkdown(string rutaPerfiles) {
        if (!ExisteDirectorio(rutaPerfiles)) {
            yield break;
        }

        foreach (string carpetaPerfil in ListarDirectorios(rutaPerfiles)) {
            string rutaPerfil = PerfilMarkdownPath(carpetaPerfil);
            if (!ExisteArchivo(rutaPerfil)) {
                continue;
            }

            yield return new(rutaPerfil, LeerLineas(rutaPerfil));
        }
    }

    public static string ArchivoDescargado(string rutaDestino, string nombreRemoto) =>
        Path.Combine(rutaDestino, Path.GetFileName(nombreRemoto));

    public static bool ExisteArchivoDescargado(string rutaDestino, string nombreRemoto) =>
        ExisteArchivo(ArchivoDescargado(rutaDestino, nombreRemoto));

    public static string GuardarArchivoDescargado(string rutaDestino, string nombreRemoto, byte[] contenido, bool forzar = false) {
        AsegurarDirectorio(rutaDestino);

        string rutaArchivo = ArchivoDescargado(rutaDestino, nombreRemoto);
        if (!forzar && ExisteArchivo(rutaArchivo)) {
            return rutaArchivo;
        }

        File.WriteAllBytes(rutaArchivo, contenido);
        return rutaArchivo;
    }

    public static string GuardarArchivoDescargadoRelativo(string rutaDestinoBase, string rutaRelativa, byte[] contenido, bool forzar = false) {
        string rutaNormalizada = rutaRelativa.Replace('\\', Path.DirectorySeparatorChar)
                                            .Replace('/', Path.DirectorySeparatorChar)
                                            .TrimStart(Path.DirectorySeparatorChar);

        if (string.IsNullOrWhiteSpace(rutaNormalizada)) {
            throw new IOException("La ruta relativa del archivo descargado no puede estar vacía.");
        }

        string rutaCompleta = Path.GetFullPath(Path.Combine(rutaDestinoBase, rutaNormalizada));
        string rutaDestinoNormalizada = Path.GetFullPath(rutaDestinoBase);

        if (!rutaCompleta.StartsWith(rutaDestinoNormalizada, StringComparison.Ordinal)) {
            throw new IOException($"La ruta relativa '{rutaRelativa}' es inválida para el destino '{rutaDestinoBase}'.");
        }

        string? directorio = Path.GetDirectoryName(rutaCompleta);
        if (!string.IsNullOrWhiteSpace(directorio)) {
            AsegurarDirectorio(directorio);
        }

        if (!forzar && ExisteArchivo(rutaCompleta)) {
            return rutaCompleta;
        }

        File.WriteAllBytes(rutaCompleta, contenido);
        return rutaCompleta;
    }

    public static List<string> BuscarCarpetasMismoLegajo(string rutaBase, int legajo) {
        List<string> carpetasMismoLegajo = new();

        if (!ExisteDirectorio(rutaBase)) {
            return carpetasMismoLegajo;
        }

        foreach (string carpetaExistente in ListarDirectorios(rutaBase)) {
            string nombreCarpetaExistente = Path.GetFileName(carpetaExistente);

            if (TieneMismoLegajo(nombreCarpetaExistente, legajo)) {
                carpetasMismoLegajo.Add(carpetaExistente);
            }
        }

        return carpetasMismoLegajo;
    }

    public static string? ObtenerCarpetaUnicaMismoLegajo(string rutaBase, int legajo) {
        List<string> carpetasMismoLegajo = BuscarCarpetasMismoLegajo(rutaBase, legajo);

        if (carpetasMismoLegajo.Count != 1) {
            return null;
        }

        return carpetasMismoLegajo[0];
    }

    public static void RenombrarCarpeta(string origen, string destino) {
        if (!ExisteDirectorio(destino)) {
            Directory.Move(origen, destino);
            return;
        }

        if (!string.Equals(origen, destino, StringComparison.OrdinalIgnoreCase)) {
            throw new IOException($"Ya existe una carpeta destino: {destino}");
        }

        string? rutaDirectorioPadre = Path.GetDirectoryName(origen);
        if (string.IsNullOrEmpty(rutaDirectorioPadre)) {
            throw new IOException($"No se pudo determinar el directorio base para renombrar: {origen}");
        }

        string rutaTemporal = Path.Combine(rutaDirectorioPadre, $".tmp-renombrar-{Guid.NewGuid():N}");
        Directory.Move(origen, rutaTemporal);
        Directory.Move(rutaTemporal, destino);
    }

    public static void CopiarCarpeta(string origen, string destino, bool forzar = false) {
        AsegurarDirectorio(destino);

        foreach (string archivoOrigen in ListarArchivos(origen)) {
            string nombreArchivo = Path.GetFileName(archivoOrigen);
            string archivoDestino = Path.Combine(destino, nombreArchivo);

            if (!forzar && ExisteArchivo(archivoDestino)) {
                continue;
            }

            File.Copy(archivoOrigen, archivoDestino, overwrite: forzar);
        }

        foreach (string subdirectorioOrigen in ListarDirectorios(origen)) {
            string nombreSubdirectorio = Path.GetFileName(subdirectorioOrigen);
            string subdirectorioDestino = Path.Combine(destino, nombreSubdirectorio);
            CopiarCarpeta(subdirectorioOrigen, subdirectorioDestino, forzar);
        }
    }

    static List<string> BuscarDirectoriosCompilacion(string rutaBase) {
        if (!ExisteDirectorio(rutaBase)) {
            return [];
        }

        return Directory.EnumerateDirectories(rutaBase, "*", SearchOption.AllDirectories)
            .Where(ruta => {
                string nombre = Path.GetFileName(ruta);
                return string.Equals(nombre, "bin", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(nombre, "obj", StringComparison.OrdinalIgnoreCase);
            })
            .OrderByDescending(ruta => ruta.Length)
            .ToList();
    }

    static string ResolverDirectorioDatos() {
        foreach (string candidato in ObtenerCandidatos()) {
            string? encontrado = BuscarDirectorioDatos(candidato);
            if (!string.IsNullOrWhiteSpace(encontrado)) {
                return encontrado;
            }
        }

        return Directory.GetCurrentDirectory();
    }

    static IEnumerable<string> ObtenerCandidatos() {
        string actual = Directory.GetCurrentDirectory();
        yield return actual;

        string subdirectorioAlumnos = Path.Combine(actual, "alumnos");
        if (ExisteDirectorio(subdirectorioAlumnos)) {
            yield return subdirectorioAlumnos;
        }

        yield return AppContext.BaseDirectory;
    }

    static string? BuscarDirectorioDatos(string rutaInicial) {
        DirectoryInfo? actual = new DirectoryInfo(rutaInicial);

        while (actual != null) {
            if (EsDirectorioDatos(actual.FullName)) {
                return actual.FullName;
            }

            actual = actual.Parent;
        }

        return null;
    }

    static bool EsDirectorioDatos(string ruta) {
        return ExisteArchivo(Path.Combine(ruta, "alumnos.md")) &&
               ExisteArchivo(Path.Combine(ruta, "Alumnos.csproj"));
    }

    static bool TieneMismoLegajo(string carpeta, int legajo) {
        return carpeta.StartsWith($"{legajo} ");
    }

    static string PerfilMarkdownPath(string carpetaPerfil) =>
        Path.Combine(carpetaPerfil, "perfil.md");
}
