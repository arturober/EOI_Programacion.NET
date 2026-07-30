using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Videojuegos.Configuracion;
using Videojuegos.Data;
using Videojuegos.Modelos;
using Videojuegos.Servicios;

// CreateBuilder prepara configuración, servicios y servidor.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Este archivo opcional permite configurar la clave sin subirla a Git.
builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

// Creamos la carpeta antes de que SQLite abra el archivo.
Directory.CreateDirectory(
    Path.Combine(builder.Environment.ContentRootPath, "Data"));

builder.Services.AddDbContext<VideojuegosContext>(opciones =>
    opciones.UseSqlite(
        builder.Configuration.GetConnectionString("Videojuegos")));

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
    .AddEntityFrameworkStores<VideojuegosContext>()
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

builder.Services.Configure<RawgOpciones>(
    builder.Configuration.GetSection(RawgOpciones.Seccion));

// La clave se añade en el servidor y nunca se envía al navegador.
builder.Services.AddHttpClient<IRawgServicio, RawgServicio>(cliente =>
{
    cliente.BaseAddress = new Uri("https://api.rawg.io/api/");
    cliente.Timeout = TimeSpan.FromSeconds(20);
    cliente.DefaultRequestHeaders.UserAgent.ParseAdd(
        "VideojuegosRazor/1.0");
});

builder.Services.AddScoped<IBibliotecaServicio, BibliotecaServicio>();

WebApplication app = builder.Build();

// En el primer arranque se crean la base de datos y sus tablas.
using (IServiceScope ambito = app.Services.CreateScope())
{
    VideojuegosContext contexto =
        ambito.ServiceProvider.GetRequiredService<VideojuegosContext>();

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
