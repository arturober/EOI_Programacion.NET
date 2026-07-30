using Biblioteca.Configuracion;
using Biblioteca.Data;
using Biblioteca.Modelos;
using Biblioteca.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// CreateBuilder prepara configuración, servicios y servidor.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Este archivo opcional permite personalizar el contacto sin subirlo a Git.
builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// Creamos la carpeta antes de que SQLite abra el archivo.
Directory.CreateDirectory(
    Path.Combine(builder.Environment.ContentRootPath, "Data"));

builder.Services.AddDbContext<BibliotecaContext>(opciones =>
    opciones.UseSqlite(
        builder.Configuration.GetConnectionString("Biblioteca")));

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
    .AddEntityFrameworkStores<BibliotecaContext>()
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

builder.Services.Configure<OpenLibraryOpciones>(
    builder.Configuration.GetSection(OpenLibraryOpciones.Seccion));

// Open Library no necesita clave; el servicio añade un User-Agent.
builder.Services.AddHttpClient<IOpenLibraryServicio, OpenLibraryServicio>(
    cliente =>
    {
        cliente.BaseAddress = new Uri("https://openlibrary.org/");
        cliente.Timeout = TimeSpan.FromSeconds(20);
    });

builder.Services.AddScoped<IFavoritosServicio, FavoritosServicio>();

WebApplication app = builder.Build();

// En el primer arranque se crean la base de datos y sus tablas.
using (IServiceScope ambito = app.Services.CreateScope())
{
    BibliotecaContext contexto =
        ambito.ServiceProvider.GetRequiredService<BibliotecaContext>();

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

app.MapControllers();
app.MapRazorPages();

app.Run();
