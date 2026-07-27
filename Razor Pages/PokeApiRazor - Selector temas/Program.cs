using System.Net.Http.Headers;

// WebApplication.CreateBuilder prepara los servicios de la aplicación.
var builder = WebApplication.CreateBuilder(args);

// Añadimos Razor Pages para poder crear páginas con .cshtml y PageModel.
builder.Services.AddRazorPages();

// Construimos la aplicación después de registrar todos sus servicios.
var app = builder.Build();

// Redirigimos automáticamente de HTTP a HTTPS.
app.UseHttpsRedirection();

// Permitimos acceder a los archivos de wwwroot.
app.UseStaticFiles();

// Activamos el sistema de rutas.
app.UseRouting();

// Asociamos las URL con las páginas Razor correspondientes.
app.MapRazorPages();

// Iniciamos la aplicación.
app.Run();
