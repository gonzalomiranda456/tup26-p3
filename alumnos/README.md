# Alumnos · TUP 2026 · Programación III

Herramienta de consola para administrar la lista de alumnos, exportar la información y revisar recursos vinculados a cada legajo.

## Requisitos

- .NET 10
- Ejecutar los comandos desde la carpeta del proyecto

## Uso

```bash
dotnet run -- ayuda
```

## Comandos

- `listar`: muestra todos los alumnos
- `sin-github`: filtra alumnos sin cuenta de GitHub
- `sin-telefono`: filtra alumnos sin teléfono
- `sin-foto`: filtra alumnos sin foto
- `guardar [archivo.md]`: exporta la lista en Markdown
- `json [archivo.json]`: exporta la lista en JSON
- `vcf [archivo.vcf]`: exporta la lista en formato vCard
- `crear-carpetas`: crea la estructura de carpetas del curso
- `prs`: revisa pull requests de alumnos
- `normalizar-prs [--simular]`: ajusta títulos de pull requests
- `bajar-prs TP1 [--forzar]`: descarga archivos del práctico desde los PRs
- `cerrar-prs TP1`: cierra todos los PRs abiertos del práctico indicado
- `revisar-presentados TP1`: marca como presentado un TP si supera 200 líneas en los archivos locales de la carpeta del trabajo
- `wapp`: muestra mensajes del grupo de WhatsApp

## Archivos de referencia

- `alumnos.md`: listado principal de alumnos
- `agenda.html`: vista HTML de la agenda/listado