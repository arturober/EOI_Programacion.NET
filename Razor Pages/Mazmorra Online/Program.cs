using System.Text.Json.Serialization;
using MazmorraOnline.Hubs;
using MazmorraOnline.Services;

// WebApplicationBuilder prepara los servicios de ASP.NET Core.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Razor Pages genera las páginas del frontend.
builder.Services.AddRazorPages();

// Los controladores proporcionan los servicios web REST.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

// SignalR comunica el servidor y los jugadores en tiempo real.
builder.Services
    .AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

// Solo existe un gestor del juego para toda la aplicación.
builder.Services.AddSingleton<GestorJuego>();

// El motor calcula la física y envía el estado a 10 Hz.
builder.Services.AddHostedService<MotorJuego>();

// Build crea la aplicación con los servicios configurados anteriormente.
WebApplication app = builder.Build();

// Los archivos de wwwroot se pueden descargar desde el navegador.
app.UseStaticFiles();

// El enrutamiento busca la Razor Page, el controlador o el hub solicitado.
app.UseRouting();

// Se publican los tres tipos de rutas utilizados por la aplicación.
app.MapRazorPages();
app.MapControllers();
app.MapHub<JuegoHub>("/hubs/juego");

// Run inicia el servidor web.
app.Run();
