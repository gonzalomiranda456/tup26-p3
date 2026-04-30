using Spectre.Console;
using Spectre.Console.Cli;

namespace Tup26.AlumnosApp;

static class AlumnosCliApp {
    public static CommandApp Crear() {
        CommandApp app = new();
        app.Configure(config => {
            config.SetApplicationName("dotnet run --");

            config.AddCommand<ListarCommand>("listar")
                .WithDescription("Muestra todos los alumnos.");
            config.AddCommand<SinGithubCommand>("sin-github")
                .WithDescription("Lista alumnos sin cuenta de GitHub.");
            config.AddCommand<SinTelefonoCommand>("sin-telefono")
                .WithDescription("Lista alumnos sin teléfono.");
            config.AddCommand<SinFotoCommand>("sin-foto")
                .WithDescription("Lista alumnos sin foto.");
            config.AddCommand<GuardarCommand>("guardar")
                .WithDescription("Exporta la lista a Markdown.");
            config.AddCommand<JsonCommand>("json")
                .WithDescription("Exporta la lista a JSON.");
            config.AddCommand<VcfCommand>("vcf")
                .WithDescription("Exporta la lista a vCard.");
            config.AddCommand<CrearCarpetasCommand>("crear-carpetas")
                .WithDescription("Crea o normaliza las carpetas de prácticos.");
            config.AddCommand<PrsCommand>("prs")
                .WithDescription("Revisa pull requests de los alumnos.");
            config.AddCommand<NormalizarPrsCommand>("normalizar-prs")
                .WithDescription("Normaliza títulos de pull requests.");
            config.AddCommand<BajarPrsCommand>("bajar-prs")
                .WithDescription("Descarga archivos del práctico desde PRs.");
            config.AddCommand<CerrarPrsCommand>("cerrar-prs")
                .WithDescription("Cierra PRs abiertos; opcionalmente filtra por TP.");
            config.AddCommand<RevisarPresentadosCommand>("revisar-presentados")
                .WithDescription("Marca TPs presentados a partir del código local.");
            config.AddCommand<RegistrarAsistenciasCommand>("registrar-asistencias")
                .WithDescription("Convierte presentes del día en asistencias acumuladas.");
            config.AddCommand<RelevarAsistenciasCommand>("relevar-asistencias")
                .WithDescription("Busca asistencias de hoy a partir de WhatsApp.");
            config.AddCommand<WappGruposCommand>("wapp-grupos")
                .WithDescription("Lista grupos y participantes de WhatsApp.");
        });

        return app;
    }

    public static string[] NormalizarArgumentosAyuda(string[] args) {
        if (args.Length == 0 || !EsAliasAyuda(args[0])) {
            return args;
        }

        if (args.Length == 1) {
            return ["--help"];
        }

        return [.. args[1..], "--help"];
    }

    public static int EjecutarModoInteractivo(CommandApp app) {
        while (true) {
            string[]? argumentosInteractivos = SolicitarComandoInteractivo();
            if (argumentosInteractivos is null) {
                AnsiConsole.MarkupLine("[grey]Saliendo del modo interactivo...[/]");
                return 0;
            }

            if (argumentosInteractivos.Length == 0) {
                AnsiConsole.Clear();
                continue;
            }

            string[] args = NormalizarArgumentosAyuda(argumentosInteractivos);
            if (args.Length == 0) {
                continue;
            }

            AnsiConsole.WriteLine();
            int codigo = app.Run(args);
            AnsiConsole.WriteLine();

            if (codigo != 0) {
                AnsiConsole.MarkupLine($"[red]El comando terminó con código {codigo}.[/]");
            }

            AnsiConsole.MarkupLine("[grey]Presioná una tecla para volver al menú...[/]");
            Console.ReadKey(intercept: true);
            AnsiConsole.Clear();
        }
    }

    public static string[]? SolicitarComandoInteractivo() {
        Alumnos alumnos = AlumnosManager.Leer();

        AnsiConsole.Write(
            new Panel(new Markup($"[bold]Modo interactivo[/]\n[grey]Elegí una acción y completá sus parámetros.[/]\n[green]Alumnos detectados:[/] {alumnos.Count()}")) {
                Header = new PanelHeader("[yellow]TUP 2026 · Alumnos[/]"),
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
            });

        InteractiveChoice opcion = PedirOpcion(
            "[bold cyan]Menú principal[/] · ¿Qué querés hacer?",
            ObtenerOpcionesPrincipales());

        return opcion.Command switch {
            "listar" => ["listar"],
            "auditoria" => SolicitarMenuAuditoria(),
            "exportar" => SolicitarMenuExportar(),
            "crear-carpetas" => ["crear-carpetas"],
            "prs" => SolicitarMenuPrs(),
            "asistencias" => SolicitarMenuAsistencias(),
            "salir" => null,
            _ => Array.Empty<string>()
        };
    }

    static string[] SolicitarMenuAuditoria() {
        InteractiveChoice opcion = PedirOpcion(
            "[bold cyan]Principal / Auditoría[/] · Elegí una auditoría",
            [
                new("sin-github", "Sin GitHub", "Filtrar alumnos sin usuario GitHub"),
                new("sin-telefono", "Sin teléfono", "Filtrar alumnos sin teléfono"),
                new("sin-foto", "Sin foto", "Filtrar alumnos sin foto"),
                new("volver", "Volver", "Regresar al menú principal")
            ]);

        return opcion.Command switch {
            "sin-github" => ["sin-github"],
            "sin-telefono" => ["sin-telefono"],
            "sin-foto" => ["sin-foto"],
            _ => Array.Empty<string>()
        };
    }

    static string[] SolicitarMenuExportar() {
        InteractiveChoice opcion = PedirOpcion(
            "[bold cyan]Principal / Exportar[/] · Elegí un formato",
            [
                new("guardar", "Markdown", "Guardar el listado en formato Markdown"),
                new("json", "JSON", "Exportar el listado en JSON"),
                new("vcf", "vCard", "Exportar contactos en formato vCard"),
                new("volver", "Volver", "Regresar al menú principal")
            ]);

        return opcion.Command switch {
            "guardar" => ConstruirArgumentosExportacion("guardar"),
            "json" => ConstruirArgumentosExportacion("json"),
            "vcf" => ConstruirArgumentosExportacion("vcf"),
            _ => Array.Empty<string>()
        };
    }

    static string[] SolicitarMenuPrs() {
        InteractiveChoice opcion = PedirOpcion(
            "[bold cyan]Principal / PRs[/] · Elegí una acción",
            [
                new("prs", "Revisar PRs", "Mostrar estado de pull requests"),
                new("normalizar-prs", "Normalizar PRs", "Ajustar títulos de pull requests"),
                new("bajar-prs", "Bajar PRs", "Descargar archivos de un trabajo práctico"),
                new("cerrar-prs", "Cerrar PRs", "Cerrar pull requests abiertos"),
                new("revisar-presentados", "Revisar presentados", "Marcar TPs presentados según líneas locales"),
                new("volver", "Volver", "Regresar al menú principal")
            ]);

        return opcion.Command switch {
            "prs" => ["prs"],
            "normalizar-prs" => ConstruirArgumentosNormalizarPrs(),
            "bajar-prs" => ConstruirArgumentosBajarPrs(),
            "cerrar-prs" => ConstruirArgumentosCerrarPrs(),
            "revisar-presentados" => ["revisar-presentados", PedirTrabajoPractico()],
            _ => Array.Empty<string>()
        };
    }

    static string[] SolicitarMenuAsistencias() {
        InteractiveChoice opcion = PedirOpcion(
            "[bold cyan]Principal / Asistencias y WhatsApp[/] · Elegí una acción",
            [
                new("registrar-asistencias", "Registrar asistencias", "Consolidar presentes del día"),
                new("relevar-asistencias", "Relevar asistencias", "Detectar presentes desde WhatsApp"),
                new("wapp-grupos", "WhatsApp grupos", "Listar grupos y participantes"),
                new("volver", "Volver", "Regresar al menú principal")
            ]);

        return opcion.Command switch {
            "registrar-asistencias" => ["registrar-asistencias"],
            "relevar-asistencias" => ["relevar-asistencias"],
            "wapp-grupos" => ["wapp-grupos"],
            _ => Array.Empty<string>()
        };
    }

    static InteractiveChoice PedirOpcion(string titulo, IReadOnlyList<InteractiveChoice> opciones) =>
        AnsiConsole.Prompt(
            new SelectionPrompt<InteractiveChoice>()
                .Title(titulo)
                .PageSize(12)
                .UseConverter(choice => $"[green]{choice.Label}[/] [grey]- {choice.Description}[/]")
                .AddChoices(opciones));

    static IReadOnlyList<InteractiveChoice> ObtenerOpcionesPrincipales() =>
        [
            new("listar", "Listar", "Mostrar todos los alumnos"),
            new("auditoria", "Auditoría", "Revisar datos faltantes o incompletos"),
            new("exportar", "Exportar", "Guardar o exportar en distintos formatos"),
            new("crear-carpetas", "Crear carpetas", "Crear o normalizar carpetas de alumnos"),
            new("prs", "PRs", "Operaciones sobre pull requests y prácticos"),
            new("asistencias", "Asistencias y WhatsApp", "Acciones vinculadas a presentes y grupos"),
            new("salir", "Salir", "Cerrar la aplicación")
        ];

    static string[] ConstruirArgumentosExportacion(string comando) {
        string ruta = AnsiConsole.Prompt(
            new TextPrompt<string>($"Ruta de salida para [green]{comando}[/] ([grey]vacío = predeterminada[/]):")
                .AllowEmpty());

        return string.IsNullOrWhiteSpace(ruta) ? [comando] : [comando, ruta.Trim()];
    }

    static string[] ConstruirArgumentosNormalizarPrs() {
        bool simular = AnsiConsole.Confirm("¿Ejecutar en modo simulación?", true);

        return simular ? ["normalizar-prs", "--simular"] : ["normalizar-prs"];
    }

    static string[] ConstruirArgumentosBajarPrs() {
        string trabajoPractico = PedirTrabajoPractico();
        bool forzar = AnsiConsole.Confirm("¿Sobrescribir archivos ya existentes?", false);

        return forzar
            ? ["bajar-prs", trabajoPractico, "--forzar"]
            : ["bajar-prs", trabajoPractico];
    }

    static string[] ConstruirArgumentosCerrarPrs() {
        bool filtrarPorTp = AnsiConsole.Confirm("¿Querés cerrar sólo los PRs de un TP específico?", false);

        return filtrarPorTp
            ? ["cerrar-prs", PedirTrabajoPractico()]
            : ["cerrar-prs"];
    }

    static string PedirTrabajoPractico() =>
        AnsiConsole.Prompt(
            new TextPrompt<string>("Trabajo práctico ([green]TP1[/] o [green]1[/]):")
                .PromptStyle("cyan")
                .Validate(valor =>
                    AlumnosCliActions.EsTrabajoPracticoValido(valor)
                        ? ValidationResult.Success()
                        : ValidationResult.Error(AlumnosCliActions.MensajeTrabajoPracticoInvalido(valor))));

    static bool EsAliasAyuda(string valor) =>
        string.Equals(valor, "ayuda", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(valor, "help", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(valor, "-h", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(valor, "--help", StringComparison.OrdinalIgnoreCase);

    sealed record InteractiveChoice(string Command, string Label, string Description);
}