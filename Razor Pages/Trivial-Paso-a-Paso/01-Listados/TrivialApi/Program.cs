using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;

// CreateBuilder prepara la configuración, el registro de servicios y el servidor.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Razor Pages se encarga de localizar y ejecutar las páginas de la carpeta Pages.
builder.Services.AddRazorPages();

// Registramos el contexto para poder recibirlo mediante inyección de dependencias.
// La cadena llamada "Trivial" se lee desde appsettings.json.
builder.Services.AddDbContext<TrivialContext>(opciones =>
    opciones.UseSqlite(
        builder.Configuration.GetConnectionString("Trivial")));

// Build termina de configurar los servicios y crea la aplicación web.
WebApplication app = builder.Build();

// Los archivos situados en wwwroot podrán descargarse directamente desde el navegador.
// Aunque esta primera versión apenas los necesita, es una parte habitual de Razor Pages
// y permitirá añadir JavaScript y el cliente del juego sin cambiar esta configuración.
app.UseStaticFiles();

// Routing examina cada dirección solicitada y encuentra la Razor Page correspondiente.
app.UseRouting();

// MapRazorPages activa los endpoints definidos mediante la directiva @page.
app.MapRazorPages();

// Run inicia el servidor y mantiene la aplicación esperando peticiones HTTP.
app.Run();

