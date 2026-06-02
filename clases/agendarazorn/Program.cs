using AgendaRazorHtmx.Repositories;
using AgendaRazorHtmx.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
    options.Conventions.ConfigureFilter(
        new Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryTokenAttribute()));

builder.Services.AddSingleton<IContactoRepository, ContactoRepository>();
builder.Services.AddScoped<IContactoService, ContactoService>();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.Run();
