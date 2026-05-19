#:sdk Microsoft.NET.Sdk.Web
// La directiva de arriba indica que este archivo usa el SDK web
// de .NET para poder crear y ejecutar una aplicación ASP.NET Core mínima.

var builder = WebApplication.CreateBuilder(args); 
// Crea el objeto inicial de configuración de la 
// aplicación usando los argumentos recibidos al 
// iniciar el programa.

builder.WebHost.UseUrls("http://localhost:5000"); 
// Indica que el servidor web debe escuchar peticiones 
// HTTP en la dirección http://localhost:5000.

var app = builder.Build(); 
// Construye la aplicación final a partir de 
// toda la configuración acumulada en builder.

app.MapGet("/", () => $"Hola - Son las {DateTime.Now:HH:mm:ss}!"); 
app.MapGet("/saludar", () => $"Chau, nos vemos mañana"); 
// Registra una ruta GET en la raíz "/" que 
// devuelve el texto "Hola - Son las ...".

app.Run(); 
// Inicia la aplicación web y deja el proceso en 
// ejecución para seguir atendiendo solicitudes.
