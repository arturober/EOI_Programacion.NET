using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Recetas.Configuracion;
using Recetas.Data;
using Recetas.Modelos;
using Recetas.Servicios;

// CreateBuilder prepara configuración, servicios y servidor.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Este archivo opcional permite usar una clave propia sin subirla a Git.
builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

// Creamos la carpeta antes de que SQLite abra el archivo.
Directory.CreateDirectory(
    Path.Combine(builder.Environment.ContentRootPath, "Data"));

builder.Services.AddDbContext<RecetasContext>(opciones =>
    opciones.UseSqlite(
        builder.Configuration.GetConnectionString("Recetas")));

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
    .AddEntityFrameworkStores<RecetasContext>()
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

builder.Services.Configure<TheMealDbOpciones>(
    builder.Configuration.GetSection(TheMealDbOpciones.Seccion));

builder.Services.AddHttpClient<ITheMealDbServicio, TheMealDbServicio>(
    cliente =>
    {
        cliente.BaseAddress =
            new Uri("https://www.themealdb.com/api/json/v1/");
        cliente.Timeout = TimeSpan.FromSeconds(20);
        cliente.DefaultRequestHeaders.UserAgent.ParseAdd(
            "RecetasRazor/1.0");
    });

builder.Services.AddScoped<IColeccionServicio, ColeccionServicio>();

WebApplication app = builder.Build();

// En el primer arranque se crean la base de datos y sus tablas.
using (IServiceScope ambito = app.Services.CreateScope())
{
    RecetasContext contexto =
        ambito.ServiceProvider.GetRequiredService<RecetasContext>();

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
