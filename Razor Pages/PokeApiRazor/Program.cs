using System.Net.Http.Headers;
using PokeApiRazor.Services;

// WebApplication.CreateBuilder prepara los servicios de la aplicación.
var builder = WebApplication.CreateBuilder(args);

// Añadimos Razor Pages para poder crear páginas con .cshtml y PageModel.
builder.Services.AddRazorPages();

// La caché evita descargar la lista completa de Pokémon en cada pulsación.
builder.Services.AddMemoryCache();

// Registramos el servicio que se encargará de comunicarse con PokeAPI.
builder.Services.AddHttpClient<PokeApiService>(cliente =>
{
    // Todas las rutas del servicio partirán de esta dirección.
    cliente.BaseAddress = new Uri("https://pokeapi.co/api/v2/");

    // Indicamos que esperamos recibir las respuestas en formato JSON.
    cliente.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));

    // Un User-Agent permite identificar educadamente nuestra aplicación.
    cliente.DefaultRequestHeaders.UserAgent.ParseAdd(
        "PokeApiRazor-Educativo/1.0");

    // Evitamos que una API que no responde deje esperando la web indefinidamente.
    cliente.Timeout = TimeSpan.FromSeconds(20);
});

// Construimos la aplicación después de registrar todos sus servicios.
var app = builder.Build();

// En producción mostramos una página de error sencilla y segura.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

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
