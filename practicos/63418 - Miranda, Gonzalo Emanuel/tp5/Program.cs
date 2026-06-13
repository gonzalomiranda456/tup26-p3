using tp5.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
    builder.Services.AddDbContext<[TU_NOMBRE_DE_CONTEXTO]>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
    
var app = builder.Build();

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
