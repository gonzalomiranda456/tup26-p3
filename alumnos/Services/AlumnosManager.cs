namespace Tup26.AlumnosApp;

static class AlumnosManager {

    public static Alumnos Leer(string rutaArchivo) {
        Alumnos alumnos = new(Array.Empty<Alumno>());
        string comisionActual = string.Empty;

        try {
            string[] lineas = File.ReadAllLines(rutaArchivo, Encoding.UTF8);

            foreach (string linea in lineas) {
                if (string.IsNullOrWhiteSpace(linea)) {
                    continue;
                }

                if (linea.StartsWith("## ")) {
                    comisionActual = linea.Substring(3).Trim();
                    continue;
                }

                if (linea.StartsWith("#") || linea.StartsWith("```") || linea.StartsWith("Legajo") || linea.StartsWith("------")) {
                    continue;
                }

                Alumno? alumno = ExtraerAlumnoFormatoMarkdown(linea, comisionActual);

                if (alumno != null) {
                    alumnos.Agregar(alumno);
                }
            }
        }
        catch (Exception ex) {
            Log.Error($"Error al leer el archivo: {ex.Message}");
        }

        return alumnos;
    }

    public static void Escribir(Alumnos alumnos, string rutaArchivo) {
        try {
            List<Alumno> alumnosOrdenados = new(alumnos);
            alumnosOrdenados.Sort(Alumno.Comparar);

            StringBuilder sb = new();
            sb.AppendLine("# TUP 2026 - Programación III");
            sb.AppendLine();

            string? comisionActual = null;

            foreach (Alumno alumno in alumnosOrdenados) {
                string comisionAlumno = ObtenerComision(alumno);

                if (comisionActual != comisionAlumno) {
                    if (comisionActual != null) {
                        sb.AppendLine("```");
                        sb.AppendLine();
                    }

                    comisionActual = comisionAlumno;
                    sb.AppendLine($"## {comisionActual}");
                    sb.AppendLine("```text");
                    sb.AppendLine("Legajo  Nombre y Apellido                Teléfono         Foto  GitHub                   Prácticos          Exámenes        ");
                    sb.AppendLine("------  -------------------------------  ---------------  ----  -----------------------  -----------------  -----------------");
                }

                sb.AppendLine(FormatearFila(alumno));
            }

            if (comisionActual != null) {
                sb.AppendLine("```");
            }

            File.WriteAllText(rutaArchivo, sb.ToString().TrimEnd() + Environment.NewLine, Encoding.UTF8);
            Log.Info($"Alumnos guardados en: {rutaArchivo}");
        }
        catch (Exception ex) {
            Log.Error($"Error al guardar el archivo: {ex.Message}");
        }
    }

    public static void Listar(Alumnos alumnos, string titulo = "Listado de Alumnos") {
        if (alumnos == null || alumnos.Count == 0) {
            Log.Warning("No hay alumnos para mostrar.");
            return;
        }

        List<Alumno> alumnosOrdenados = new(alumnos);
        alumnosOrdenados.Sort(Alumno.Comparar);

        string encabezado = FormatearFilaTabla("Legajo", "Nombre y Apellido", "Telefono", "Foto", "GitHub", "Comision", "Practicos", "Examenes");
        string separador = new string('-', encabezado.Length);
        ConsoleColor colorAnterior = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(titulo.ToUpper());
        Console.ForegroundColor = colorAnterior;

        Console.WriteLine(separador);
        Console.WriteLine(encabezado);
        Console.WriteLine(separador);

        foreach (Alumno alumno in alumnosOrdenados) {
            Console.WriteLine(FormatearFilaTabla( alumno.Legajo.ToString(), alumno.NombreCompleto, alumno.Telefono, alumno.ConFoto ? "Si" : "No", alumno.GitHub, alumno.Comision, FormatearEstados(alumno.practicos), FormatearEstados(alumno.examenes)));
        }

        Console.WriteLine(separador);
        Console.WriteLine($"Total de alumnos: {alumnos.Count}");
        Console.WriteLine();
    }

    public static void CrearCarpetas(Alumnos alumnos) {
        string rutaBase = AppPaths.PracticosDirectory;
        Directory.CreateDirectory(rutaBase);

        foreach (Alumno alumno in alumnos) {
            string nombreCarpeta = alumno.CarpetaNombre;
            string rutaCarpeta   = Path.Combine(rutaBase, nombreCarpeta);
            try {
                List<string> carpetasConLegajo = BuscarCarpetasMismoLegajo(rutaBase, alumno.Legajo);

                if (carpetasConLegajo.Count == 0) {
                    Directory.CreateDirectory(rutaCarpeta);
                    Log.Debug($" ➕ {rutaCarpeta, -40}");
                } else if (carpetasConLegajo.Count == 1){
                    string rutaCarpetaExistente = carpetasConLegajo[0];
                    string rutaRelativa = Path.GetRelativePath(rutaBase, rutaCarpetaExistente); 
                    if (string.Equals(rutaCarpetaExistente, rutaCarpeta, StringComparison.OrdinalIgnoreCase)) {
                        Log.Info($" ✅ {rutaRelativa, -40}");
                    } else {
                        RenombrarCarpeta(rutaCarpetaExistente, rutaCarpeta);
                        Log.Warning($" 🔄 {rutaRelativa, -40} → {nombreCarpeta}");
                    }
                } else {
                    Log.Warning($" ⚠️  {alumno.Legajo}. Revisar manualmente las duplicadas.");
                }
            }
            catch (Exception ex) {
                Log.Error($"Error al crear la carpeta para {nombreCarpeta}: {ex.Message}");
            }
        }
    }

    public static void CopiarFotoPerfil(Alumnos alumnos, string rutaFotos) {
        string rutaBase = AppPaths.PracticosDirectory;

        if (!Directory.Exists(rutaBase)) {
            Log.Error($"No existe la carpeta base de prácticos: {rutaBase}");
            return;
        }

        foreach (Alumno alumno in alumnos) {
            if (string.IsNullOrWhiteSpace(alumno.TelefonoId)) {
                continue;
            }

            string rutaFotoOrigen = Path.Combine(rutaFotos, alumno.TelefonoId, "foto.png");
            if (!File.Exists(rutaFotoOrigen)) {
                continue;
            }

            string? rutaCarpetaAlumno = ObtenerCarpetaUnicaMismoLegajo(rutaBase, alumno.Legajo);
            if (rutaCarpetaAlumno == null || !Directory.Exists(rutaCarpetaAlumno)) {
                continue;
            }

            string rutaFotoDestino = Path.Combine(rutaCarpetaAlumno, "foto.png");
            if (File.Exists(rutaFotoDestino)) {
                continue;
            }

            try {
                File.Copy(rutaFotoOrigen, rutaFotoDestino);
                Log.Info($"Foto copiada: {rutaFotoOrigen} -> {rutaFotoDestino}");
            }
            catch (Exception ex) {
                Log.Error($"Error al copiar la foto para {alumno.CarpetaNombre}: {ex.Message}");
            }
        }
    }

    public static void CopiarEnunciadoPracticos(Alumnos alumnos, string practico, bool forzar = false) {
        string nombrePractico    = practico.Trim();
        string origen            = Path.Combine(AppPaths.EnunciadosDirectory, nombrePractico);
        string rutaBasePracticos = AppPaths.PracticosDirectory;
        string carpetaPractico   = nombrePractico.ToLower();

        if (string.IsNullOrWhiteSpace(nombrePractico)) {
            Log.Error("Debe indicar el nombre del práctico a copiar.");
            return;
        }

        if (!Directory.Exists(origen)) {
            Log.Error($"No existe la carpeta del enunciado: {origen}");
            return;
        }

        if (!Directory.Exists(rutaBasePracticos)) {
            Log.Error($"No existe la carpeta base de prácticos: {rutaBasePracticos}");
            return;
        }

        foreach (Alumno alumno in alumnos) {
            string rutaAlumno = Path.Combine(rutaBasePracticos, alumno.CarpetaNombre);

            if (!Directory.Exists(rutaAlumno)) {
                try {
                    Directory.CreateDirectory(rutaAlumno);
                    Log.Debug($" ➕ {rutaAlumno, -40}");
                }
                catch (Exception ex) {
                    Log.Error($"No se pudo crear la carpeta del alumno {rutaAlumno}: {ex.Message}");
                    continue;
                }
            }

            string destino = Path.Combine(rutaAlumno, carpetaPractico);

            try {
                CopiarCarpeta(origen, destino, forzar);
                Log.Info($"Enunciado copiado: {origen} -> {destino}");
            }
            catch (Exception ex) {
                Log.Error($"Error al copiar el enunciado para {alumno.CarpetaNombre}: {ex.Message}");
            }
        }
    }

    public static void ActualizarDesdePerfiles(Alumnos alumnos, string rutaPerfiles) {
        Dictionary<int, Alumno> porLegajo = new();

        foreach (Alumno alumno in alumnos) {
            porLegajo[alumno.Legajo] = alumno;
        }

        foreach (string carpetaPerfil in Directory.GetDirectories(rutaPerfiles)) {
            string rutaPerfil = Path.Combine(carpetaPerfil, "perfil.md");
            if (!File.Exists(rutaPerfil)) {
                continue;
            }

            try {
                int legajo = 0;
                string gitHub = string.Empty;

                foreach (string linea in File.ReadAllLines(rutaPerfil, Encoding.UTF8)) {
                    string l = linea.Trim();

                    if (l.StartsWith("- Legajo:")) {
                        string valor = l.Substring("- Legajo:".Length).Trim();
                        int.TryParse(valor, out legajo);
                    }
                    else if (l.StartsWith("- Github:")) {
                        string valor = l.Substring("- Github:".Length).Trim();

                        if (!string.IsNullOrWhiteSpace(valor) &&
                            !valor.StartsWith("No", StringComparison.OrdinalIgnoreCase)) {
                            gitHub = valor;
                        }
                    }
                }

                if (legajo == 0 || !porLegajo.ContainsKey(legajo)) {
                    continue;
                }

                Alumno alumno = porLegajo[legajo];
                bool actualizado = false;

                if (!string.IsNullOrWhiteSpace(gitHub) && alumno.GitHub != gitHub) {
                    Log.Info($"  GitHub actualizado {alumno.CarpetaNombre}: '{alumno.GitHub}' -> '{gitHub}'");
                    alumno.GitHub = gitHub;
                    actualizado = true;
                }

                if (!actualizado) {
                    Log.Warning($"  Sin cambios: {alumno.CarpetaNombre}");
                }
            }
            catch (Exception ex) {
                Log.Error($"Error al leer perfil {rutaPerfil}: {ex.Message}");
            }
        }
    }

    public static void EscribirJSON(Alumnos alumnos, string rutaArchivo) {
        try {
            var datos = alumnos.Select(alumno => new {
                alumno.Legajo,
                alumno.Comision,
                alumno.Nombre,
                alumno.Apellido,
                alumno.Telefono,
                TieneFoto = alumno.ConFoto,
                GitHub = alumno.GitHub,
                Practicos = alumno.practicos.Select(e => e.ToEmoji()).ToList(),
                Examenes = alumno.examenes.Select(e => e.ToEmoji()).ToList()
            });

            JsonSerializerOptions opciones = new() {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            File.WriteAllText(rutaArchivo, JsonSerializer.Serialize(datos, opciones) + Environment.NewLine, new UTF8Encoding(false));
            
            Log.Info($"Alumnos guardados en JSON: {rutaArchivo}");
        }
        catch (Exception ex) {
            Log.Error($"Error al guardar el archivo JSON: {ex.Message}");
        }
    }

    public static void EscribirVCard(Alumnos alumnos, string rutaArchivo) {
        try {
            var alumnosConTelefono = alumnos.Where(a => a.ConTelefono)
                .OrderBy(a => a.Comision).ThenBy(a => a.NombreCompleto).ThenBy(a => a.Legajo);

            StringBuilder sb = new();
            foreach (Alumno alumno in alumnosConTelefono) {
                AppendVCardContacto(sb, alumno);
            }
            File.WriteAllText(rutaArchivo, sb.ToString(), new UTF8Encoding(false));

            Log.Info($"Contactos vCard guardados en: {rutaArchivo}");
        } catch (Exception ex) {
            Log.Error($"Error al guardar el archivo vCard: {ex.Message}");
        }
    }

    static Alumno? ExtraerAlumnoFormatoMarkdown(string linea, string comisionActual) {
        List<string> columnas = Regex.Split(linea.TrimEnd(), @"\s{2,}").ToList();

        while (columnas.Count < 7) {
            columnas.Add(string.Empty);
        }

        if (!int.TryParse(columnas[0].Trim(), out int legajo)) {
            return null;
        }

        (string apellido, string nombre) = ExtraerApellidoNombre(columnas[1]);

        Alumno alumno = new(legajo, comisionActual, nombre, apellido, ExtraerTelefono(columnas[2]), ExtraerGitHub(columnas[4]), ExtraerFoto(columnas[3]));
        CargarEstados(alumno.practicos, columnas[5]);
        CargarEstados(alumno.examenes, columnas[6]);

        return alumno;
    }


    static string FormatearFilaTabla(string legajo, string nombreApellido, string telefono, string foto, string gitHub, string comision, string pruebas, string examenes) {
        string colLegajo         = AjustarColumna(legajo, 6);
        string colNombreApellido = AjustarColumna(nombreApellido, 26);
        string colTelefono       = AjustarColumna(telefono, 15);
        string colFoto           = AjustarColumna(foto, 4);
        string colGitHub         = AjustarColumna(gitHub, 25);
        string colComision       = AjustarColumna(comision, 10);
        string colPruebas        = AjustarColumna(pruebas, 20);
        string colExamenes       = AjustarColumna(examenes, 20);

        return $"{colLegajo}  {colNombreApellido}  {colTelefono}  {colFoto}  {colGitHub}  {colComision}  {colPruebas}  {colExamenes}";
    }

    static string ObtenerComision(Alumno alumno) {
        return FormatearTexto(alumno.Comision);
    }

    static string FormatearFila(Alumno alumno) {
        string legajo         = AjustarColumna(alumno.Legajo.ToString(), 6);
        string nombreApellido = AjustarColumna(alumno.NombreCompleto, 31);
        string telefono       = AjustarColumna(alumno.Telefono, 15);
        string foto           = AjustarColumna(alumno.TieneFoto ? "Si" : "No", 4);
        string gitHub         = AjustarColumna(alumno.GitHub, 23);
        string pruebas        = AjustarColumna(FormatearEstados(alumno.practicos, 10));
        string examenes       = AjustarColumna(FormatearEstados(alumno.examenes, 10));

        return $"{legajo}  {nombreApellido}  {telefono}  {foto}  {gitHub}  {pruebas}  {examenes}";
    }


    static string AjustarColumna(string texto, int ancho = 20) {
        string valor = FormatearTexto(texto);

        if (valor.Length > ancho) { return valor.Substring(0, ancho); }

        return valor.PadRight(ancho);
    }

    static string FormatearTexto(string texto) {
        if (string.IsNullOrWhiteSpace(texto)) { return "—"; }

        return texto.Trim();
    }

    static string LimpiarCampo(string texto) {
        string valor = texto.Trim();
        if (valor is "—" or "-" or "(-)" or "(—)") { return string.Empty; }

        return valor;
    }

    static string ExtraerTelefono(string texto) {
        return LimpiarCampo(texto);
    }

    static (string, string) ExtraerApellidoNombre(string nombreCompleto) {
        string apellido;
        string nombre;
        string[] partes = nombreCompleto.Split(',', 2);

        if (partes.Length == 2) {
            apellido = LimpiarCampo(partes[0]);
            nombre   = LimpiarCampo(partes[1]);
        } else {
            apellido = LimpiarCampo(nombreCompleto);
            nombre   = string.Empty;
        }

        return (apellido, nombre);
    }

    static string ExtraerGitHub(string texto) {
        string valor = LimpiarCampo(texto);
        return string.Equals(valor, "no", StringComparison.OrdinalIgnoreCase) ? string.Empty : valor;
    }

    static string FormatearGitHub(string gitHub) {
        gitHub = gitHub.Trim();
        return string.IsNullOrWhiteSpace(gitHub) ? "-" : gitHub;
    }

    static string FormatearEstados(List<Estado> estados, int maxEstados = 20) {
        string valor = string.Empty;
        if (estados?.Count > 0) {
            valor = string.Join(string.Empty, estados.Select(e => e.ToEmoji()));
        }
        valor = valor.Replace(" ", "⚪️");
        while (StringInfo.ParseCombiningCharacters(valor).Length < maxEstados) {
            valor += "⚪️";
        }
        return valor;
    }

    static bool ExtraerFoto(string texto) {
        texto = texto.Trim().ToLower();
        return texto== "si" || texto == "true" || texto == "yes";
    }

    static void CargarEstados(List<Estado> destino, string texto) {
        destino.Clear();

        TextElementEnumerator enumerador = StringInfo.GetTextElementEnumerator(texto);
        while (enumerador.MoveNext()) {
            string elemento = enumerador.GetTextElement().Trim();
            if (string.IsNullOrWhiteSpace(elemento)) {
                continue;
            }

            Estado estado = EstadoExtensions.Parse(elemento);
            if (estado != Estado.Vacio || EsEstadoVacio(elemento)) {
                destino.Add(estado);
            }
        }

        while (destino.Count > 0 && destino[^1] == Estado.Vacio) {
            destino.RemoveAt(destino.Count - 1);
        }
    }

    static bool EsEstadoVacio(string texto) =>
        texto is "⚪" or "⚪️";

    static bool TieneMismoLegajo(string carpeta, int legajo) {
        return carpeta.StartsWith($"{legajo} ");
    }

    static List<string> BuscarCarpetasMismoLegajo(string rutaBase, int legajo) {
        List<string> carpetasMismoLegajo = new();

        if (!Directory.Exists(rutaBase)) {
            return carpetasMismoLegajo;
        }

        foreach (string carpetaExistente in Directory.GetDirectories(rutaBase)) {
            string nombreCarpetaExistente = Path.GetFileName(carpetaExistente);

            if (TieneMismoLegajo(nombreCarpetaExistente, legajo)) {
                carpetasMismoLegajo.Add(carpetaExistente);
            }
        }

        return carpetasMismoLegajo;
    }

    static string? ObtenerCarpetaUnicaMismoLegajo(string carpeta, int legajo) {
        List<string> carpetasMismoLegajo = BuscarCarpetasMismoLegajo(carpeta, legajo);

        if (carpetasMismoLegajo.Count != 1) {
            return null;
        }

        return carpetasMismoLegajo[0];
    }

    static void RenombrarCarpeta(string origen, string destino) {
        if (!Directory.Exists(destino)) {
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

    static void CopiarCarpeta(string origen, string destino, bool forzar = false) {
        Directory.CreateDirectory(destino);

        foreach (string archivoOrigen in Directory.GetFiles(origen)) {
            string nombreArchivo = Path.GetFileName(archivoOrigen);
            string archivoDestino = Path.Combine(destino, nombreArchivo);

            if (!forzar && File.Exists(archivoDestino)) {
                continue;
            }

            File.Copy(archivoOrigen, archivoDestino, overwrite: forzar);
        }

        foreach (string subdirectorioOrigen in Directory.GetDirectories(origen)) {
            string nombreSubdirectorio  = Path.GetFileName(subdirectorioOrigen);
            string subdirectorioDestino = Path.Combine(destino, nombreSubdirectorio);
            CopiarCarpeta(subdirectorioOrigen, subdirectorioDestino, forzar);
        }
    }

    static void AppendVCardContacto(StringBuilder sb, Alumno alumno) {
        string apellido         = FormatearTextoVcard(alumno.Apellido);
        string nombre           = FormatearTextoVcard(alumno.Nombre);
        string nombreCompleto   = FormatearTextoVcard($"{nombre} {apellido}");
        string comision         = FormatearTextoVcard(alumno.Comision);
        string etiquetaBusqueda = FormatearTextoVcard(ObtenerEtiquetaBusqueda(alumno));
        string etiquetaVisible  = FormatearTextoVcard($"{etiquetaBusqueda}-{alumno.Legajo}");
        string telefonoE164     = $"+{alumno.TelefonoId}";

        sb.AppendLine("BEGIN:VCARD");
        sb.AppendLine("VERSION:3.0");
        sb.AppendLine($"N:{apellido};{nombre};;;");
        sb.AppendLine($"FN:{nombreCompleto} | {etiquetaVisible}");
        sb.AppendLine($"NICKNAME:{etiquetaVisible}");
        sb.AppendLine("ORG:TUP 2026 - Programacion III");
        sb.AppendLine($"CATEGORIES:{etiquetaBusqueda}");
        sb.AppendLine($"NOTE:Legajo {alumno.Legajo} | Comision {comision} | Etiqueta {etiquetaBusqueda} | GitHub {ObtenerGitHubVisible(alumno)}");
        sb.AppendLine($"X-TUP-LEGAJO:{alumno.Legajo}");
        sb.AppendLine($"X-TUP-COMISION:{comision}");
        sb.AppendLine($"TEL;TYPE=CELL;TYPE=VOICE:{telefonoE164}");
        sb.AppendLine("END:VCARD");
    }

    static string FormatearTextoVcard(string texto) {
        string valor = FormatearTexto(texto);
        return valor
            .Replace("\\", "\\\\")
            .Replace(";", "\\;")
            .Replace(",", "\\,")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n");
    }

    static string ObtenerGitHubVisible(Alumno alumno) {
        string gitHub = FormatearGitHub(alumno.GitHub);
        return gitHub == "-" ? "sin GitHub" : gitHub;
    }

    static string ObtenerEtiquetaBusqueda(Alumno alumno) {
        return $"TUP26-P3-{ObtenerComision(alumno)}";
    }

}
