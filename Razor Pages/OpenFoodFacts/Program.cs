using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenFoodFacts.Configuracion;
using OpenFoodFacts.Data;
using OpenFoodFacts.Modelos;
using OpenFoodFacts.Servicios;

// CreateBuilder prepara la configuración, los servicios y el servidor.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Este archivo opcional permite cambiar el contacto sin subirlo a Git.
builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

// Creamos la carpeta antes de que SQLite abra el archivo.
Directory.CreateDirectory(
    Path.Combine(builder.Environment.ContentRootPath, "Data"));

builder.Services.AddDbContext<AlimentosContext>(opciones =>
    opciones.UseSqlite(
        builder.Configuration.GetConnectionString("OpenFoodFacts")));

// Identity administra cuentas, contraseñas, cookies y bloqueos.
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
    .AddEntityFrameworkStores<AlimentosContext>()
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

builder.Services.Configure<OpenFoodFactsOpciones>(
    builder.Configuration.GetSection(OpenFoodFactsOpciones.Seccion));

builder.Services.AddHttpClient<
    IOpenFoodFactsServicio,
    OpenFoodFactsServicio>((proveedor, cliente) =>
    {
        OpenFoodFactsOpciones opciones = proveedor
            .GetRequiredService<IOptions<OpenFoodFactsOpciones>>()
            .Value;

        cliente.BaseAddress =
            new Uri("https://world.openfoodfacts.org/");
        cliente.Timeout = TimeSpan.FromSeconds(25);

        // El servicio externo exige identificar la aplicación y un contacto.
        cliente.DefaultRequestHeaders.UserAgent.ParseAdd(
            $"OpenFoodFactsRazor/1.0 ({opciones.ContactoValido})");
    });

builder.Services.AddScoped<IColeccionServicio, ColeccionServicio>();

WebApplication app = builder.Build();

// En el primer arranque se crean la base de datos y todas sus tablas.
using (IServiceScope ambito = app.Services.CreateScope())
{
    AlimentosContext contexto =
        ambito.ServiceProvider.GetRequiredService<AlimentosContext>();

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
