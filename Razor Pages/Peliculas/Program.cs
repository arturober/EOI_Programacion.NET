using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Peliculas.Configuracion;
using Peliculas.Data;
using Peliculas.Modelos;
using Peliculas.Servicios;

// CreateBuilder prepara la configuración, los servicios y el servidor web.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Este archivo opcional permite trabajar con claves locales sin subirlos a Git.
// Las variables de entorno se añaden después para que tengan mayor prioridad.
builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true)
    .AddEnvironmentVariables();

// Razor Pages se ocupa de las páginas HTML y los formularios.
builder.Services.AddRazorPages();

// Los controladores exponen una pequeña API JSON educativa.
builder.Services.AddControllers();

// La caché evita repetir continuamente las mismas llamadas a TMDB.
builder.Services.AddMemoryCache();

// Creamos la carpeta antes de que SQLite intente abrir el archivo.
Directory.CreateDirectory(
    Path.Combine(builder.Environment.ContentRootPath, "Data"));

// Entity Framework utilizará la cadena Peliculas definida en appsettings.json.
builder.Services.AddDbContext<PeliculasContext>(opciones =>
    opciones.UseSqlite(
        builder.Configuration.GetConnectionString("Peliculas")));

// Identity administra usuarios, contraseñas, cookies y bloqueos de seguridad.
builder.Services
    .AddIdentity<Usuario, IdentityRole>(opciones =>
    {
        // El correo se utiliza como identificador único de acceso.
        opciones.User.RequireUniqueEmail = true;

        // No se solicita confirmación por correo electrónico.
        opciones.SignIn.RequireConfirmedEmail = false;

        // Una contraseña didáctica, pero razonablemente segura.
        opciones.Password.RequiredLength = 8;
        opciones.Password.RequireUppercase = true;
        opciones.Password.RequireLowercase = true;
        opciones.Password.RequireDigit = true;
        opciones.Password.RequireNonAlphanumeric = false;

        // Se bloquea temporalmente una cuenta tras cinco intentos fallidos.
        opciones.Lockout.MaxFailedAccessAttempts = 5;
        opciones.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        opciones.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<PeliculasContext>()
    .AddErrorDescriber<ErroresIdentityEnEspanol>()
    .AddDefaultTokenProviders();

// Indicamos a Identity dónde están nuestras páginas de cuenta.
builder.Services.ConfigureApplicationCookie(opciones =>
{
    opciones.LoginPath = "/Cuenta/Login";
    opciones.AccessDeniedPath = "/Cuenta/AccesoDenegado";
    opciones.Cookie.HttpOnly = true;
    opciones.ExpireTimeSpan = TimeSpan.FromDays(14);
    opciones.SlidingExpiration = true;
});

// Los datos se leen desde appsettings, claves o variables de entorno.
builder.Services.Configure<TmdbOpciones>(
    builder.Configuration.GetSection(TmdbOpciones.Seccion));

// El cliente tipado centraliza todas las llamadas a la API externa.
builder.Services.AddHttpClient<ITmdbServicio, TmdbServicio>(cliente =>
{
    cliente.BaseAddress = new Uri("https://api.themoviedb.org/3/");
    cliente.Timeout = TimeSpan.FromSeconds(15);
});

// Este servicio contiene toda la lógica de favoritos y de SQLite.
builder.Services.AddScoped<IFavoritosServicio, FavoritosServicio>();

// Build termina de configurar los servicios y crea la aplicación.
WebApplication app = builder.Build();

// En el primer arranque se crean automáticamente la base de datos y sus tablas.
using (IServiceScope ambito = app.Services.CreateScope())
{
    PeliculasContext contexto =
        ambito.ServiceProvider.GetRequiredService<PeliculasContext>();

    await contexto.Database.EnsureCreatedAsync();
}

// En producción se ocultan los detalles internos de las excepciones.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication identifica al usuario y Authorization aplica [Authorize].
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

// Run inicia el servidor y lo deja esperando peticiones.
app.Run();
