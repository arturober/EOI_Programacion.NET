using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;

// CreateBuilder prepara la configuración, el registro de servicios y el servidor.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Razor Pages se encarga de localizar y ejecutar las páginas de la carpeta Pages.
builder.Services.AddRazorPages();

// Los controladores se utilizarán para devolver datos JSON desde las rutas /api.
builder.Services.AddControllers();

// Registramos el contexto para poder recibirlo mediante inyección de dependencias.
// La cadena llamada "Trivial" se lee desde appsettings.json.
builder.Services.AddDbContext<TrivialContext>(opciones =>
    opciones.UseSqlite(
        builder.Configuration.GetConnectionString("Trivial")));

// CORS permite que un cliente servido desde otro origen consulte la API.
// Para simplificar este proyecto educativo se aceptan todos los orígenes,
// cabeceras y métodos. En una aplicación real convendría limitar los orígenes.
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("PermitirTodos", politica =>
    {
        politica
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Build termina de configurar los servicios y crea la aplicación web.
WebApplication app = builder.Build();

// Los archivos situados en wwwroot podrán descargarse directamente desde el navegador.
// Aunque esta primera versión apenas los necesita, es una parte habitual de Razor Pages
// y permitirá añadir JavaScript y el cliente del juego sin cambiar esta configuración.
app.UseStaticFiles();

// Routing examina cada dirección solicitada y encuentra la Razor Page correspondiente.
app.UseRouting();

// Aplicamos la política después de Routing y antes de ejecutar los endpoints.
app.UseCors("PermitirTodos");

// MapControllers activa las rutas declaradas mediante atributos en Controllers.
app.MapControllers();

// MapRazorPages activa los endpoints definidos mediante la directiva @page.
app.MapRazorPages();

// Run inicia el servidor y mantiene la aplicación esperando peticiones HTTP.
app.Run();
