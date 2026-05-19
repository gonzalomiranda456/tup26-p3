#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5001");

var app = builder.Build();

int contador = 0;

app.MapGet("/contador", () => {
    return RespuestaContador();
});

app.MapPut("/contador", () => {
    contador++;
    return RespuestaContador();
});

app.MapDelete("/contador", () => {
    contador = 0;
    return RespuestaContador();
});

app.Run();

string RespuestaContador() => $$"""{"contador": {{contador}} }""";
