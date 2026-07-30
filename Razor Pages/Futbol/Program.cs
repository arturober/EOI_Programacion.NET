using System.Globalization;
using Futbol.Configuracion;
using Futbol.Data;
using Futbol.Modelos;
using Futbol.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Los textos, números y fechas se muestran con formato de España.
CultureInfo cultura = new("es-ES");
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

// CreateBuilder prepara la configuración, los servicios y el servidor.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// El archivo local es opcional y nunca debe subirse al repositorio.
builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

// SQLite necesita que la carpeta exista antes de abrir la base de datos.
Directory.CreateDirectory(
    Path.Combine(builder.Environment.ContentRootPath, "Data"));

builder.Services.AddDbContext<FutbolContext>(opciones =>
    opciones.UseSqlite(
        builder.Configuration.GetConnectionString("Futbol")));

// Identity gestiona usuarios, contraseñas seguras, cookies y bloqueos.
builder.Services
    .AddIdentity<Usuario, IdentityRole>(opciones =>
    {
        opciones.User.RequireUniqueEmail = true;
        opciones.SignIn.RequireConfirmedEmail = false;

        opciones.Password.RequiredLength = 8;
        opciones.Password.RequireUppercase = true;
        opciones.Password.RequireLowercase = true;
        opciones.Password.RequireDigit = true;
        opciones.Password.RequireNonAlphanumeric = false;

        opciones.Lockout.MaxFailedAccessAttempts = 5;
        opciones.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(5);
        opciones.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<FutbolContext>()
    .AddErrorDescriber<ErroresIdentityEnEspanol>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opciones =>
{
    opciones.LoginPath = "/Cuenta/Login";
    opciones.AccessDeniedPath = "/Cuenta/AccesoDenegado";
    opciones.Cookie.HttpOnly = true;
    opciones.ExpireTimeSpan = TimeSpan.FromDays(14);
    opciones.SlidingExpiration = true;
});

builder.Services.Configure<FootballDataOpciones>(
    builder.Configuration.GetSection(FootballDataOpciones.Seccion));

// El cliente mantiene la URL base, pero el token se añade en cada petición.
builder.Services.AddHttpClient<IFutbolServicio, FutbolServicio>(cliente =>
{
    cliente.BaseAddress =
        new Uri("https://api.football-data.org/v4/");
    cliente.Timeout = TimeSpan.FromSeconds(20);
    cliente.DefaultRequestHeaders.UserAgent.ParseAdd(
        "FutbolRazorEducativo/1.0");
});

builder.Services.AddScoped<IFavoritosServicio, FavoritosServicio>();

WebApplication app = builder.Build();

// Para facilitar la práctica, las tablas se crean en el primer arranque.
using (IServiceScope ambito = app.Services.CreateScope())
{
    FutbolContext contexto =
        ambito.ServiceProvider.GetRequiredService<FutbolContext>();

    await contexto.Database.EnsureCreatedAsync();
}

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

app.Run();
