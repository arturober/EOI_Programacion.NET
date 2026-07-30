using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NasaExplorer.Configuracion;
using NasaExplorer.Data;
using NasaExplorer.Modelos;
using NasaExplorer.Servicios;

// Fechas y números se presentan con el formato habitual de España.
CultureInfo cultura = new("es-ES");
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Este fichero opcional permite una alternativa local a User Secrets.
builder.Configuration.AddJsonFile(
    "appsettings.Local.json",
    optional: true,
    reloadOnChange: true);

// Razor Pages aporta páginas, formularios, validación y protección antifalsificación.
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();
builder.Services.Configure<NasaOpciones>(
    builder.Configuration.GetSection("Nasa"));

// SQLite contiene en el mismo contexto las tablas de Identity y favoritos.
string cadenaConexion = builder.Configuration.GetConnectionString("NasaContext")
    ?? "Data Source=nasa-explorer.db";
builder.Services.AddDbContext<NasaContext>(opciones =>
    opciones.UseSqlite(cadenaConexion));

// No se exige confirmar el correo: es un proyecto docente y local.
builder.Services
    .AddIdentity<Usuario, IdentityRole>(opciones =>
    {
        opciones.SignIn.RequireConfirmedAccount = false;
        opciones.SignIn.RequireConfirmedEmail = false;
        opciones.User.RequireUniqueEmail = true;
        opciones.Password.RequiredLength = 6;
        opciones.Password.RequireDigit = true;
        opciones.Password.RequireLowercase = true;
        opciones.Password.RequireUppercase = false;
        opciones.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<NasaContext>()
    .AddErrorDescriber<ErroresIdentityEnEspanol>()
    .AddDefaultTokenProviders();

// La cookie recuerda la URL original para volver tras iniciar sesión.
builder.Services.ConfigureApplicationCookie(opciones =>
{
    opciones.LoginPath = "/Cuenta/Login";
    opciones.AccessDeniedPath = "/Cuenta/AccesoDenegado";
    opciones.ExpireTimeSpan = TimeSpan.FromDays(14);
    opciones.SlidingExpiration = true;
});

// Cada fuente tiene su dirección base y puede fallar de forma independiente.
builder.Services.AddHttpClient("nasa", cliente =>
{
    cliente.BaseAddress = new Uri("https://api.nasa.gov/");
    cliente.Timeout = TimeSpan.FromSeconds(25);
});
builder.Services.AddHttpClient("imagenes", cliente =>
{
    cliente.BaseAddress = new Uri("https://images-api.nasa.gov/");
    cliente.Timeout = TimeSpan.FromSeconds(25);
});
builder.Services.AddHttpClient("epic", cliente =>
{
    cliente.BaseAddress = new Uri("https://epic.gsfc.nasa.gov/");
    cliente.Timeout = TimeSpan.FromSeconds(25);
});
builder.Services.AddHttpClient("eonet", cliente =>
{
    cliente.BaseAddress = new Uri("https://eonet.gsfc.nasa.gov/api/v3/");
    cliente.Timeout = TimeSpan.FromSeconds(25);
});
builder.Services.AddHttpClient("donki", cliente =>
{
    cliente.BaseAddress = new Uri("https://kauai.ccmc.gsfc.nasa.gov/DONKI/WS/get/");
    cliente.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient("exoplanetas", cliente =>
{
    cliente.BaseAddress = new Uri(
        "https://exoplanetarchive.ipac.caltech.edu/TAP/");
    cliente.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<INasaServicio, NasaServicio>();
builder.Services.AddScoped<IFavoritosServicio, FavoritosServicio>();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// EnsureCreated simplifica el primer arranque para el alumnado.
// En un proyecto real convendría sustituirlo por migraciones de EF Core.
using (IServiceScope scope = app.Services.CreateScope())
{
    NasaContext context = scope.ServiceProvider.GetRequiredService<NasaContext>();
    await context.Database.EnsureCreatedAsync();
}

app.Run();
