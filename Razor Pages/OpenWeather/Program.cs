using OpenWeather.Configuracion;
using OpenWeather.Servicios;

// CreateBuilder prepara la configuración, los servicios y el servidor web.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Este archivo opcional permite usar una clave local sin subirla a GitHub.
// Las variables de entorno se añaden después para que tengan la máxima prioridad.
builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true)
    .AddEnvironmentVariables();

// Razor Pages se ocupa de localizar y ejecutar las páginas de la carpeta Pages.
builder.Services.AddRazorPages();

// Los controladores exponen una pequeña API JSON propia bajo la ruta /api.
builder.Services.AddControllers();

// MemoryCache evita repetir inmediatamente las mismas peticiones a OpenWeather.
builder.Services.AddMemoryCache();

// Esta sección se lee desde appsettings, secretos de usuario o variables de entorno.
builder.Services.Configure<OpenWeatherOpciones>(
    builder.Configuration.GetSection(OpenWeatherOpciones.Seccion));

// AddHttpClient crea y reutiliza correctamente las conexiones HTTP.
// El servicio queda disponible mediante inyección de dependencias.
builder.Services.AddHttpClient<IOpenWeatherServicio, OpenWeatherServicio>(cliente =>
{
    cliente.BaseAddress = new Uri("https://api.openweathermap.org/");
    cliente.Timeout = TimeSpan.FromSeconds(15);
});

// Build termina de registrar los servicios y crea la aplicación.
WebApplication app = builder.Build();

// En producción se ocultan los detalles internos de las excepciones.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Redirige las peticiones HTTP a HTTPS cuando el servidor lo permite.
app.UseHttpsRedirection();

// Permite servir JavaScript, iconos y otros archivos situados en wwwroot.
app.UseStaticFiles();

// Routing relaciona cada dirección con su Razor Page o controlador.
app.UseRouting();

// Activa los endpoints JSON definidos mediante atributos en Controllers.
app.MapControllers();

// Activa las páginas que contienen la directiva @page.
app.MapRazorPages();

// Run inicia el servidor y lo deja esperando peticiones.
app.Run();
